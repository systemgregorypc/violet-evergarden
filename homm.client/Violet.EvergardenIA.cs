using System;
using System.Linq;
using HoMM.Sensors;
using HoMM;
using HoMM.ClientClasses;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace Homm.Client
{
    // Класс ИИ
    class BestAI
    {
        // Поля
        public HommSensorData sensorData;
        public HommClient client;
        public MyMap myMap; // Копия карты

        // Массив всевозможных передвижений
        public Chain[] directions = new Chain[]
        {
            new Chain(-1,-1, ((int)Direction.LeftUp).ToString()), // Влево-вверх
            new Chain(-1, 1, ((int)Direction.LeftDown).ToString()), // Влево-вниз
            new Chain( 1,-1, ((int)Direction.RightUp).ToString()), // Вправо-вверх
            new Chain( 1, 1, ((int)Direction.RightDown).ToString()), // Вправо-вниз
            new Chain( 0,-1, ((int)Direction.Up).ToString()), // Вверх
            new Chain( 0, 1, ((int)Direction.Down).ToString()) // Вниз
        };

        // Конструктор
        public BestAI(HommSensorData sensorData, HommClient client)
        {
            this.sensorData = sensorData;
            this.client = client;
            myMap = new MyMap(sensorData.Map.Width, sensorData.Map.Height);
            myMap.UpdateMap(sensorData);
        }

        // Попытка реализовать метод ThinkingWhatToDoNext блок-схемы
        public void ThinkingWhatToDoNext()
        {
            int step = 0;
            while (true)
            {
                step++;
                Console.WriteLine($"\nStep №:{step}\n");
                try
                {
                    if (sensorData.IsDead)
                    {
                        client.Wait(2);
                    }
                    // Находимся ли мы в таверне и сколько юнитов можем купить
                    int hireUnits = HireUnit(sensorData.Location);
                    // Если можем нанять хоть кого-нибудь в таверне
                    if (hireUnits != 0)
                    {
                        // Нанимаем
                        client.HireUnits(hireUnits);
                    }

                    Cell entity = new Cell();

                    entity = FindEnemy();
                    if (entity != null)
                    {
                        if (entity.X != -1 && entity.Y != -1)
                        {
                            Chain heh = AStarSolver((Cell)sensorData.Location, entity);
                            client.Move(StringToDirection(heh.path)[0]);
                            continue;
                        }
                    }

                    // Список ближайших объектов
                    Dictionary<Cell, string> nearestStuff = new Dictionary<Cell, string>();

                    // Если у нас нет золотой шахты, но есть неохраняемая шахта, добавляем ее в список на проверку
                    // Ищем шахту
                    entity = FindMine();
                    if (entity != null)
                    {
                        if (entity.X != -1 && entity.Y != -1)
                        {
                            nearestStuff.Add(entity, "Mine");
                        }
                    }

                    // Ищем таверну
                    entity = FindSecurityDwelling();

                    if (entity != null)
                    {
                        if (entity.X != -1 && entity.Y != -1)
                        {
                            nearestStuff.Add(entity, "Dwelling");
                        }
                    }

                    // Ищем ресурс
                    entity = FindResource();

                    if (entity != null)
                    {
                        if (entity.X != -1 && entity.Y != -1)
                        {
                            nearestStuff.Add(entity, "Resource");
                        }
                    }
                    // Если список не пустой
                    if (nearestStuff.Count() != 0)
                    {
                        Chain heh = FindNearest(nearestStuff.Keys.ToList());
                        client.Move(StringToDirection(heh.path)[0]);
                    }
            }
                catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

        }

        }

        // Проверка видим ли мы врага
        private Cell FindEnemy()
        {
            var heroes = sensorData.Map.Objects.Where(o => o.Hero != null).Select(o => o);
            MapObjectData enemy = null;

            if (heroes.Count() == 1)
            {
                return new Cell();
            }
            enemy = heroes.Where(o => o.Hero.Name != sensorData.MyRespawnSide).
                Select(o => o).FirstOrDefault();

            // Если врага нет, переходим к следующему действию
            if (enemy == null)
            {
                return new Cell();
            }

            int enemyArmy = 0;
            // Для каждого типа юнита записываем его потери
            foreach (var item in enemy.Hero.Army)
            {
                enemyArmy += item.Value;
            }

            if ((Cell)enemy.Location == new Cell(0, 0) && enemyArmy == 0)
            {
                return new Cell();
            }

            // Оцениваем исход боя в случае нападения на врага
            var attack = Combat.Resolve(new ArmiesPair(sensorData.MyArmy, enemy.Hero.Army));

            // Если мы проигрываем в нападении
            if (attack.IsDefenderWin)
            {
                FindSecurityDwelling();
            }

            // Оцениваем исход боя в случае нападения врага
            var defend = Combat.Resolve(new ArmiesPair(enemy.Hero.Army, sensorData.MyArmy));
            // Если мы выигрываем в обороне
            if (defend.IsDefenderWin)
            {
                // Возвращаем координату врага
                return (Cell)enemy.Location;
            }

            return FindSecurityDwelling();
        }

        // Нанимаем юнита в таверне по посылаемым координатам
        private int HireUnit(LocationInfo coord)
        {
            // Ищем таверну по посылаем координатам
            var curDw = myMap.dwellings.
                    Where(dw => dw == (TopItem)coord).
                    Select(dw => dw).FirstOrDefault();
            // Если по этой координате находится таверна
            if (curDw != null)
            {
                // Смотрим доступное число юнитов для найма
                int minPossible = curDw.dwellingIsHere.AvailableToBuyCount;
                // Определяем их тип
                UnitType unitType = curDw.dwellingIsHere.UnitType;
                // Если у нас нет денег или здесь нет юнитов возвращаем false
                if (IHaveARes(Resource.Gold) <= 0 || minPossible <= 0)
                {
                    return 0;
                }

                // Иначе получаем словарь стоймости юнитов данного типа 
                var dictionary = UnitsConstants.Current.UnitCost.Where(t => t.Key.Equals(unitType)).Select(t => t.Value).FirstOrDefault();

                // Проверяем наличие ресурсов 
                foreach (var item in dictionary)
                {
                    // Если у нас есть ресусры данного типа на покупку хотя бы одного юнита 
                    if (IHaveARes(item.Key) >= item.Value)
                    {
                        // Смотрим сколько юнитов можем купить имея только данный ресурс 
                        // и сравниваем с min 
                        if (minPossible > IHaveARes(item.Key) / item.Value)
                        {
                            minPossible = IHaveARes(item.Key) / item.Value;
                        }

                        continue;
                    }
                    // Иначе если у нас нет какого либо ресурса, возвращаем 0 
                    else
                    {
                        return 0;
                    }
                }
                return minPossible;
            }
            return 0;
        }

        // Проверяем какое количество ресурсов посылаемого типа у нас есть в наличии
        private int IHaveARes(Resource res)
        {
            return sensorData.MyTreasury.Where(tr => tr.Key.Equals(res)).Select(tr => tr.Value).FirstOrDefault();
        }

        // Поиск ресурсов 
        private Cell FindResource()
        {
            // Получаем все ресурсы из памяти 
            var resInMind = myMap.resources.
            Where(res => res.resourceIsHere != null).
            Select(res => res).ToList();

            int goldCount = IHaveARes(Resource.Gold);
            if (goldCount == 0)
            {
                var pes = resInMind.
                Where(res => res.resourceIsHere.Resource == Resource.Gold).
                Select(coord => new Cell(coord.X, coord.Y)).ToList();

                Chain nearGold = FindNearest(pes);
                return nearGold;
            }
            else
            {
                // Находим самый ближайший к нам 
                Chain nearRes = FindNearest(resInMind.Select(res => new Cell(res.X, res.Y)).ToList());
                // Если такой ресурс был найден, делаем шаг по направлению к нему 
                if (nearRes != null)
                {
                    return nearRes;
                }
            }
            return new Cell();
        }


        // Метод нахождения таверны
        private Cell FindSecurityDwelling()
        {
            // Получаем список наших сокровищ
            Dictionary<Resource, int> localTreasure = sensorData.MyTreasury.
                Select(t => new { t.Key, t.Value }).
                ToDictionary(t => t.Key, t => t.Value);

            // Если есть золото
            if (localTreasure[0] > 0)
            {
                var heroes = sensorData.Map.Objects.
                    Where(o => o.Hero != null).Select(o => o);
                MapObjectData enemy = null;

                if (heroes.Count() != 1)
                {
                    enemy = heroes.Where(o => !o.Hero.Name.
                    Equals(sensorData.MyRespawnSide)).
                    Select(o => o).FirstOrDefault();
                }

                // Список ближайших таверн
                List<Cell> nearDwellings = new List<Cell>();
                // По каждому ресурсу, имеющимуся у нас смотрим
                foreach (var item in localTreasure)
                {
                    // Если данного типа ресурса нет, смотрим следующий
                    if (item.Value == 0)
                    {
                        continue;
                    }
                    // Для каждого типа ресурса делаем соответствие с типом нанимаего 
                    // юнита
                    int proverochka = -1;
                    switch (item.Key)
                    {
                        case Resource.Glass: // Если ресурс стекло - то ищем лучников
                            proverochka = 1;
                            break;
                        case Resource.Iron: // Железо - пехота
                            proverochka = 0;
                            break;
                        case Resource.Ebony: // Эбонит - конница
                            proverochka = 2;
                            break;
                        case Resource.Gold: // Золото - ополчение
                        default:
                            proverochka = 3;
                            break;
                    }

                    // Ищем все таверны послыаемого типа юнитов
                    var dwInMind = myMap.dwellings.Where(count => count.dwellingIsHere.AvailableToBuyCount > 0).
                        Where(t => (int)t.dwellingIsHere.UnitType == proverochka).
                        Select(cord => new Cell(cord.X, cord.Y)).ToList();

                    // Добавляем ближайшую таверну данного типа в список
                    if (dwInMind.Count() != 0)
                    {
                        //если в данной таверне мы не можем нанять ни одного юнита, 
                        //удаляем её из списка таверн, подлежащих проверке
                        //грёбанные рыцари!!!
                        for (int i = 0; i < dwInMind.Count(); i++)
                        {
                            if (HireUnit(new LocationInfo(dwInMind[i].X, dwInMind[i].Y)) == 0)
                            {
                                dwInMind.RemoveAt(i);
                                i--;
                            }
                        }
                        int count = 0;
                        while (dwInMind.Count > 0)
                        {
                            count++;
                            if (count > 15)
                            {
                                return new Cell();
                            }
                            var securityDw = FindNearest(dwInMind);
                            //если я бегу к таверне дольше врага
                            if (enemy != null)
                            {
                                var enemyTravel = AStarSolver((Cell)enemy.Location, new Cell(securityDw.X, securityDw.Y));

                                if (enemyTravel == new Cell())
                                {
                                    break;
                                }
                                if (securityDw.F > enemyTravel.F)
                                {
                                    dwInMind.Remove(securityDw);
                                }
                                else
                                {
                                    break;
                                }
                                
                            }
                            else
                            {
                                break;
                            }
                        }
                        nearDwellings.Add(FindNearest(dwInMind));
                    }
                }
                // Если была найдена хоть одна таверна, возвращаем координату ближайшей
                if (nearDwellings != null)
                {
                    return FindNearest(nearDwellings);
                }
            }
            // Если ничего не было найдено, возвращаем пустую ячейку
            return new Cell();
        }

        // Метод нахождения таверны
        private Cell FindDwelling()
        {
            // Получаем список наших сокровищ
            Dictionary<Resource, int> localTreasure = sensorData.MyTreasury.
                Select(t => new { t.Key, t.Value }).
                ToDictionary(t => t.Key, t => t.Value);

            // Если есть золото
            if (localTreasure[0] > 0)
            {
                // Список ближайших таверн
                List<Cell> nearDwellings = new List<Cell>();
                // По каждому ресурсу, имеющимуся у нас смотрим
                foreach (var item in localTreasure)
                {
                    // Если данного типа ресурса нет, смотрим следующий
                    if (item.Value == 0)
                    {
                        continue;
                    }
                    // Для каждого типа ресурса делаем соответствие с типом нанимаего 
                    // юнита
                    int proverochka = -1;
                    switch (item.Key)
                    {
                        case Resource.Glass: // Если ресурс стекло - то ищем лучников
                            proverochka = 1;
                            break;
                        case Resource.Iron: // Железо - пехота
                            proverochka = 0;
                            break;
                        case Resource.Ebony: // Эбонит - конница
                            proverochka = 2;
                            break;
                        case Resource.Gold: // Золото - ополчение
                        default:
                            proverochka = 3;
                            break;
                    }

                    // Ищем все таверны послыаемого типа юнитов
                    //var dwInMind = myMap.dwellings.Where(count => count.dwellingIsHere.AvailableToBuyCount > 0).
                    //    Where(t => (int)t.dwellingIsHere.UnitType == proverochka).
                    //    Select(cord => new Cell(cord.X, cord.Y));

                    var first = myMap.dwellings;

                    var second = first.Where(count => count.dwellingIsHere.AvailableToBuyCount > 0).Select(dw => dw);

                    var dwInMind = second.Where(t => (int)t.dwellingIsHere.UnitType == proverochka).Select(cord => new Cell(cord.X, cord.Y)).ToList();


                    // Добавляем ближайшую таверну данного типа в список
                    if (dwInMind.Count() != 0)
                    {
                        nearDwellings.Add(FindNearest(dwInMind));
                    }
                }
                // Если была найдена хоть одна таверна, возвращаем координату ближайшей
                if (nearDwellings != null)
                {
                    return FindNearest(nearDwellings);
                }
            }
            // Если ничего не было найдено, возвращаем пустую ячейку
            return new Cell();
        }

        private Cell FindMine()
        {
            // Ближайшая шахта
            Chain nearMines = new Chain();
            // Смотрим на все вражеские шахты
            var notMyMines = myMap.mines.Where(mine => mine.mineIsHere.Owner != sensorData.MyRespawnSide).Select(mine => mine);

            // Если вражеских шахт нет
            if (notMyMines == null)
            {
                // Возвращаем клетку с координатами (-1, -1)
                return new Cell();
            }

            // Если у нас есть хоть одна золотая шахта
            if (myMap.mines.Where(mine => mine.mineIsHere.Resource == Resource.Gold).Where(mine => mine.mineIsHere.Owner == sensorData.MyRespawnSide).Select(mine => mine).Count() != 0)
            {
                // Выбираем ближайшую шахту любого типа, которую можем захватить
                nearMines = FindNearest(notMyMines.Select(cord => new Cell(cord.X, cord.Y)).ToList());
            }
            // Иначе, если у нас нет ниодной золотой шахты
            else
            {
                // Из списка вражеских шахт находим все золотые
                var notMyGoldMines = notMyMines.Where(mine => mine.mineIsHere.Resource == Resource.Gold).Select(cord => new Cell(cord.X, cord.Y)).ToList();
                // Если у врага есть такая или несколько таких
                if (notMyGoldMines != null)
                {
                    // Выбираем ближайшую золотую шахту, к которой можем пройти
                    nearMines = FindNearest(notMyGoldMines);
                }

            }
            return nearMines;
        }

        // Метод поиска ближайшего из списка посылаемых координат
        private Chain FindNearest(List<Cell> objects)
        {
            Chain min = new Chain();
            min.G = double.MaxValue;
            foreach (var item in objects)
            {
                var heh = AStarSolver((Cell)sensorData.Location, item);

                // Если был найден более быстрый путь
                if (heh.G > 0 && heh.G < min.G)
                {
                    min = heh;
                }
            }
            // Если путь не был найден, возвращаем null, иначе возвращаем путь к этой 
            // ячейке
            return (min.path == string.Empty) ? null : min;
        }

        // Алгоритм А*
        public Chain AStarSolver(Cell start, Cell finish)
        {
            if (finish == null)
            {
                return new Chain(start);
            }
            Dictionary<UnitType, int> tempArmy = sensorData.MyArmy;
            // Список вершин, подлежащих проверке
            List<Chain> opened_list = new List<Chain>();
            // Список посещенных вершин
            List<Chain> visited_list = new List<Chain>();

            // Добавляем начальную вершину в список вершин подлежащих проверке
            opened_list.Add(new Chain(start));

            // Рассматриваемая переменная
            Chain current;

            // Пока открытый список не пустой
            while (opened_list.Count > 0)
            {
                // Извлекаем вершину с наименьшей оценкой F из открытого списка
                current = opened_list.
                    Where(cell => cell.F == opened_list.Min(mincell => mincell.F)).
                    Select(cell => cell).FirstOrDefault();

                // Удаляем эту вершину из открытого списка
                opened_list.Remove(current);
                // Добавляем ее в закрытый список
                visited_list.Add(current);

                // Рассматриваем все точки вокруг текущей вершины
                foreach (var side in directions)
                {
                    // Веришна после сдвига
                    var shift = current + side;
                    shift.G = current.G;

                    // Если эта вершина (внутри foreach говорим про вершину после сдвига)
                    // уже находится в закрытом списке, игнорируем ее
                    if (visited_list.Where(coord => coord == shift).
                        Select(o => o).FirstOrDefault() != null)
                    {
                        continue;
                    }
                    // Если эта вершина стена или точка находящееся не в пределах карты,
                    // игнорируем ее
                    if (!InRange(shift))
                    {
                        continue;
                    }
                    // Если на этой вершине находится враг, которого мы не в силах одолеть,
                    // игнориуем ее

                    int losses = InDanger(shift, tempArmy);

                    // Если мы проиграли бой, то игнорируем эту клетку
                    if (losses == -1)
                    {
                        continue;
                    }
                    // Иначе, наши потери влияют на оценку пути
                    else if (losses != 0)
                    {
                        shift.G += 2;
                        shift.F += 0.1 * losses;
                    }

                    // Записываем путь сдвига
                    // Путь не верный! Ты записывал только координату сдвига в путь и перезатирал 
                    // уже имеющийся
                    shift.path = current.path + side.path;

                    // Проверяем есть ли уже эта вершина в открытом списке
                    var inOpened_list = opened_list.
                        Where(cell => cell == shift).
                        Select(cell => cell).
                        FirstOrDefault();

                    // Если есть в открытом списке
                    if (inOpened_list != null)
                    {
                        // Проверяем оценки G вершин сдвига и найденной в открытом списке
                        // Если оценка G у сдвига больше чем у вершины из открытого списка
                        // игнорируем ее
                        if (shift.G > inOpened_list.G)
                        {
                            continue;
                        }
                        // Иначе, если путь в данную координату оказался быстрее
                        // (оценка G у вершины сдвига меньше)
                        else
                        {
                            // Перезаписываем путь в эту координату
                            opened_list[opened_list.IndexOf(inOpened_list)].path = shift.path;
                        }
                    }
                    // Иначе, если нет в открытом списке
                    else
                    {
                        // Вычисляем оценку H (для шестигранников)
                        if (finish.X == shift.X || finish.Y == shift.Y)
                        {
                            shift.H += 0.3;
                        }
                        shift.H += Math.Sqrt(Math.Pow((finish.X - shift.X), 2) + Math.Pow((finish.Y - shift.Y), 2));

                        // Вычисляем оценку G
                        shift.G += myMap.cells[shift.X, shift.Y].travel_cost;

                        // Вычисляем оценку F
                        shift.F += shift.H + shift.G;

                        // Добавляем эту вершину в открытый список 
                        opened_list.Add(shift);

                        // Если эта вершина является координатой финиша, то возвращаем эту вершину
                        if (shift == finish)
                        {
                            return shift;
                        }
                    }
                }
            }
            // Если путь не был найден, возвращаем координату старта
            return new Chain(start);
        }

        // Проверяем возможность пройти по посылаемой клетке
        private bool InRange(Chain shift)
        {
            // Если за границами карты, то не можем пройти
            if (shift.X < 0 || shift.X >= myMap.weight || shift.Y < 0 || shift.Y >= myMap.height)
            {
                return false;
            }
            // Если эта клетка - стена, то не моем пройти
            else if (myMap.cells[shift.X, shift.Y].travel_cost == -1)
            {
                return false;
            }
            // Если все проверки пройдены, то пройти можем
            return true;
        }

        // игнорируем эту клетку
        private int InDanger(Chain shift, Dictionary<UnitType, int> tempArmy)
        {
            Dictionary<UnitType, int> enemyArmy = new Dictionary<UnitType, int>();

            var curNeu = myMap.neutrals.Where(n => n.X == shift.X && n.Y == shift.Y).
                Select(n => n.neutralIsHere).FirstOrDefault();
            // Если там есть нейтралы 
            if (curNeu != null)
            {
                enemyArmy = curNeu.Army;
            }
            else
            {
                var curOb = myMap.GetCurrentCell(shift.X, shift.Y);
                // Если текущий объект пустой
                if (curOb == null)
                {
                    return 0;
                }

                // Если там есть нейтралы
                if (curOb.NeutralArmy != null)
                {
                    enemyArmy = curOb.NeutralArmy.Army;

                }
                //Если там есть не мой гарнизон
                else if (curOb.Garrison != null && !curOb.Garrison.Owner.Equals(sensorData.MyRespawnSide))
                {
                    enemyArmy = curOb.Garrison.Army;
                }
                //Если там соперник
                else if (curOb.Hero != null && !curOb.Hero.Name.Equals(sensorData.MyRespawnSide))
                {
                    enemyArmy = curOb.Hero.Army;
                }
                else
                {
                    return 0;
                }
            }

            // Проводим бой
            var fight = Combat.Resolve(new ArmiesPair(tempArmy, enemyArmy));

            // Если мы победили, записываем остатки армии
            if (fight.IsAttackerWin)
            {
                // Записываем наши остатки армии в tempArmy и считаем, что можем пройти по этой клетке

                // Потери в бою
                int losses = 0;

                // Для каждого типа юнита записываем его потери
                foreach (var item in tempArmy)
                {
                    // Смотрим число юнитов данного типа до боя
                    var beforeBattle = tempArmy.Where(o => o.Key == item.Key).Select(o => o.Value).FirstOrDefault();
                    // Смотрим число юнитов данного типа после боя
                    var afterBattle = fight.AttackingArmy.Where(o => o.Key == item.Key).Select(o => o.Value).FirstOrDefault();
                    // Записываем потери данного типа
                    losses += beforeBattle - afterBattle;
                }


                //var delta = tempArmy - fight.AttackingArmy;

                tempArmy = fight.AttackingArmy;

                return losses;
            }
            // Если выйграли нейтралы, игнорируем клетку
            else
            {
                return -1;
            }
        }

        // Метод, преобразующий посылаем путь в массив передвижений
        public static Direction[] StringToDirection(string path)
        {
            // Если строка пустая, возвращаем пустое передвижение
            if (path == null)
            {
                return new Direction[0];
            }
            // Преобразуем полученную строку в массив движений на игровом поле 
            Direction[] dir_path = new Direction[path.Length];

            for (int i = 0; i < dir_path.Length; i++)
            {
                switch (path[i])
                {
                    case '0':
                        dir_path[i] = Direction.Up;
                        break;
                    case '1':
                        dir_path[i] = Direction.Down;
                        break;
                    case '2':
                        dir_path[i] = Direction.LeftUp;
                        break;
                    case '3':
                        dir_path[i] = Direction.LeftDown;
                        break;
                    case '4':
                        dir_path[i] = Direction.RightUp;
                        break;
                    case '5':
                        dir_path[i] = Direction.RightDown;
                        break;
                    default:
                        break;
                }
            }
            return dir_path;
        }

        // Метод Updates, обработчик события, обновляет локальную карту
        public void Updates(HommSensorData sensorData)
        {
            this.sensorData = sensorData;

            myMap.UpdateMap(sensorData);
        }

    }
}

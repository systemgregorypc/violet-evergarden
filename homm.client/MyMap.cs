using System;
using System.Linq;
using HoMM.Sensors;
using HoMM;
using HoMM.ClientClasses;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Homm.Client
{
    // Класс MyMap - локальная запись всех открытых участков на карте
    class MyMap
    {
        private HommSensorData sensorData;
        public int weight; // Длина карты
        public int height; // Глубина карты

        public Chain[,] cells; // Список ячеек на карте
        public List<TopItem> mines = new List<TopItem>(); // Список найденных шахт
        public List<TopItem> dwellings = new List<TopItem>(); // Список найденных таверн
        public List<TopItem> neutrals = new List<TopItem>(); // Список найденых нейтралов 
        public List<TopItem> resources = new List<TopItem>(); // Список найденых ресурсов
        
        // Конструктор
        public MyMap(int weight, int height)
        {
            this.weight = weight;
            this.height = height;
            cells = new Chain[weight, height];
            InitMap();
        }

        // Метод-инициализатор, заполняет карту пустыми ячейками
        private void InitMap()
        {
            for (int h = 0; h < height; h++)
            {
                for (int w = 0; w < weight; w++)
                {
                    cells[w, h] = new Chain(w, h);
                }
            }
        }

        // Метод, возвращающий текущую клетку из sensorData по посылаемым координатам
        public MapObjectData GetCurrentCell(int w, int h)
        {
            return sensorData.Map.Objects.
                          Where(cell => cell.Location.X == w && cell.Location.Y == h).
                          Select(cell => cell).FirstOrDefault();
        }

        // Метод заполянющий выбранную ячейку
        private void FillCurrentCell(int w, int h)
        {
            var temp = GetCurrentCell(w, h);

            // Если клетка находится в тумане войны
            if (temp == null)
            {
                return;
            }
            else
            {
                // Если стена, то пройти нельзя
                if (temp.Wall != null)
                {
                    cells[w, h].travel_cost = -1;
                }
                // Если не стена
                else
                {
                    // Записываем стоймость передвижения по клетке
                    cells[w, h].travel_cost = TileTerrain.Parse(temp.Terrain.ToString()[0]).TravelCost;

                    // Если на этой клетке находится таверна, добавляем ее в список таверн
                    if (temp.Dwelling != null)
                    {

                        if (w == 0 && h == 0 && sensorData.MyRespawnSide.Equals("Right") || w == weight - 1 && h == height - 1 && sensorData.MyRespawnSide.Equals("Left"))
                        { }
                        else
                            dwellings.Add(new TopItem(w, h, temp.Dwelling));
                    }
                    // Если на этой клетке находится шахта, добавляем ее в список шахт
                    if (temp.Mine != null)
                    {
                        mines.Add(new TopItem(w, h, temp.Mine));
                    }
                    // Если здесь нейтралы 
                    if (temp.NeutralArmy != null)
                    {
                        neutrals.Add(new TopItem(w, h, temp.NeutralArmy));
                    }
                    // Если здесь кучка ресурсов 
                    if (temp.ResourcePile != null)
                    {
                        resources.Add(new TopItem(w, h, temp.ResourcePile));
                    }

                }
            }
        }

        // Метод обновления карты
        public void UpdateMap(HommSensorData data)
        {
            sensorData = data;
            for (int h = 0; h < height; h++)
            {
                for (int w = 0; w < weight; w++)
                {
                    // Если клетка пустая
                    if (cells[w, h].travel_cost == 0)
                    {
                        FillCurrentCell(w, h);
                    }
                }
            }

            UpdateDwelling();
            UpdateMine();
            UpdateResource();
            UpdateNeutral();
        }

        // Дописать для всех таверн, не только тех в которых мы наняли юнитов
        // Метод обновления таверн
        public void UpdateDwelling()
        {
            var fef = sensorData.Map.Objects.Where(o => o.Dwelling != null).Select(o => o);
            foreach (var item in fef)
            {
                int ind = dwellings.FindIndex(coord => coord == (TopItem)item.Location);
                if (ind != -1)
                {
                    dwellings[ind].dwellingIsHere = item.Dwelling;

                }
            }
        }

        // Дописать для всех таверн, не только тех в которых мы наняли юнитов
        // Метод обновления шахт
        public void UpdateMine()
        {
            var fef = sensorData.Map.Objects.Where(o => o.Mine != null).Select(o => o);

            foreach (var item in fef)
            {
                int ind = mines.FindIndex(coord => coord == (TopItem)item.Location);
                if (ind != -1)
                {
                    mines[ind].mineIsHere = item.Mine;
                }
            }
        }

        // Метод обновления ресурсов 
        private void UpdateResource()
        {
            for (int i = 0; i < resources.Count; i++)
            {
                var temp = GetCurrentCell(resources[i].X, resources[i].Y);
                //если клетка с проверяемым ресурсом видна 
                if (temp != null)
                {
                    //ресурса там нет 
                    if (temp.ResourcePile == null)
                    {
                        resources.RemoveAt(i);
                        i--;
                    }
                }
            }
        }

        // Метод обновления нейтралов 
        private void UpdateNeutral()
        {

            for (int i = 0; i < neutrals.Count; i++)
            {
                var temp = GetCurrentCell(neutrals[i].X, neutrals[i].Y);

                if (temp != null)
                {
                    if (temp.NeutralArmy == null)
                    {
                        neutrals.RemoveAt(i);
                        i--;
                    }
                }
            }
        }
    }
}

using System;
using System.Linq;
using HoMM.Sensors;
using HoMM;
using HoMM.ClientClasses;
using System.Collections.Generic;
using System.Diagnostics;

namespace Homm.Client
{
    // Класс Cell - координаты
    class Cell
    {
        // Поля-координаты
        public int X;
        public int Y;

        // Конструкторы
        public Cell()
        {
            X = -1;
            Y = -1;
        }
        public Cell(int ax, int ay) : this()
        {
            X = ax;
            Y = ay;
        }

        // явное приведение типа LocationInfo
        public static explicit operator Cell(LocationInfo v) 
        {
            return new Cell(v.X, v.Y);
        }
        // явное приведение типа Direction
        public static explicit operator Cell(Direction point)
        {
            switch (point)
            {
                case Direction.Up:
                    return new Cell(0, -1);
                case Direction.Down:
                    return new Cell(0, 1);
                case Direction.LeftUp:
                    return new Cell(-1, -1);
                case Direction.LeftDown:
                    return new Cell(-1, 1);
                case Direction.RightUp:
                    return new Cell(1, -1);
                case Direction.RightDown:
                    return new Cell(1, 1);
                default:
                    break;
            }
            return new Cell(0, 0);
        }

        // Переопределение оператора ==
        public static bool operator ==(Cell left, Cell right)
        {
            // If both are null, or both are same instance, return true. 
            if (System.Object.ReferenceEquals(left, right))
            {
                return true;
            }

            // If one is null, but not both, return false. 
            if (((object)left == null) || ((object)right == null))
            {
                return false;
            }

            // Return true if the fields match: 
            return left.X == right.X && left.Y == right.Y;
        }
        // Переопределение оператора !=
        public static bool operator !=(Cell left, Cell right)
        {
            return !(left == right);
        }
        // Переопределние оперптора сложения
        public static Cell operator +(Cell current, Cell shift)
        {
            //В зависимости от координаты Х шестигранника current меняется алгоритм перемещения по диагоналям
            //Если мы совершаем движение со сдвигом по координате Х (в сторону)
            if (shift.X != 0) // X==1 || X==-1
            {
                //в зависимости от текущего положения
                //не изменяем координату Y, если Х был чётный, а движение совершалось в сторону и вниз
                //или если X был не чётный, а движение совершалось в сторону и вверх
                if (current.X % 2 == 0 && shift.Y == 1 || current.X % 2 == 1 && shift.Y == -1)
                {
                    return new Cell(current.X + shift.X, current.Y);
                }
            }
            // возвращаем координатную сумму
            return new Chain(current.X + shift.X, current.Y + shift.Y);
        }
        
    }

    // Класс Chain
    class Chain : Cell
    {
        public string path; // Поле, запоминающее путь перемещения объекта
        public double travel_cost; // Поле, считающее время пути path
        public double F; // Оценка F = H + G
        public double H; // Оценка H = (finish.x - start.x) + (finish.y - start.y)
        public double G; // Оценка G = стоймость пути в данной точке

        // Конструкторы
        public Chain() : base()
        {
            path = string.Empty;
            travel_cost = 0;
            F = H = G = 0;
        }
        public Chain(int ax, int ay) : base(ax, ay) { }
        public Chain(Cell cell) : base(cell.X, cell.Y) { }
        public Chain(int ax, int ay, double travel_time) : base(ax, ay)
        {
            this.travel_cost = travel_time;
        }
        public Chain(int ax, int ay, string path) : base(ax, ay)
        {
            this.path = path;
        }

        // Переопределение оператора сложения
        public static Chain operator +(Chain current, Chain shift)
        {
            //В зависимости от координаты Х шестигранника current меняется алгоритм перемещения по диагоналям
            //Если мы совершаем движение со сдвигом по координате Х (в сторону)
            if (shift.X != 0) // X==1 || X==-1
            {
                //в зависимости от текущего положения
                //не изменяем координату Y, если Х был чётный, а движение совершалось в сторону и вниз
                //или если X был не чётный, а движение совершалось в сторону и вверх
                if (current.X % 2 == 0 && shift.Y == 1 || current.X % 2 == 1 && shift.Y == -1)
                {
                    return new Chain(current.X + shift.X, current.Y);
                }
            }

            Chain temp = new Chain(current.X + shift.X, current.Y + shift.Y);
            //temp.G = current.G + shift.G;
            // возвращаем координатную сумму
            return temp;
        }
        // Явное приведение типа LocationInfo
        public static explicit operator Chain(LocationInfo v)
        {
            return new Chain(v.X, v.Y);
        }

    }

}

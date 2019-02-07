using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Move
{
    class Point
    {
        public int Left { get; set; }
        public int Top { get; set; }
    }
    class Wall
    {
        public int Left { get; set; }
        public int Top { get; set; }
    }
    /*
      class который хранит  состояние поля: стенки, позицию курсора.  */
    class Field
    {
        int barricades;
        public Wall[] WallsArray;

        public Field(int barricades) => this.barricades = barricades;

        public Point point;
    }
    /*
      класс, который отвечает за отрисовку поля на консоль. */
    class FieldOnConsole
    {
        public int Barricades { get; set; }
        public void CreateWalls()
        {
            Field field = new Field(Barricades)
            {
                WallsArray = new Wall[Barricades]
            };
        }
        public void SetStartCursor()
        {

        }
            
    }
    /*
    класс который отвечает за перемещение курсора по полю.  */
    class MoveOnField
    {

    }
    class Program
    {
        static void Main(string[] args)
        {
        }
    }
}

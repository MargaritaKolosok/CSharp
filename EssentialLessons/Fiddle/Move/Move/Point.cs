using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Move
{
    struct Point
    {
        public int Left { get; set; }
        public int Top { get; set; }
        public char PointSymbol { get; set; }

        public Point(int left, int top, char symb)
        {
            Left = left;
            Top = top;
            PointSymbol = symb;
        }
        public void Draw()
        {
            DrawPoint(PointSymbol);
        }
        void DrawPoint(char point)
        {
            Console.SetCursorPosition(Left, Top);
            Console.Write(point);
        }
        public void Clear()
        {
            DrawPoint(' ');
        }
        public void MoveLeft()
        {
            Left--;
        }
        public void MoveRight()
        {
            Left++;
        }
        public void MoveTop()
        {
            Top--;
        }
        public void MoveDown()
        {
            Top++;
        }
    }
}

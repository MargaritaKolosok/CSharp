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

        Grafic.GRAFIC_POINT GraficPoint;

        public Point(int left, int top, char symb)
        {
            Left = left;
            Top = top;
            PointSymbol = GraficPoint.SYMBOL;
        }

        public void Draw()
        {
            DrawPoint(GraficPoint);
        }

        void DrawPoint(IGraficPoint point)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.SetCursorPosition(Left, Top);
            Console.ForegroundColor = (ConsoleColor)point.FOREGROUND;
            Console.BackgroundColor = (ConsoleColor)point.BACKGROUND;
            Console.Write(point.SYMBOL);            
        }

        public void Clear()
        {
            DrawPoint(new Grafic.GRAFIC_SPACE());
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

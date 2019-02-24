using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Move
{
    static class Grafic
    {
        public static char BARRICADE = 'X';
        public static char WALL = '*';
        public static char BONUS = '$';
        public static char POINT = '\u263A';
        public static char EXIT = 'E';

      //  public static char GRAFIC_WALL = '\u2580';
      //  public static char GRAFIC_BARRICADE = '\u2591';
        
        public struct GRAFIC_WALL : IGraficPoint
        {
           public char SYMBOL { get { return '\u2580'; } }
           public int FOREGROUND { get { return 7; } }
           public int BACKGROUND { get { return 0; } }
        }
        public struct GRAFIC_POINT : IGraficPoint
        {
            public char SYMBOL { get { return '\u263A'; } }
            public int FOREGROUND { get { return 5; } }
            public int BACKGROUND { get { return 0; } }
        }
        public struct GRAFIC_BARRICADE : IGraficPoint
        {
            public char SYMBOL { get { return '\u2592'; } }
            public int FOREGROUND { get { return 12; } }
            public int BACKGROUND { get { return 15; } }
        }
        public struct GRAFIC_BONUS : IGraficPoint
        {
            public char SYMBOL { get { return '$'; } }
            public int FOREGROUND { get { return 14; } }
            public int BACKGROUND { get { return 10; } }
        }
        public struct GRAFIC_EXIT : IGraficPoint
        {
            public char SYMBOL { get { return 'E'; } }
            public int FOREGROUND { get { return 0; } }
            public int BACKGROUND { get { return 14; } }
        }
        public struct GRAFIC_SPACE : IGraficPoint
        {
            public char SYMBOL { get { return '.'; } }
            public int FOREGROUND { get { return 2; } }
            public int BACKGROUND { get { return 0; } }
        }


    }
}

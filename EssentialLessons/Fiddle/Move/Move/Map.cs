using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Move
{
    class Map
    {
        public static int width = 20, height = 20;
        public char[,] Walls = new char[20, 20];
        string level;

        int WALL_PERCENTAGE = 5;
        public int BONUS_COUNT = 10;
        public static char BARRICADE = 'X';
        public static char WALL = '*';
        public static char BONUS = '$';
        public static char POINT = '*';
        public static char EXIT = 'E';

        public Map()
        {
            Console.CursorVisible = false;
            GenerateBorder();
        }

        void DrawBarricade(char point, int left, int top)
        {
            Console.SetCursorPosition(left, top);
            Console.Write(point);
        }

        void GenerateBorder()
        {
            for (int top = 0; top < width; top++)
            {
                for (int left = 0; left < height; left++)
                {
                    Walls[top, left] = (top == 0 || top == width - 1 || left == 0 || left == height - 1) ? POINT : ' ';
                    char wallBuilder = Walls[top, left];
                    DrawBarricade(wallBuilder, left, top);
                }
            }
            GenerateBarricades(BARRICADE, WALL_PERCENTAGE);
            GenerateBarricades(BONUS, BONUS_COUNT);
        }
       public void GenerateBarricades(char ch, int barricades_count)
        {
            Random random = new Random();
            int counter = 0;
            int top, left;
            void Random()
            {
                left = random.Next(0, width);
                top = random.Next(0, width);                
            }
            while (counter < barricades_count)
            {
                Random();

                if (Walls[top, left] == ' ')
                {
                    Walls[top, left] = ch;
                    DrawBarricade(ch, left, top);
                    counter++;
                }                
            }
        }
      
        public bool IsBarricade(Point point)
        {
            if (Walls[point.Top, point.Left] == Map.BARRICADE || Walls[point.Top, point.Left] == Map.WALL)
            {
                return true;
            }
            else if (Walls[point.Top, point.Left] == Map.BONUS)
            {
                Bonus.Count++;
                Walls[point.Top, point.Left] = ' ';
                return false;
            }
            else if (Walls[point.Top, point.Left] == Map.EXIT)
            {
                Clear();                
                Game game = new Game();
                game.StartGame();
                return false;
            }
            else
            {
                return false;
            }
        }        
        public void Clear()
        {
            Console.Clear();
        }
    }
}

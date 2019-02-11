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
           
           // DrawBorder();            
        }

        void Barricade(char point, int left, int top)
        {
            Console.SetCursorPosition(left, top);
            Console.Write(point);
        }
        void GenerateBorder()
        {
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    Walls[i, j] = (i == 0 || i == width - 1 || j == 0 || j == height - 1) ? POINT : ' ';
                    char wallBuilder = Walls[i,j];
                    Barricade(wallBuilder, i, j);
                }
            }
            GenerateBarricades(BARRICADE, WALL_PERCENTAGE);
            GenerateBarricades(BONUS, BONUS_COUNT);
        }
       public void GenerateBarricades(char ch, int barricades_count)
        {
            Random random = new Random();
            int counter = 0;
            int x, y;
            void Random()
            {
                x = random.Next(0, width);
                y = random.Next(0, width);
            }
            while (counter < barricades_count)
            {
                Random();

                if (Walls[x, y] == ' ')
                {
                    Walls[x, y] = ch;
                    Barricade(ch, x, y);
                    counter++;
                }                
            }
        }
      /*  void Border()
        {
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    Console.Write(Walls[i, j]);
                  
                }
                Console.WriteLine();
            }
        }
        */
        
        public bool IsBarricade(Point point)
        {
            if (Walls[point.Top, point.Left] == Map.BARRICADE || Walls[point.Top, point.Left] == Map.WALL)
            {
                return true;
            }
            else if (Walls[point.Top, point.Left] == Map.BONUS)
            {
                Bonus.Count++;

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

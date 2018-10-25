using System;

namespace Task_2
{
    class Program
    {
        static void Main(string[] args)
        {
            /*
             * 
            Используя Visual Studio, создайте проект по шаблону Console Application.  
            Используя циклы и метод: 
            Console.Write("*"), Console.Write(" "), Console.Write("\n") (для перехода на новую строку).  
 
            Выведите на экран: 
            · прямоугольник 
            · прямоугольный треугольник  
            · равносторонний треугольник   
            · ромб 

             */
            int i, j; int border_1 = 5, border_2 = 9;

            for (i=0; i<border_1; i++) {

                for (j=0; j<border_2; j++) {

                    if (i == 0 || i == border_1-1)
                    {

                        Console.Write("*");

                    }
                    else if (j == 0 || j == border_2-1)
                    {
                        Console.Write("*");
                    }
                    else {
                        Console.Write(" ");
                    }

                }

                Console.Write("\n");

            }

            Console.ReadKey();

            
        }
    }
}

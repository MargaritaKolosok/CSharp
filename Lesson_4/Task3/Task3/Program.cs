using System;

namespace Task3
{
    class Program
    {
        static void Main(string[] args)
        {
            /*Используя Visual Studio, создайте проект по шаблону Console Application.  
                Напишите программу определения, попадает ли указанное пользователем число от 0 до 100
                в числовой промежуток [0 - 14] [15 - 35] [36 - 50][50 - 100].
                Если да, то укажите, в какой именно промежуток.
                Если пользователь указывает число, не входящее ни в один из имеющихся числовых промежутков,
                то выводится соответствующее сообщение. 
            */


            int num;
            string userEntered="", result;

            while (userEntered!="exit") { 
           
            
           Console.WriteLine("Enter any number from 0 to 100 to check interval");
            
           userEntered = Console.ReadLine();

           bool isNumber = Int32.TryParse(userEntered, out num);

            if (!isNumber)
            {
                result = "Not a number entered. Try again";
            }
            else if (num > 0 && num < 14)
            {

                result = "[0 - 14]";

            } else if (num > 15 && num < 35)
            {

                result = "[15 - 35]";

            } else if (num > 36 && num < 50)
            {
                result = "[36 - 50]";
            } else if (num > 51 && num < 100)
            {
                result = "[50 - 100]";
            } else
            {
                result = "Number is not in diapason of 1 - 100";
            }


            Console.WriteLine(result);
            

            }
            

        }
    }
}

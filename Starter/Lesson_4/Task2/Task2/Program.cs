using System;

namespace Task2
{
    class Program
    {
        static void Main(string[] args)
        {

            double operand1 = 5, operand2 = 3;
            double result = 0;

            string sign = "";
            while (sign!="exit") { 


            Console.WriteLine("Please write down sign");

            sign = Console.ReadLine();

            switch (sign) {
                case "-":
                    {
                        result = operand1 - operand2;
                        break;
                    }
                case "+":
                    {
                        result = operand1 + operand2;
                        break;

                    }
                case "*":
                    {
                        result = operand1 * operand2;
                        break;

                    }
                case "/":
                    {
                        if (operand2 != 0)
                        {
                            result = operand1 / operand2;
                        }
                        else {
                            Console.WriteLine("Unallowed value");

                        }
                        break;

                    }
                default:
                    {

                        Console.WriteLine("There is no such sign. Try again");
                        break;

                    }

                    

            }
            Console.WriteLine("Result = {0}", result);
           

        }
      }
    }
}

using System;

namespace Task_1
{
    class Program
    {
        static void Main(string[] args)
        {
            /*
             Используя Visual Studio, создайте проект по шаблону Console Application.  
        Создайте четыре метода для выполнения арифметических операций, с именами:
        Add – сложение, Sub – вычитание, Mul – умножение, Div – деление.
        Каждый метод должен принимать два целочисленных аргумента и выводить на экран результат выполнения арифметической
        операции соответствующей имени метода. Метод деления Div, должен выполнять проверку попытки деления на ноль.  
        Требуется предоставить пользователю возможность вводить с клавиатуры значения операндов и знак арифметической операции, для выполнения вычислений. 
                    
             */

            int value_1, value_2; var result = 0;
            string @operator;

            Console.Write("Value1 = ");
            value_1 = Int32.Parse(Console.ReadLine());

            Console.Write("Operator:");


            @operator = Convert.ToString(Console.ReadLine());

            Console.Write("Value2 = ");
            value_2 = Int32.Parse(Console.ReadLine());

            switch (@operator)
            {
                case "+":
                    {
                        result = Add(value_1, value_2);
                        break;
                    }
                case "-":
                    {

                        result = Sub(value_1, value_2);
                        break;

                    }
                case "*":
                    {
                        result = Mul(value_1, value_2);
                        break;
                    }
                case "/":
                    {
                        result = Div(value_1, value_2);
                        break;
                    }
            }




            int Add(int operand_1, int operand_2)
            {

                return operand_1 + operand_2;

            }

            int Sub(int operand_1, int operand_2)
            {
                return operand_1 - operand_2;
            }

            int Mul(int operand_1, int operand_2)
            {
                return operand_1 * operand_2;
            }

            int Div(int operand_1, int operand_2)
            {
                if (operand_2 != 0)
                {

                    return operand_1 / operand_2;

                }
                else
                {
                    return -1; //
                }

            }



            Console.WriteLine("Result = {0}", result);
            Console.ReadKey();



        }
    }
}

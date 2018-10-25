using System;

namespace Task4
{
    class Program
    {
        static void Main(string[] args)
        {
            /*Объем V цилиндра радиусом – R и высотой – h, вычисляется по формуле: V = πR 2 h 
 Площадь S поверхности цилиндра вычисляется по формуле: S = 2πR 2 + 2πR 2 = 2πR(R+h) 

            
 */

            double V, h = 12.767236712, R = 3.23232, S1, S2;
            const double pi = Math.PI;

            V = pi * Math.Pow(pi, 2) * h;
            S1 = 2 * pi * R*h + 2 * pi * Math.Pow(R, 2);
            S2 = 2 * pi * R * (R+h);

            Console.WriteLine("V = {0}, S1 = {1}, S2 = {2} ", V, S1, S2);
            Console.ReadKey();

        }
    }
}

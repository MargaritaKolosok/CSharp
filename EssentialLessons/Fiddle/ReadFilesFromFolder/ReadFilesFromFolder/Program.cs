using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

    class X
    {
    static string exePath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
    static string LevelsFolder = System.IO.Path.Combine(exePath, "../../levels");

    public static string[] myFiles;

    public static void GetAllFiles()
    {
        myFiles = Directory.GetFiles(LevelsFolder);
        
      //  for (int i=0; i< myFiles.Length; i++)
      //  {
       //     Console.WriteLine(myFiles[i]);
       // }
    }
    }

    class Program
    {
        static void Main(string[] args)
        {
        //X x = new X();
        X.GetAllFiles();
        Console.WriteLine(X.myFiles[1]);
           

            Console.ReadKey();
        }
    
}

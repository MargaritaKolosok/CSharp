using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class Stack
{
    private int topNum;
    private int[] stack;

    public Stack(int num)
    {
        topNum = 0;
        stack = new int[num];
    }

    public void Push(int x)
    {
        if (topNum == stack.Length)
        {
            Console.WriteLine("Stack overloaded");
            return;            
        }
        stack[topNum] = x;
        topNum++;
    }
    public int Pop()
    {        
        if (topNum == 0)
        {
            Console.WriteLine("Stack position at 0");
            return (int) 0;
        }
        topNum--;
        return stack[topNum];      
        }
}
namespace _4_Stack
{
    class Program
    {
        static void Main(string[] args)
        {
            Stack myStack = new Stack(6);          
            myStack.Push(1);
            myStack.Push(2);
            myStack.Push(3);
            myStack.Push(2);
            myStack.Push(3);
            Console.WriteLine(myStack.Pop());
            Console.WriteLine(myStack.Pop());
            Console.WriteLine(myStack.Pop());
            Console.WriteLine(myStack.Pop());
            Console.WriteLine(myStack.Pop());
            Console.WriteLine(myStack.Pop());
            Console.WriteLine(myStack.Pop());
            Console.ReadKey();
        }
    }
}

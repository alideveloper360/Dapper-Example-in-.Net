using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Static_Shared_Memory_Demo
{
    class MyClass
    {
        public static int a;
        public int b;
        
        public void fun(int ok)
        {
            a = ok;
        }

        public void print()
        {
            Console.WriteLine($"Static: {a}");
            Console.WriteLine($"Non Static: {b}");
        }
    }

    class Program
    {

        static void Main(string[] args)
        {
            Console.WriteLine("Object 1 Printing");
            MyClass p = new MyClass();
            p.fun(5);
            p.print();


            Console.WriteLine("---------------2nd Object-----------------");
            MyClass p1 = new MyClass();
            p1.print();
            p1.fun(10);
            p1.print();

            Console.WriteLine("Againt printing object 1");
            p.print();

            Console.ReadLine();
        }
    }
}

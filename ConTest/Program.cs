using System;
using System.Text.RegularExpressions;

namespace ConTest
{
    class Program
    {
        static void Main(string[] args)
        {

            var a = Regex.Escape(' '.ToString());
            Console.WriteLine("Hello World!");
        }
    }
}

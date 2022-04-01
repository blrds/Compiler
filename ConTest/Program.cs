using System;
using System.Text.RegularExpressions;

namespace ConTest
{
    class Program
    {
        static void Main(string[] args)
        {

            var a=Console.ReadLine();
            var r = new Regex("\\bint\\b");
            var b = a.Split("\\b");

        }
    }
}

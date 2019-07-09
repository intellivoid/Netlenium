using NetleniumServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Netlenium_Server
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Netlenium Server";
            Console.WriteLine("Written by Zi Xing Narrakas");
            Console.WriteLine();
            
            Console.WriteLine(WebService.Start(8080));

            Console.ReadLine();
            Environment.Exit(0);

        }
    }
}

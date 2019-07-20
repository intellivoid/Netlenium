using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebAPI_Test
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "amd64 and intel i386 should fucking die";
            var Driver = new Netlenium.Driver.Opera.Driver();
            Driver.DriverManager.Initialize();

            Console.WriteLine("Done");
            Console.ReadLine();
        }
    }
}

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
            var Driver = new Netlenium.Driver.Firefox.Driver();
            Driver.TargetPlatform = Netlenium.Driver.Platform.Linux64;
            Driver.DriverManager.Initalize();

            Console.WriteLine("Done");
            Console.ReadLine();
        }
    }
}

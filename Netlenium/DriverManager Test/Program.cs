using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriverManager_Test
{
    class Program
    {
        static void Main(string[] args)
        {
            var Netlenium = new Netlenium.Driver.Chrome.Driver();
            Netlenium.DriverManager.Initalize();

            Console.ReadLine();
        }
    }
}

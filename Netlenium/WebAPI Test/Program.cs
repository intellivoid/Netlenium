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
            var Response = Netlenium.WebAPI.GitHub.Releases.GetReleases("mozilla", "geckodriver");
            foreach(Netlenium.WebAPI.GitHub.Release release in Response)
            {
                Console.WriteLine(release.URL.ToString());
            }

            Console.WriteLine("Done");
            Console.ReadLine();
        }
    }
}

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
            var Response = Netlenium.WebAPI.GitHub.Releases.GetLatestRelease("mozilla", "geckodriver");
            foreach(Netlenium.WebAPI.GitHub.Asset asset in Response.Assets)
            {
                Console.WriteLine(Response.TagName);
                Console.WriteLine(asset.Size);
                Console.WriteLine(asset.Name);
                Console.WriteLine(asset.Size);
                Console.WriteLine(asset.DownloadCount);
                Console.WriteLine(asset.URL.ToString());
                Console.WriteLine(asset.BrowserDownloadURL.ToString());
                Console.WriteLine();
            }

            Console.WriteLine("Done");
            Console.ReadLine();
        }
    }
}

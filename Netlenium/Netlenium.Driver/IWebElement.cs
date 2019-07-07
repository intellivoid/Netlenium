using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Netlenium.Driver
{
    public interface IWebElement
    {
        /// <summary>
        /// Simulates typing text into the element
        /// </summary>
        /// <param name="input"></param>
        void SendKeys(string input);
    }
}

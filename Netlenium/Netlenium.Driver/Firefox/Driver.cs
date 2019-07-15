using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Netlenium.Logging;

namespace Netlenium.Driver.Firefox
{
    public class Driver : IDriver
    {
        public Service Logging => throw new NotImplementedException();

        public IDriverManager DriverManager => throw new NotImplementedException();

        public Browser TargetBrowser => throw new NotImplementedException();

        public Platform TargetPlatform { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public IActions Actions => throw new NotImplementedException();

        public IDocument Document => throw new NotImplementedException();

        public IProxy ProxyConfiguration => throw new NotImplementedException();

        public bool Headless { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool DriverLoggingEnabled { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool DriverVerboseLoggingEnabled { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public void Start()
        {
            throw new NotImplementedException();
        }

        public void Stop()
        {
            throw new NotImplementedException();
        }
    }
}

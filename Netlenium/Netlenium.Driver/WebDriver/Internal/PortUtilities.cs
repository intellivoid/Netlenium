using System.Net;
using System.Net.Sockets;

namespace Netlenium.Driver.WebDriver.Internal
{
    /// <summary>
    /// Encapsulates methods for working with ports.
    /// </summary>
    internal static class PortUtilities
    {
        /// <summary>
        /// Finds a random, free port to be listened on.
        /// </summary>
        /// <returns>A random, free port to be listened on.</returns>
        public static int FindFreePort()
        {
            // Locate a free port on the local machine by binding a socket to
            // an IPEndPoint using IPAddress.Any and port 0. The socket will
            // select a free port.
            var listeningPort = 0;
            var portSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                var socketEndPoint = new IPEndPoint(IPAddress.Any, 0);
                portSocket.Bind(socketEndPoint);
                socketEndPoint = (IPEndPoint)portSocket.LocalEndPoint;
                listeningPort = socketEndPoint.Port;
            }
            finally
            {
                portSocket.Close();
            }

            return listeningPort;
        }
    }
}

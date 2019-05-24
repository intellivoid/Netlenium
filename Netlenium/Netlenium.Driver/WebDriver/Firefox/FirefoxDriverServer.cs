using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using Netlenium.Driver.WebDriver.Firefox.Internal;
using Netlenium.Driver.WebDriver.Remote;

namespace Netlenium.Driver.WebDriver.Firefox
{
    /// <summary>
    /// Provides methods for launching Firefox with the WebDriver extension installed.
    /// </summary>
    public class FirefoxDriverServer : ICommandServer
    {
        private string host;
        private List<IPEndPoint> addresses = new List<IPEndPoint>();
        private FirefoxProfile profile;
        private FirefoxBinary process;
        private Uri extensionUri;

        /// <summary>
        /// Initializes a new instance of the <see cref="FirefoxDriverServer"/> class.
        /// </summary>
        /// <param name="binary">The <see cref="FirefoxBinary"/> on which to make the connection.</param>
        /// <param name="profile">The <see cref="FirefoxProfile"/> creating the connection.</param>
        /// <param name="host">The name of the host on which to connect to the Firefox extension (usually "localhost").</param>
        public FirefoxDriverServer(FirefoxBinary binary, FirefoxProfile profile, string host)
        {
            this.host = host;
            if (profile == null)
            {
                this.profile = new FirefoxProfile();
            }
            else
            {
                this.profile = profile;
            }

            if (binary == null)
            {
                process = new FirefoxBinary();
            }
            else
            {
                process = binary;
            }
        }

        /// <summary>
        /// Gets the <see cref="Uri"/> for communicating with this server.
        /// </summary>
        public Uri ExtensionUri
        {
            get { return extensionUri; }
        }

        /// <summary>
        /// Starts the server.
        /// </summary>
        public void Start()
        {
            using (ILock lockObject = new SocketLock(profile.Port - 1))
            {
                lockObject.LockObject(process.Timeout);
                try
                {
                    var portToUse = DetermineNextFreePort(host, profile.Port);
                    profile.Port = portToUse;
                    profile.WriteToDisk();
                    process.StartProfile(profile, new string[] { "-foreground" });

                    SetAddress(portToUse);

                    // TODO (JimEvans): Get a better url algorithm.
                    extensionUri = new Uri(string.Format(CultureInfo.InvariantCulture, "http://{0}:{1}/hub/", host, portToUse));
                    ConnectToBrowser(process.Timeout);
                }
                finally
                {
                    lockObject.UnlockObject();
                }
            }
        }

        /// <summary>
        /// Releases all resources used by the <see cref="FirefoxDriverServer"/>.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="FirefoxDriverServer"/> and optionally
        /// releases the managed resources.
        /// </summary>
        /// <param name="disposing"><see langword="true"/> to release managed and resources;
        /// <see langword="false"/> to only release unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // This should only be called after the QUIT command has been sent,
                // so go ahead and clean up our process and profile.
                process.Dispose();
                profile.Clean();
            }
        }

        private static int DetermineNextFreePort(string host, int port)
        {
            // Attempt to connect to the given port on the host
            // If we can't connect, then we're good to use it
            int newPort;

            for (newPort = port; newPort < port + 200; newPort++)
            {
                using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
                {
                    socket.ExclusiveAddressUse = true;
                    var hostEntry = Dns.GetHostEntry(host);

                    // Use the first IPv4 address that we find
                    var endPointAddress = IPAddress.Parse("127.0.0.1");
                    foreach (var ip in hostEntry.AddressList)
                    {
                        if (ip.AddressFamily == AddressFamily.InterNetwork)
                        {
                            endPointAddress = ip;
                            break;
                        }
                    }

                    var address = new IPEndPoint(endPointAddress, newPort);

                    try
                    {
                        socket.Bind(address);
                        return newPort;
                    }
                    catch (SocketException)
                    {
                        // Port is already bound. Skip it and continue
                    }
                }
            }

            throw new WebDriverException(string.Format(CultureInfo.InvariantCulture, "Cannot find free port in the range {0} to {1} ", port, newPort));
        }

        private static List<IPEndPoint> ObtainLoopbackAddresses(int port)
        {
            var endpoints = new List<IPEndPoint>();

            // Obtain a reference to all network interfaces in the machine
            var adapters = NetworkInterface.GetAllNetworkInterfaces();
            foreach (var adapter in adapters)
            {
                var properties = adapter.GetIPProperties();
                foreach (IPAddressInformation uniCast in properties.UnicastAddresses)
                {
                    if (uniCast.Address.AddressFamily == AddressFamily.InterNetwork && IPAddress.IsLoopback(uniCast.Address))
                    {
                        endpoints.Add(new IPEndPoint(uniCast.Address, port));
                    }
                }
            }

            return endpoints;
        }

        private static bool IsSocketConnected(Socket extensionSocket)
        {
            return extensionSocket != null && extensionSocket.Connected;
        }

        private void SetAddress(int port)
        {
            if (string.Compare("localhost", host, StringComparison.OrdinalIgnoreCase) == 0)
            {
                addresses = ObtainLoopbackAddresses(port);
            }
            else
            {
                var hostEntry = Dns.GetHostEntry(host);

                // Use the first IPv4 address that we find
                var endPointAddress = IPAddress.Parse("127.0.0.1");
                foreach (var ip in hostEntry.AddressList)
                {
                    if (ip.AddressFamily == AddressFamily.InterNetwork)
                    {
                        endPointAddress = ip;
                        break;
                    }
                }

                var hostEndPoint = new IPEndPoint(endPointAddress, port);
                addresses.Add(hostEndPoint);
            }

            if (addresses.Count == 0)
            {
                throw new WebDriverException(string.Format(CultureInfo.InvariantCulture, "Could not find any IPv4 addresses for host '{0}'", host));
            }
        }

        private void ConnectToBrowser(TimeSpan timeToWait)
        {
            // Attempt to connect to the browser extension on a Socket.
            // A successful connection means the browser is running and
            // the extension has been properly initialized.
            Socket extensionSocket = null;
            var waitUntil = DateTime.Now.AddMilliseconds(timeToWait.TotalMilliseconds);
            try
            {
                while (!IsSocketConnected(extensionSocket) && waitUntil > DateTime.Now)
                {
                    foreach (var addr in addresses)
                    {
                        try
                        {
                            extensionSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                            extensionSocket.Connect(addr);
                            break;
                        }
                        catch (SocketException)
                        {
                            System.Threading.Thread.Sleep(250);
                        }
                    }
                }

                // If the socket was either not created or not connected successfully,
                // throw an exception.
                if (!IsSocketConnected(extensionSocket))
                {
                    if (extensionSocket == null || extensionSocket.RemoteEndPoint == null)
                    {
                        var addressBuilder = new StringBuilder();
                        foreach (var address in addresses)
                        {
                            if (addressBuilder.Length > 0)
                            {
                                addressBuilder.Append(", ");
                            }

                            addressBuilder.AppendFormat(CultureInfo.InvariantCulture, "{0}:{1}", address.Address.ToString(), address.Port.ToString(CultureInfo.InvariantCulture));
                        }

                        throw new WebDriverException(string.Format(CultureInfo.InvariantCulture, "Failed to start up socket within {0} milliseconds. Attempted to connect to the following addresses: {1}", timeToWait.TotalMilliseconds, addressBuilder.ToString()));
                    }
                    else
                    {
                        var endPoint = (IPEndPoint)extensionSocket.RemoteEndPoint;
                        var formattedError = string.Format(CultureInfo.InvariantCulture, "Unable to connect to host {0} on port {1} after {2} milliseconds", endPoint.Address.ToString(), endPoint.Port.ToString(CultureInfo.InvariantCulture), timeToWait.TotalMilliseconds);
                        throw new WebDriverException(formattedError);
                    }
                }
            }
            finally
            {
                extensionSocket.Close();
            }
        }
    }
}

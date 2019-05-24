using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Netlenium.Driver.WebDriver.Firefox.Internal
{
    /// <summary>
    /// Provides a mutex-like lock on a socket.
    /// </summary>
    internal class SocketLock : ILock
    {
        private static int delayBetweenSocketChecks = 100;

        private int lockPort;
        private Socket lockSocket;

        /// <summary>
        /// Initializes a new instance of the <see cref="SocketLock"/> class.
        /// </summary>
        /// <param name="lockPort">Port to use to acquire the lock.</param>
        /// <remarks>The <see cref="SocketLock"/> class will attempt to acquire the
        /// specified port number, and wait for it to become free.</remarks>
        public SocketLock(int lockPort)
        {
            this.lockPort = lockPort;
            lockSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            PreventSocketInheritance();
        }

        /// <summary>
        /// Locks the mutex port.
        /// </summary>
        /// <param name="timeout">The <see cref="TimeSpan"/> describing the amount of time to wait for
        /// the mutex port to become available.</param>
        public void LockObject(TimeSpan timeout)
        {
            var hostEntry = Dns.GetHostEntry("localhost");

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

            var address = new IPEndPoint(endPointAddress, lockPort);

            // Calculate the 'exit time' for our wait loop.
            var maxWait = DateTime.Now.Add(timeout);

            // Attempt to acquire the lock until something goes wrong or we run out of time.
            do
            {
                try
                {
                    if (IsLockFree(address))
                    {
                        return;
                    }

                    Thread.Sleep(delayBetweenSocketChecks);
                }
                catch (ThreadInterruptedException e)
                {
                    throw new WebDriverException("the thread was interrupted", e);
                }
                catch (IOException e)
                {
                    throw new WebDriverException("An unexpected error occurred", e);
                }
            }
            while (DateTime.Now < maxWait);

            throw new WebDriverException(string.Format(CultureInfo.InvariantCulture, "Unable to bind to locking port {0} within {1} milliseconds", lockPort, timeout.TotalMilliseconds));
        }

        /// <summary>
        /// Unlocks the mutex port.
        /// </summary>
        public void UnlockObject()
        {
            try
            {
                lockSocket.Close();
            }
            catch (IOException e)
            {
                throw new WebDriverException("An error occured unlocking the object", e);
            }
        }

        /// <summary>
        /// Releases all resources associated with this <see cref="SocketLock"/>
        /// </summary>
        public void Dispose()
        {
            if (lockSocket != null && lockSocket.Connected)
            {
                lockSocket.Close();
            }

            GC.SuppressFinalize(this);
        }

        private bool IsLockFree(IPEndPoint address)
        {
            try
            {
                lockSocket.Bind(address);
                return true;
            }
            catch (SocketException)
            {
                return false;
            }
        }

        private void PreventSocketInheritance()
        {
            // TODO (JimEvans): Handle the non-Windows case.
            if (Platform.CurrentPlatform.IsPlatformType(PlatformType.Windows))
            {
                NativeMethods.SetHandleInformation(lockSocket.Handle, NativeMethods.HandleInformation.Inherit | NativeMethods.HandleInformation.ProtectFromClose, NativeMethods.HandleInformation.None);
            }
        }
    }
}

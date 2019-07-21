using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Netlenium.Logging;

namespace Netlenium.Intellivoid
{
    public class HttpServer : IDisposable
    {

        private bool disposed;
        private TcpListener listener;
        private readonly object syncLock = new object();
        private readonly Dictionary<HttpClient, bool> clients = new Dictionary<HttpClient, bool>();
        private HttpServerState state = HttpServerState.Stopped;
        private AutoResetEvent clientsChangedEvent = new AutoResetEvent(false);

        public HttpServerState State
        {
            get => state;
            private set
            {
                if (state != value)
                {
                    state = value;

                    OnStateChanged(EventArgs.Empty);
                }
            }
        }

        public event HttpRequestEventHandler RequestReceived;

        protected virtual void OnRequestReceived(HttpRequestEventArgs e)
        {
            var ev = RequestReceived;

            if (ev != null)
                ev(this, e);
        }

        public event HttpExceptionEventHandler UnhandledException;

        protected virtual void OnUnhandledException(HttpExceptionEventArgs e)
        {
            var ev = UnhandledException;

            if (ev != null)
                ev(this, e);
        }

        public event EventHandler StateChanged;

        protected virtual void OnStateChanged(EventArgs e)
        {
            var ev = StateChanged;

            if (ev != null)
                ev(this, e);
        }

        public IPEndPoint EndPoint { get; set; }

        public int ReadBufferSize { get; set; }

        public int WriteBufferSize { get; set; }

        public string ServerBanner { get; set; }

        public TimeSpan ReadTimeout { get; set; }

        public TimeSpan WriteTimeout { get; set; }

        public TimeSpan ShutdownTimeout { get; set; }

        internal HttpServerUtility ServerUtility { get; private set; }

        internal HttpTimeoutManager TimeoutManager { get; private set; }

        public HttpServer()
        {

            EndPoint = new IPEndPoint(IPAddress.Loopback, 0);

            ReadBufferSize = 4096;
            WriteBufferSize = 4096;
            ShutdownTimeout = TimeSpan.FromSeconds(30);
            ReadTimeout = TimeSpan.FromSeconds(30);
            WriteTimeout = TimeSpan.FromSeconds(90);

            ServerBanner = String.Format("Netlenium Framework/{0}", GetType().Assembly.GetName().Version);
        }

        public void Start()
        {
            VerifyState(HttpServerState.Stopped);

            State = HttpServerState.Starting;

            WebService.Logging.WriteEntry(MessageType.Information, "HyperWS", $"Starting HTTP server at {EndPoint}");

            TimeoutManager = new HttpTimeoutManager(this);

            // Start the listener.

            var listener = new TcpListener(EndPoint);

            try
            {
                listener.Start();

                EndPoint = (IPEndPoint)listener.LocalEndpoint;

                this.listener = listener;

                ServerUtility = new HttpServerUtility();

                WebService.Logging.WriteEntry(MessageType.Information, "HyperWS", String.Format("HTTP server running at {0}", EndPoint));
            }
            catch (Exception ex)
            {
                State = HttpServerState.Stopped;
                WebService.Logging.WriteEntry(MessageType.Error, "HyperWS", $"Failed to start HTTP server, {ex.Message}");
                throw new WebServerException("Failed to start HTTP server", ex);
            }

            State = HttpServerState.Started;

            BeginAcceptTcpClient();
        }

        public void Stop()
        {
            VerifyState(HttpServerState.Started);
            WebService.Logging.WriteEntry(MessageType.Information, "HyperWS", "Stopping HTTP Server");
            
            State = HttpServerState.Stopping;

            try
            {
                // Prevent any new connections.

                listener.Stop();

                // Wait for all clients to complete.

                StopClients();
            }
            catch (Exception ex)
            {
                WebService.Logging.WriteEntry(MessageType.Error, "HyperWS", $"Failed to stop HTTP server; {ex}");
                throw new WebServerException("Failed to stop HTTP server", ex);
            }
            finally
            {
                listener = null;

                State = HttpServerState.Stopped;
                WebService.Logging.WriteEntry(MessageType.Information, "HyperWS", "Stopped HTTP server");
            }
        }

        private void StopClients()
        {
            var shutdownStarted = DateTime.Now;
            var forceShutdown = false;

            // Clients that are waiting for new requests are closed.

            List<HttpClient> clients;

            lock (syncLock)
            {
                clients = new List<HttpClient>(this.clients.Keys);
            }

            foreach (var client in clients)
            {
                client.RequestClose();
            }

            // First give all clients a chance to complete their running requests.

            while (true)
            {
                lock (syncLock)
                {
                    if (this.clients.Count == 0)
                        break;
                }

                var shutdownRunning = DateTime.Now - shutdownStarted;

                if (shutdownRunning >= ShutdownTimeout)
                {
                    forceShutdown = true;
                    break;
                }

                clientsChangedEvent.WaitOne(ShutdownTimeout - shutdownRunning);
            }

            if (!forceShutdown)
                return;

            // If there are still clients running after the timeout, their
            // connections will be forcibly closed.

            lock (syncLock)
            {
                clients = new List<HttpClient>(this.clients.Keys);
            }

            foreach (var client in clients)
            {
                client.ForceClose();
            }

            // Wait for the registered clients to be cleared.

            while (true)
            {
                lock (syncLock)
                {
                    if (this.clients.Count == 0)
                        break;
                }

                clientsChangedEvent.WaitOne();
            }
        }

        private void BeginAcceptTcpClient()
        {
            var listener = this.listener;
            if (listener != null)
                listener.BeginAcceptTcpClient(AcceptTcpClientCallback, null);
        }

        private void AcceptTcpClientCallback(IAsyncResult asyncResult)
        {
            try
            {
                var listener = this.listener; // Prevent race condition.

                if (listener == null)
                    return;

                var tcpClient = listener.EndAcceptTcpClient(asyncResult);

                // If we've stopped already, close the TCP client now.

                if (state != HttpServerState.Started)
                {
                    tcpClient.Close();
                    return;
                }

                var client = new HttpClient(this, tcpClient);

                RegisterClient(client);

                client.BeginRequest();

                BeginAcceptTcpClient();
            }
            catch (ObjectDisposedException)
            {
                // EndAcceptTcpClient will throw a ObjectDisposedException
                // when we're shutting down. This can safely be ignored.
            }
            catch (Exception ex)
            {
                WebService.Logging.WriteEntry(MessageType.Information, "HyperWS", $"Failed to accept TCP client {ex}");
            }
        }

        private void RegisterClient(HttpClient client)
        {
            if (client == null)
                throw new ArgumentNullException("client");

            lock (syncLock)
            {
                clients.Add(client, true);

                clientsChangedEvent.Set();
            }
        }

        internal void UnregisterClient(HttpClient client)
        {
            try
            {
                if (client == null)
                    throw new ArgumentNullException("client");

                lock (syncLock)
                {

                    clients.Remove(client);

                    clientsChangedEvent.Set();
                }
            }
            catch(Exception exception)
            {
                WebService.Logging.WriteEntry(MessageType.Warning, "HyperWS", $"Cannot unregister client; {exception.Message}");
            }
        }

        private void VerifyState(HttpServerState state)
        {
            if (disposed)
                throw new ObjectDisposedException(GetType().Name);
            if (this.state != state)
                throw new InvalidOperationException(String.Format("Expected server to be in the '{0}' state", state));
        }

        public void Dispose()
        {
            if (!disposed)
            {
                if (state == HttpServerState.Started)
                    Stop();

                if (clientsChangedEvent != null)
                {
                    ((IDisposable)clientsChangedEvent).Dispose();
                    clientsChangedEvent = null;
                }

                if (TimeoutManager != null)
                {
                    TimeoutManager.Dispose();
                    TimeoutManager = null;
                }

                disposed = true;
            }
        }

        internal void RaiseRequest(HttpContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            OnRequestReceived(new HttpRequestEventArgs(context));
        }

        internal bool RaiseUnhandledException(HttpContext context, Exception exception)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            var e = new HttpExceptionEventArgs(context, exception);

            OnUnhandledException(e);

            return e.Handled;
        }
    }
}

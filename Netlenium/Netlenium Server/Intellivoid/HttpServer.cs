using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Intellivoid.HyperWS
{
    public class HttpServer : IDisposable
    {

        private bool _disposed;
        private TcpListener _listener;
        private readonly object _syncLock = new object();
        private readonly Dictionary<HttpClient, bool> _clients = new Dictionary<HttpClient, bool>();
        private HttpServerState _state = HttpServerState.Stopped;
        private AutoResetEvent _clientsChangedEvent = new AutoResetEvent(false);

        public HttpServerState State
        {
            get { return _state; }
            private set
            {
                if (_state != value)
                {
                    _state = value;

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

            NetleniumServer.WebService.logging.WriteEntry(Netlenium.Logging.MessageType.Information, "HyperWS", $"Starting HTTP server at {EndPoint}");

            TimeoutManager = new HttpTimeoutManager(this);

            // Start the listener.

            var listener = new TcpListener(EndPoint);

            try
            {
                listener.Start();

                EndPoint = (IPEndPoint)listener.LocalEndpoint;

                _listener = listener;

                ServerUtility = new HttpServerUtility();

                NetleniumServer.WebService.logging.WriteEntry(Netlenium.Logging.MessageType.Information, "HyperWS", String.Format("HTTP server running at {0}", EndPoint));
            }
            catch (Exception ex)
            {
                State = HttpServerState.Stopped;
                NetleniumServer.WebService.logging.WriteEntry(Netlenium.Logging.MessageType.Error, "HyperWS", $"Failed to start HTTP server, {ex.Message}");
                throw new WebServerException("Failed to start HTTP server", ex);
            }

            State = HttpServerState.Started;

            BeginAcceptTcpClient();
        }

        public void Stop()
        {
            VerifyState(HttpServerState.Started);
            NetleniumServer.WebService.logging.WriteEntry(Netlenium.Logging.MessageType.Information, "HyperWS", "Stopping HTTP Server");
            
            State = HttpServerState.Stopping;

            try
            {
                // Prevent any new connections.

                _listener.Stop();

                // Wait for all clients to complete.

                StopClients();
            }
            catch (Exception ex)
            {
                NetleniumServer.WebService.logging.WriteEntry(Netlenium.Logging.MessageType.Error, "HyperWS", $"Failed to stop HTTP server; {ex}");
                throw new WebServerException("Failed to stop HTTP server", ex);
            }
            finally
            {
                _listener = null;

                State = HttpServerState.Stopped;
                NetleniumServer.WebService.logging.WriteEntry(Netlenium.Logging.MessageType.Information, "HyperWS", "Stopped HTTP server");
            }
        }

        private void StopClients()
        {
            var shutdownStarted = DateTime.Now;
            bool forceShutdown = false;

            // Clients that are waiting for new requests are closed.

            List<HttpClient> clients;

            lock (_syncLock)
            {
                clients = new List<HttpClient>(_clients.Keys);
            }

            foreach (var client in clients)
            {
                client.RequestClose();
            }

            // First give all clients a chance to complete their running requests.

            while (true)
            {
                lock (_syncLock)
                {
                    if (_clients.Count == 0)
                        break;
                }

                var shutdownRunning = DateTime.Now - shutdownStarted;

                if (shutdownRunning >= ShutdownTimeout)
                {
                    forceShutdown = true;
                    break;
                }

                _clientsChangedEvent.WaitOne(ShutdownTimeout - shutdownRunning);
            }

            if (!forceShutdown)
                return;

            // If there are still clients running after the timeout, their
            // connections will be forcibly closed.

            lock (_syncLock)
            {
                clients = new List<HttpClient>(_clients.Keys);
            }

            foreach (var client in clients)
            {
                client.ForceClose();
            }

            // Wait for the registered clients to be cleared.

            while (true)
            {
                lock (_syncLock)
                {
                    if (_clients.Count == 0)
                        break;
                }

                _clientsChangedEvent.WaitOne();
            }
        }

        private void BeginAcceptTcpClient()
        {
            var listener = _listener;
            if (listener != null)
                listener.BeginAcceptTcpClient(AcceptTcpClientCallback, null);
        }

        private void AcceptTcpClientCallback(IAsyncResult asyncResult)
        {
            try
            {
                var listener = _listener; // Prevent race condition.

                if (listener == null)
                    return;

                var tcpClient = listener.EndAcceptTcpClient(asyncResult);

                // If we've stopped already, close the TCP client now.

                if (_state != HttpServerState.Started)
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
                NetleniumServer.WebService.logging.WriteEntry(Netlenium.Logging.MessageType.Information, "HyperWS", $"Failed to accept TCP client {ex}");
            }
        }

        private void RegisterClient(HttpClient client)
        {
            if (client == null)
                throw new ArgumentNullException("client");

            lock (_syncLock)
            {
                _clients.Add(client, true);

                _clientsChangedEvent.Set();
            }
        }

        internal void UnregisterClient(HttpClient client)
        {
            try
            {
                if (client == null)
                    throw new ArgumentNullException("client");

                lock (_syncLock)
                {

                    _clients.Remove(client);

                    _clientsChangedEvent.Set();
                }
            }
            catch(Exception exception)
            {
                NetleniumServer.WebService.logging.WriteEntry(Netlenium.Logging.MessageType.Warning, "HyperWS", $"Cannot unregister client; {exception.Message}");
            }
        }

        private void VerifyState(HttpServerState state)
        {
            if (_disposed)
                throw new ObjectDisposedException(GetType().Name);
            if (_state != state)
                throw new InvalidOperationException(String.Format("Expected server to be in the '{0}' state", state));
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                if (_state == HttpServerState.Started)
                    Stop();

                if (_clientsChangedEvent != null)
                {
                    ((IDisposable)_clientsChangedEvent).Dispose();
                    _clientsChangedEvent = null;
                }

                if (TimeoutManager != null)
                {
                    TimeoutManager.Dispose();
                    TimeoutManager = null;
                }

                _disposed = true;
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

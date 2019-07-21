using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace Netlenium.Intellivoid
{
    internal class HttpTimeoutManager : IDisposable
    {
        private Thread thread;
        private ManualResetEvent closeEvent = new ManualResetEvent(false);

        public TimeoutQueue ReadQueue { get; private set; }
        public TimeoutQueue WriteQueue { get; private set; }

        public HttpTimeoutManager(HttpServer server)
        {
            if (server == null)
                throw new ArgumentNullException(nameof(server));

            ReadQueue = new TimeoutQueue(server.ReadTimeout);
            WriteQueue = new TimeoutQueue(server.WriteTimeout);

            thread = new Thread(ThreadProc);
            thread.Start();
        }

        private void ThreadProc()
        {
            while (!closeEvent.WaitOne(TimeSpan.FromSeconds(1)))
            {
                ProcessQueue(ReadQueue);
                ProcessQueue(WriteQueue);
            }
        }

        private void ProcessQueue(TimeoutQueue queue)
        {
            while (true)
            {
                var item = queue.DequeueExpired();
                if (item == null)
                    return;

                if (!item.AsyncResult.IsCompleted)
                {
                    try
                    {
                        item.Disposable.Dispose();
                    }
                    catch
                    {
                        // Ignore exceptions.
                    }
                }
            }
        }

        public void Dispose()
        {
            if (thread != null)
            {
                closeEvent.Set();
                thread.Join();
                thread = null;
            }
            if (closeEvent != null)
            {
                closeEvent.Close();
                closeEvent = null;
            }
        }

        public class TimeoutQueue
        {
            private readonly object syncRoot = new object();
            private readonly Stopwatch stopwatch = Stopwatch.StartNew();
            private readonly long timeout;
            private readonly Queue<TimeoutItem> items = new Queue<TimeoutItem>();

            public TimeoutQueue(TimeSpan timeout)
            {
                this.timeout = (long)(timeout.TotalSeconds * Stopwatch.Frequency);
            }

            public void Add(IAsyncResult asyncResult, IDisposable disposable)
            {
                if (asyncResult == null)
                    throw new ArgumentNullException(nameof(asyncResult));
                if (disposable == null)
                    throw new ArgumentNullException(nameof(disposable));

                lock (syncRoot)
                {
                    items.Enqueue(new TimeoutItem(stopwatch.ElapsedTicks + timeout, asyncResult, disposable));
                }
            }

            public TimeoutItem DequeueExpired()
            {
                lock (syncRoot)
                {
                    if (items.Count == 0)
                        return null;

                    var item = items.Peek();
                    if (item.Expires < stopwatch.ElapsedTicks)
                        return items.Dequeue();

                    return null;
                }
            }
        }

        public class TimeoutItem
        {
            public long Expires { get; private set; }
            public IAsyncResult AsyncResult { get; private set; }
            public IDisposable Disposable { get; private set; }

            public TimeoutItem(long expires, IAsyncResult asyncResult, IDisposable disposable)
            {
                Expires = expires;
                AsyncResult = asyncResult ?? throw new ArgumentNullException(nameof(asyncResult));
                Disposable = disposable;
            }
        }
    }
}

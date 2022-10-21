using System;
using System.Threading;
using System.Threading.Tasks;

namespace TSkin
{
    public class ThreadOne : IDisposable
    {
        public ThreadOne(Action action, int interval)
        {
            Task.Run(() =>
            {
                while (true)
                {
                    if (token.Token.IsCancellationRequested)
                    {
                        return;
                    }
                    else
                    {
                        action();
                        Thread.Sleep(interval);
                    }
                }
            }).ContinueWith((action =>
            {
                token.Dispose();
                token = null;
            }));
        }
        public ThreadOne(Action action, Action with_action, int interval)
        {
            Task.Run(() =>
            {
                while (true)
                {
                    if (token.Token.IsCancellationRequested)
                    {
                        return;
                    }
                    else
                    {
                        action();
                        Thread.Sleep(interval);
                    }
                }
            }).ContinueWith((action =>
            {
                with_action();
                token.Dispose();
                token = null;
            }));
        }
        public ThreadOne(Action action, Action with_action, Func<bool> can_action, int interval)
        {
            bool isend = true;
            Task.Run(() =>
            {
                while (can_action())
                {
                    if (token == null || token.Token.IsCancellationRequested)
                    {
                        isend = false;
                        return;
                    }
                    else
                    {
                        action();
                        Thread.Sleep(interval);
                    }
                }
            }).ContinueWith((action =>
            {
                if (isend)
                    with_action();
                if (token != null)
                    token.Dispose();
                token = null;
            }));
        }

        public void Cancel()
        {
            if (token != null)
                token.Cancel();
        }

        public void Dispose()
        {
            if (token != null)
            {
                Cancel();
                token.Dispose();
                token = null;
            }
        }

        CancellationTokenSource token = new CancellationTokenSource();
    }
}

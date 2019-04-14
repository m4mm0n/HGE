using System;
using System.ComponentModel;
using System.Threading;

namespace HGE.IO
{
    public class ThreadTimer : IDisposable
    {
        private readonly TimerCallback _timerDelegate;
        private bool _enabled;
        private int _interval = 100; //100ms
        private Timer _timer;
        private bool disposed;

        public ThreadTimer()
        {
            _timerDelegate = timer_Tick;
        }

        public ThreadTimer(int intervalMs)
        {
            _interval = intervalMs;
            _timerDelegate = timer_Tick;
        }

        public int GetCount { get; private set; }

        public int Interval
        {
            get => _interval;
            set
            {
                if (value <= 0)
                    _interval = -1;
                else
                    _interval = value;
                if (Enabled)
                {
                    var timer = _timer;
                    lock (timer)
                    {
                        _timer.Change(_interval, _interval);
                    }
                }
            }
        }

        public bool Enabled
        {
            get => _enabled && _timer != null;
            set
            {
                if (value == _enabled) return;
                if (value)
                {
                    if (_timer != null)
                    {
                        var timer = _timer;
                        lock (timer)
                        {
                            _timer.Change(_interval, _interval);
                            _enabled = true;
                            return;
                        }
                    }

                    Start();
                    return;
                }

                if (_timer != null)
                {
                    var timer = _timer;
                    lock (timer)
                    {
                        _timer.Change(-1, -1);
                        _enabled = false;
                        return;
                    }
                }

                Stop();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public event EventHandler Tick;

        private void Dispose(bool disposing)
        {
            if (!disposed)
                try
                {
                    Stop();
                }
                catch
                {
                }

            disposed = true;
        }

        ~ThreadTimer()
        {
            Dispose(false);
        }

        private void InvokeDelegate(Delegate del, object[] args)
        {
            var synchronizeInvoke = del.Target as ISynchronizeInvoke;
            if (synchronizeInvoke != null)
            {
                if (!synchronizeInvoke.InvokeRequired)
                {
                    del.DynamicInvoke(args);
                    return;
                }

                try
                {
                    synchronizeInvoke.BeginInvoke(del, args);
                    return;
                }
                catch
                {
                    return;
                }
            }

            del.DynamicInvoke(args);
        }

        private void ProcessDelegate(Delegate del, params object[] args)
        {
            if (del == null || _timer == null) return;
            var timer = _timer;
            lock (timer)
            {
                foreach (var del2 in del.GetInvocationList()) InvokeDelegate(del2, args);
            }
        }

        public void Start()
        {
            if (_timer == null)
            {
                _timer = new Timer(_timerDelegate, null, _interval, _interval);
                _enabled = true;
                return;
            }

            var timer = _timer;
            lock (timer)
            {
                _timer.Change(_interval, _interval);
                _enabled = true;
            }
        }

        public void Stop()
        {
            if (_timer != null)
            {
                var timer = _timer;
                lock (timer)
                {
                    _timer.Change(-1, -1);
                    _timer.Dispose();
                    _enabled = false;
                }

                _timer = null;
                return;
            }

            _enabled = false;
        }

        private void timer_Tick(object state)
        {
            GetCount++;
            if (Tick != null) ProcessDelegate(Tick, this, EventArgs.Empty);
        }
    }
}
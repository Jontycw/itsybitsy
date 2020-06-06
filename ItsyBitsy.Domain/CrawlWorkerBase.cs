using System;
using System.Threading;

namespace ItsyBitsy.Domain
{
    public enum WorkerState : byte
    {
        Running = 0,
        Stopped = 1,
        StoppedUnexpectedly = 2,
        Paused = 4
    }

    public abstract class CrawlWorkerBase : ICrawlWorker
    {
        private readonly Thread _workerThread;
        protected readonly bool _separateThread;
        public CrawlWorkerBase(bool separateThread)
        {
            _separateThread = separateThread;
            _workerThread = new Thread(DoWork)
            {
                IsBackground = true
            };
        }

        public event EventHandler WorkerComplete;

        protected virtual void RaiseWorkerComplete()
        {
            WorkerComplete?.Invoke(this, new EventArgs());
        }

        protected abstract void DoWorkInternal();
        protected abstract bool TerminateCondition();
        protected virtual int Delay => 0;

        public void Pause()
        {
            State = WorkerState.Paused;
        }

        public void Resume()
        {
            State = WorkerState.Running;
        }
        public void Stop()
        {
            State = WorkerState.Stopped;
        }

        public WorkerState State { get; private set; } = WorkerState.Stopped;

        private void DoWork()
        {
            State = WorkerState.Running;

            try
            {
                while (!TerminateCondition())
                {
                    if (Delay > 0)
                        Thread.Sleep(Delay);

                    DoWorkInternal();

                    while (State == WorkerState.Paused)
                    {
                        Thread.Sleep(1000);

                        if (State == WorkerState.Stopped)
                            break;
                    }

                    if (State == WorkerState.Stopped)
                        break;
                }
            }
            catch
            {
                State = WorkerState.StoppedUnexpectedly;
            }
            finally
            {
                RaiseWorkerComplete();
            }
        }

        public void Start()
        {
            if (_separateThread)
                _workerThread.Start();
            else
                DoWorkInternal();
        }
    }
}

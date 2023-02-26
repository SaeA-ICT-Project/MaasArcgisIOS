using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace MaaSArcGISIOS.Common
{
    public abstract class EngineThread : Common.IEngine
    {
        private bool mIsRunning = false;
        private CancellationTokenSource _Cts;
        public EngineThread()
        {
        }

        protected abstract void begin();
        protected abstract void loop();
        protected abstract void end();

        public void Start(int pSleep = 1)
        {
            _Cts = new System.Threading.CancellationTokenSource();
            if (mIsRunning == false)
            {
                System.Threading.ThreadPool.QueueUserWorkItem(EngineLoopThread, pSleep);
            }
        }

        public void Stop()
        {
            try
            {
                _Cts.Cancel();
                _Cts.Dispose();
            }
            catch
            {

            }
        }

        private void EngineLoopThread(object state)
        {
            mIsRunning = true;
            int _Sleep = (int)state;
            System.Threading.CancellationToken token = _Cts.Token;
            begin();
            while (!token.IsCancellationRequested)
            {
                loop();
                System.Threading.Thread.Sleep(_Sleep);
            }
            mIsRunning = false;
            end();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace MaaSArcGISIOS.Common
{
    public class RingQueue<T>
    {
        private int Capacity { set; get; }
        private Queue<T> QueueData { set; get; }

        public RingQueue(int pCapacity, Queue<T> pQueueData)
        {
            this.Capacity = pCapacity < 1 ? 1 : pCapacity;
            this.QueueData = pQueueData;
        }

        public void Push(T pInputData)
        {
            try
            {
                if (this.QueueData.Count < this.Capacity)
                {
                    QueueData.Enqueue(pInputData);
                }
                else
                {
                    QueueData.Dequeue();
                    QueueData.Enqueue(pInputData);
                }
            }
            catch
            {

            }
        }

        public bool IsEmpty()
        {
            return QueueData.Count <= 0 ? true : false;
        }

        public T Top()
        {
            try
            {
                if (QueueData.Count < 1)
                {
                    return default(T);
                }
                else
                {
                    return QueueData.Peek();
                }
            }
            catch
            {
                return default(T);
            }
        }

        public T Pop()
        {
            try
            {
                if (QueueData.Count < 1)
                {
                    return default(T);
                }
                else
                {
                    var _TopData = QueueData.Peek();
                    QueueData.Dequeue();

                    return _TopData;
                }
            }
            catch
            {
                return default(T);
            }
        }
    }
}

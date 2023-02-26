using System;
using System.Collections.Generic;
using System.Text;

namespace MaaSArcGISIOS.Common
{
    public class EventArgsManager
    {
        public delegate void EventArgsHandler(object pItem);
        public virtual event EventArgsHandler pEvent;
        public virtual void Publish(object pItem = null)
        {
            pEvent?.Invoke(pItem);
        }

        public virtual void Subscribe(EventArgsHandler pHandler)
        {
            pEvent += pHandler;
        }

        public virtual void UnSubscribe(EventArgsHandler pHandler)
        {
            pEvent -= pHandler;
        }
    }

    public class EventArgsNotifier
    {
        public Dictionary<string, EventArgsManager> pEventNotifier { get; set; }

        public EventArgsNotifier()
        {
            pEventNotifier = new Dictionary<string, EventArgsManager>();
        }

        /// <summary>
        /// Event를 생성합니다.
        /// </summary>
        /// <param name="pEventName">Event 이름. 같은 이름이 있으면 추가되지 않습니다.</param>
        public virtual void SetEventArgs(string pEventName)
        {
            if (pEventNotifier == null)
                return;

            if (!pEventNotifier.ContainsKey(pEventName))
            {
                pEventNotifier.Add(pEventName, new EventArgsManager());
            }
            else
            {
                throw new Exception("이미 같은 이름의 이벤트가 존재합니다.");
            }
        }

        /// <summary>
        /// 이미 선언된 Event에 대해 Event를 송신합니다.(Event Trigger 동작)
        /// </summary>
        /// <param name="pEventName">Event 이름</param>
        /// <param name="pEventItem">송신시 EventItem</param>
        public virtual void CallEventArgs(string pEventName, object pEventItem = null)
        {
            if (pEventNotifier == null)
                return;

            if (pEventNotifier.ContainsKey(pEventName))
            {
                pEventNotifier[pEventName].Publish(pEventItem);
            }
            else
            {
                throw new Exception("해당 이름의 이벤트가 존재하지 않습니다.");
            }
        }

        /// <summary>
        /// 이미 선언된 Event에 대해 Event를 수신합니다. (Event Trigger 구독)
        /// </summary>
        /// <param name="pEventName">Event 이름</param>
        /// <param name="pMethod">수신받아질 Event Method 또는 Action</param>
        public virtual void GetEventArgs(string pEventName, EventArgsManager.EventArgsHandler pMethod)
        {
            if (pEventNotifier == null)
                return;

            if (pEventNotifier.ContainsKey(pEventName))
            {
                pEventNotifier[pEventName].Subscribe(pMethod);
            }
            else
            {
                throw new Exception("해당 이름의 이벤트가 존재하지 않습니다.");
            }
        }

        /// <summary>
        /// 이미 선언된 Event에 대해 Event를 구독취소합니다. (Event Trigger 구독취소)
        /// </summary>
        /// <param name="pEventName">Event 이름</param>
        /// <param name="pMethod">수신받아질 Event Method 또는 Action</param>
        public virtual void RemoveEventArgs(string pEventName, EventArgsManager.EventArgsHandler pMethod)
        {
            if (pEventNotifier == null)
                return;

            if (pEventNotifier.ContainsKey(pEventName))
            {
                pEventNotifier[pEventName].UnSubscribe(pMethod);
            }
            else
            {
                throw new Exception("해당 이름의 이벤트가 존재하지 않습니다.");
            }
        }
    }
}

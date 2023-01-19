using UnityEngine;
using System.Collections.Generic;

namespace T4.Managers
{
    public class EventManager : MonoBehaviour
    {
        private static EventManager _instance;
        public static EventManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType(typeof(EventManager)) as EventManager;

                    if (!_instance)
                    {
                        Debug.LogError($"There needs to be one active {typeof(EventManager)} script on a GameObject in your scene.");
                    }
                }

                return _instance;
            }
        }

        public delegate void EventDelegate<in T>(T e) where T : Events.BaseEvent;
        private delegate void EventDelegate(Events.BaseEvent e);

        private readonly Dictionary<System.Type, EventDelegate> delegates = new();
        private readonly Dictionary<System.Delegate, EventDelegate> delegateLookup = new();

        public void AddListener<T>(EventDelegate<T> del) where T : Events.BaseEvent
        {
            // Early-out if we've already registered this delegate
            if (delegateLookup.ContainsKey(del))
                return;

            // Create a new non-generic delegate which calls our generic one.
            // This is the delegate we actually invoke.
            EventDelegate internalDelegate = (e) => del((T)e);
            delegateLookup[del] = internalDelegate;

            EventDelegate tempDel;
            if (delegates.TryGetValue(typeof(T), out tempDel))
            {
                delegates[typeof(T)] = tempDel += internalDelegate;
            }
            else
            {
                delegates[typeof(T)] = internalDelegate;
            }
        }

        public void RemoveListener<T>(EventDelegate<T> del) where T : Events.BaseEvent
        {
            EventDelegate internalDelegate;
            if (delegateLookup.TryGetValue(del, out internalDelegate))
            {
                EventDelegate tempDel;
                if (delegates.TryGetValue(typeof(T), out tempDel))
                {
                    tempDel -= internalDelegate;
                    if (tempDel == null)
                    {
                        delegates.Remove(typeof(T));
                    }
                    else
                    {
                        delegates[typeof(T)] = tempDel;
                    }
                }

                delegateLookup.Remove(del);
            }
        }

        public void Raise(Events.BaseEvent e)
        {
            EventDelegate del;
            if (delegates.TryGetValue(e.GetType(), out del))
            {
                del.Invoke(e);
            }
        }
    }
}
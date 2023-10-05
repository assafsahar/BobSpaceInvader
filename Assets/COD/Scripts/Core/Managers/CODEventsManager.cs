using System;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

namespace COD.Core
{
    public class CODEventsManager
    {
        private Dictionary<CODEventNames, List<Action<object>>> activeListeners = new();
        private Dictionary<CODEventNames, List<object>> eventQueue = new();

        public void AddListener(CODEventNames eventName, Action<object> listener)
        {
            if (activeListeners.TryGetValue(eventName, out var listOfEvents))
            {
                if (listOfEvents.Contains(listener))
                {
                    return;
                }
                listOfEvents.Add(listener);
            }
            else
            {
                activeListeners.Add(eventName, new List<Action<object>> { listener });
            }

            // Process queued events if any
            if (eventQueue.TryGetValue(eventName, out var queuedEvents))
            {
                foreach (var obj in queuedEvents)
                {
                    listener.Invoke(obj);
                }
                eventQueue.Remove(eventName); 
            }
        }

        public void RemoveListener(CODEventNames eventName, Action<object> listener)
        {
            if (activeListeners.TryGetValue(eventName, out var listOfEvents))
            {
                listOfEvents.Remove(listener);

                if (listOfEvents.Count <= 0)
                {
                    activeListeners.Remove(eventName);
                }
            }
        }

        public void InvokeEvent(CODEventNames eventName, object obj)
        {
            if (activeListeners.TryGetValue(eventName, out var listOfEvents))
            {
                foreach (var action in listOfEvents)
                {
                    action.Invoke(obj);
                }
            }
            else
            {
                // If there are no active listeners, queue the event
                if (eventQueue.TryGetValue(eventName, out var queuedEvents))
                {
                    queuedEvents.Add(obj);
                }
                else
                {
                    eventQueue.Add(eventName, new List<object> { obj });
                }
            }
        }
    }

    public enum CODEventNames
    {
        OnMoveUp = 0,
        OnMoveDown = 1,
        OnMoveStraight = 2,
        OnTouchStarted = 3,
        OnTouchEnded = 4,
        OnTouchStayed = 5,
        OnCollectableCollected = 6,
        OnScoreSet = 7,
        OnEnergyChanged = 8,
        OnUpgraded = 9,
        RequestScoreUpdate = 10
    }
}

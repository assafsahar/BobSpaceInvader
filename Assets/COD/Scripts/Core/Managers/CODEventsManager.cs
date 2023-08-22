using System;
using System.Collections.Generic;

namespace COD.Core
{
    public class CODEventsManager
    {
        private Dictionary<CODEventNames, List<Action<object>>> activeListeners = new();

        public void AddListener(CODEventNames eventName, Action<object> listener)
        {
            if (activeListeners.TryGetValue(eventName, out var listOfEvents))
            {
                if (listOfEvents.Contains(listener))
                {
                    return;
                }
                listOfEvents.Add(listener);
                return;
            }

            activeListeners.Add(eventName, new List<Action<object>> { listener });
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
        OnUpgraded = 9
    }
}

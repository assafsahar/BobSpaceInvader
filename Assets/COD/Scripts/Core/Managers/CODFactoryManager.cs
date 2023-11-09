using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace COD.Core
{
    /// <summary>
    /// This class is a factory for creating game objects
    /// </summary>
    public class CODFactoryManager
    {
        public void CreateAsync<T>(T origin, Vector3 pos, Action<T> onCreated) where T : Object
        {
            var clone = Object.Instantiate(origin, pos, Quaternion.identity);
            onCreated?.Invoke(clone);
        }

        public void MultiCreateAsync<T>(T origin, Vector3 pos, int amount, Action<List<T>> onCreated) where T : Object
        {
            List<T> createdObjects = new List<T>();

            for (var i = 0; i < amount; i++)
            {
                CreateAsync(origin, pos, OnCreated);
            }

            void OnCreated(T createdObject)
            {
                createdObjects.Add(createdObject);

                if (createdObjects.Count == amount)
                {
                    onCreated?.Invoke(createdObjects);
                }
            }
        }
    }
}

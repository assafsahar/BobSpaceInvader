using System;
using System.Collections.Generic;
using UnityEngine;

namespace COD.Core
{
    /// <summary>
    ///  this is an object pooling system. It includes methods for reinitializing 
    ///  the object when it's pulled from the pool and for resetting it before 
    ///  it's returned to the pool
    /// </summary>
    public class CODPoolManager
    {
        private Dictionary<PoolNames, CODPool> pools = new();

        private Transform rootPools;

        public CODPoolManager()
        {
            rootPools = new GameObject().transform;
            UnityEngine.Object.DontDestroyOnLoad(rootPools);
        }

        public void InitPool(string resourceName, int amount, int maxAmount, PoolNames poolName)
        {
            var original = Resources.Load<CODPoolable>(resourceName);
            InitPool(original, amount, maxAmount, poolName);
        }
        public void InitParticlePool(GameObject particleEffectPrefab, int initialSize, PoolNames poolName)
        {
            List<CODPoolable> particlePoolables = new List<CODPoolable>();

            for (int i = 0; i < initialSize; i++)
            {
                GameObject obj = GameObject.Instantiate(particleEffectPrefab);
                obj.name = particleEffectPrefab.name;
                obj.transform.parent = rootPools;
                obj.SetActive(false);

                CODPoolable poolableParticle = obj.GetComponent<CODPoolable>();
                if (poolableParticle == null)
                {
                    poolableParticle = obj.AddComponent<CODPoolable>();
                }

                poolableParticle.PoolName = poolName;
                particlePoolables.Add(poolableParticle);
            }

            var pool = new CODPool
            {
                AllPoolables = new Queue<CODPoolable>(particlePoolables),
                UsedPoolables = new List<CODPoolable>(),
                AvailablePoolables = new Queue<CODPoolable>(particlePoolables),
                MaxPoolables = initialSize
            };

            pools.Add(poolName, pool);
        }
        public void InitPool(CODPoolable original, int amount, int maxAmount, PoolNames poolName)
        {
            CODManager.Instance.FactoryManager.MultiCreateAsync(original, Vector3.zero, amount,
                delegate (List<CODPoolable> list)
                {
                    foreach (var poolable in list)
                    {
                        poolable.name = original.name;
                        poolable.transform.parent = rootPools;
                        poolable.gameObject.SetActive(false);
                    }

                    var pool = new CODPool
                    {
                        AllPoolables = new Queue<CODPoolable>(list),
                        UsedPoolables = new List<CODPoolable>(),
                        AvailablePoolables = new Queue<CODPoolable>(list),
                        MaxPoolables = maxAmount
                    };

                    pools.Add(poolName, pool);
                });
        }

        public CODPoolable GetPoolable(PoolNames poolName)
        {
            if (pools.TryGetValue(poolName, out CODPool pool))
            {
                if (pool.AvailablePoolables.TryDequeue(out CODPoolable poolable))
                {
                    CODDebug.Log($"[GetPoolable] Taken: {poolable.name}, ID: {poolable.UniqueId}");

                    poolable.OnTakenFromPool();
                    pool.UsedPoolables.Add(poolable);
                    poolable.gameObject.SetActive(true);
                    return poolable;
                }

                //Create more
                //CODDebug.Log($"pool - {poolName} no enough poolables, used poolables {pool.UsedPoolables.Count}");

                return null;
            }

            //CODDebug.Log($"pool - {poolName} wasn't initialized");
            return null;
        }

        public void ReturnPoolable(CODPoolable poolable)
        {
            if (pools.TryGetValue(poolable.PoolName, out CODPool pool))
            {
                if (pool.UsedPoolables.Contains(poolable))
                {
                    CODDebug.Log($"Returning {poolable.name}, ID: {poolable.UniqueId}");

                    poolable.OnReturnedToPool();
                    poolable.gameObject.SetActive(false);

                    pool.UsedPoolables.Remove(poolable);
                    //pool.UsedPoolables.Dequeue();
                    pool.AvailablePoolables.Enqueue(poolable);
                    CODDebug.Log($"Returned {poolable.name} to pool. Available: {pool.AvailablePoolables.Count}, ID: {poolable.UniqueId}");
                }
                else
                {
                    CODDebug.LogException($"Trying to return a poolable that isn't marked as used: {poolable.name}, ID: {poolable.UniqueId}");
                }

            }
            else
            {
                CODDebug.LogException($"Attempted to return a poolable of type {poolable.PoolName}, but no pool exists for this type.");
            }
        }

        public void DestroyPool(PoolNames name)
        {
            if (pools.TryGetValue(name, out CODPool pool))
            {
                foreach (var poolable in pool.AllPoolables)
                {
                    poolable.PreDestroy();
                    ReturnPoolable(poolable);
                }

                foreach (var poolable in pool.AllPoolables)
                {
                    UnityEngine.Object.Destroy(poolable);
                }

                pool.AllPoolables.Clear();
                pool.AvailablePoolables.Clear();
                pool.UsedPoolables.Clear();

                pools.Remove(name);
            }
        }
        public void Cleanup()
        {
            foreach (var poolName in Enum.GetValues(typeof(PoolNames)))
            {
                DestroyPool((PoolNames)poolName);
            }

            if (rootPools != null)
                UnityEngine.Object.Destroy(rootPools.gameObject);
        }
    }

    public class CODPool
    {
        public Queue<CODPoolable> AllPoolables = new();
        public List<CODPoolable> UsedPoolables = new();
        public Queue<CODPoolable> AvailablePoolables = new();
        public int MaxPoolables = 100;
    }

    public enum PoolNames
    {
        NA = -1,
        ScoreToast = 0,
        Collectable = 1,
        NormalCoinToast = 2,
        SuperCoinToast = 3,
        EnergyToast = 4,
        ParticleEffect = 5
    }
}
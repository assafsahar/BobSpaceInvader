using System.Collections.Generic;
using UnityEngine;

namespace COD.Core
{
    public class CODPoolManager
    {
        private Dictionary<PoolNames, CODPool> pools = new();

        private Transform rootPools;

        public CODPoolManager()
        {
            rootPools = new GameObject().transform;
            Object.DontDestroyOnLoad(rootPools);
        }

        public void InitPool(string resourceName, int amount, int maxAmount = 100)
        {
            var original = Resources.Load<CODPoolable>(resourceName);
            InitPool(original, amount, maxAmount);
        }
        public void InitPool(CODPoolable original, int amount, int maxAmount)
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
                        UsedPoolables = new Queue<CODPoolable>(),
                        AvailablePoolables = new Queue<CODPoolable>(list),
                        MaxPoolables = maxAmount
                    };

                    pools.Add(original.PoolName, pool);
                });
        }

        public CODPoolable GetPoolable(PoolNames poolName)
        {
            if (pools.TryGetValue(poolName, out CODPool pool))
            {
                if (pool.AvailablePoolables.TryDequeue(out CODPoolable poolable))
                {
                    //CODDebug.Log($"GetPoolable - {poolName}");

                    poolable.OnTakenFromPool();
                    pool.UsedPoolables.Enqueue(poolable);
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
                pool.AvailablePoolables.Enqueue(poolable);
                poolable.OnReturnedToPool();
                poolable.gameObject.SetActive(false);
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
                    Object.Destroy(poolable);
                }

                pool.AllPoolables.Clear();
                pool.AvailablePoolables.Clear();
                pool.UsedPoolables.Clear();

                pools.Remove(name);
            }
        }
    }

    public class CODPool
    {
        public Queue<CODPoolable> AllPoolables = new();
        public Queue<CODPoolable> UsedPoolables = new();
        public Queue<CODPoolable> AvailablePoolables = new();
        public int MaxPoolables = 100;
    }

    public enum PoolNames
    {
        NA = -1,
        ScoreToast = 0,
        Collectable = 1
    }
}


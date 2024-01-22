using COD.Core;
using System;

/// <summary>
/// this class is meant to be inherited by objects that are 
/// meant to be reused from an object pool to improve performance 
/// by avoiding frequent instantiation and destruction
/// </summary>
public class CODPoolable : CODMonoBehaviour
{
    public PoolNames PoolName;
    public string UniqueId { get; private set; }
    private void Awake()
    {
        UniqueId = Guid.NewGuid().ToString();
    }
    virtual public void OnTakenFromPool()
    {
        this.gameObject.SetActive(true);
    }
    virtual public void OnReturnedToPool()
    {
        this.gameObject.SetActive(false);
    }

    virtual public void PreDestroy()
    {

    }
}

using System;

namespace COD.Core
{
    public class CODGameLoaderBase : CODMonoBehaviour
    {
        public virtual void StartLoad(Action onComplete)
        {
            onComplete?.Invoke();
        }

    }
}


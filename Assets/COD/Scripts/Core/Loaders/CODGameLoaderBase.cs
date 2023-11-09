using System;

namespace COD.Core
{
    /// <summary>
    /// provides common loading functionality for game loaders
    /// </summary>
    public class CODGameLoaderBase : CODMonoBehaviour
    {
        public virtual void StartLoad(Action onComplete)
        {
            onComplete?.Invoke();
        }

    }
}


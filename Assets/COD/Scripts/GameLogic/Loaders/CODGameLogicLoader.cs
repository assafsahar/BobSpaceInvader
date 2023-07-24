using COD.Core;
using System;
using UnityEngine;

namespace COD.GameLogic
{
    public class CODGameLogicLoader : CODGameLoaderBase
    {
        public override void StartLoad(Action onComplete)
        {
            CODGameLogicManager codGameLogicManager = new CODGameLogicManager();
            codGameLogicManager.LoadManager(() =>
            {
                base.StartLoad(onComplete);
            });
        } 
 
    }
}

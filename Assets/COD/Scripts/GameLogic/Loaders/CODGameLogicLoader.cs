using COD.Core;
using System;
using UnityEngine;

namespace COD.GameLogic
{
    /// <summary>
    /// handles the initial loading and setup of the game logic managers.
    /// </summary>
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

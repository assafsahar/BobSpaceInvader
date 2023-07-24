using COD.Core;
using System;

namespace COD.GameLogic
{
    public class CODGameLogicManager : ICODBaseManager
    {
        public static CODGameLogicManager Instance;
        //public CODScoreManager ScoreManager;
        //public CODUpgradeManager UpgradeManager;

        public CODGameLogicManager()
        {
            if (Instance != null)
            {
                return;
            }

            Instance = this;
        }

        public void LoadManager(Action onComplete)
        {
            /*ScoreManager = new CODScoreManager();
            UpgradeManager = new CODUpgradeManager(
             
                );*/
            onComplete.Invoke();
        }
    }
}

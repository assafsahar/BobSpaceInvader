using System;

namespace COD.Core
{
    /// <summary>
    /// This class serves as a central point of management for various subsystems
    /// of the COD game, such as event management and object pooling. 
    /// It offers singleton-like access to these subsystems, which are used 
    /// throughout the game's classes.
    /// </summary>
    public class CODManager : ICODBaseManager
    {
        public static CODManager Instance;
        public CODEventsManager EventsManager;
        public CODFactoryManager FactoryManager;
        public CODPoolManager PoolManager;
        public CODSaveManager SaveManager;
        public CODConfigOfflineManager ConfigManager;
        /*public CODCrashManager CrashManager;
        
        
        
        
        */
        public CODPopupManager PopupManager;

        public Action onInitAction;
        //public InputManager InputManager;

        public CODManager()
        {
            if (Instance != null)
            {
                return;
            }

            Instance = this;
        }

        public void LoadManager(Action onComplete)
        {
            onInitAction = onComplete;
            InitManagers();
        }

        private void InitManagers()
        {
            EventsManager = new CODEventsManager();
            //InputManager = new InputManager();
            FactoryManager = new CODFactoryManager();
            PoolManager = new CODPoolManager();
            SaveManager = new CODSaveManager();
            ConfigManager = new CODConfigOfflineManager();
            //CODDebug.Log("InitManagers");
            /*CrashManager = new CODCrashManager();
            
            
            
            */
            PopupManager = new CODPopupManager();
            onInitAction.Invoke();
        }

    }
}

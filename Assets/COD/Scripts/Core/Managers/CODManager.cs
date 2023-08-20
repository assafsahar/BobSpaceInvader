using System;

namespace COD.Core
{
    public class CODManager : ICODBaseManager
    {
        public static CODManager Instance;
        public CODEventsManager EventsManager;
        public CODFactoryManager FactoryManager;
        public CODPoolManager PoolManager;

        /*public CODCrashManager CrashManager;
        
        
        
        public CODSaveManager SaveManager;
        public CODConfigManager ConfigManager;*/

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
            //InitFirebase(delegate { InitManagers(); });
            InitManagers();
        }

        private void InitManagers()
        {
            EventsManager = new CODEventsManager();
            //InputManager = new InputManager();
            FactoryManager = new CODFactoryManager();
            PoolManager = new CODPoolManager();

            //CODDebug.Log("InitManagers");
            /*CrashManager = new CODCrashManager();
            
            
            SaveManager = new CODSaveManager();
            ConfigManager = new CODConfigManager(delegate
            {
                onInitAction.Invoke();
            });*/
            onInitAction.Invoke();
        }

        /*private void InitFirebase(Action onComplete)
        {
            Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task => {
                var dependencyStatus = task.Result;
                if (dependencyStatus == Firebase.DependencyStatus.Available)
                {
                    // Create and hold a reference to your FirebaseApp,
                    // where app is a Firebase.FirebaseApp property of your application class.
                    var app = Firebase.FirebaseApp.DefaultInstance;

                    // Set a flag here to indicate whether Firebase is ready to use by your app.
                    CODDebug.Log($"Firebase initialized");
                    onComplete.Invoke();
                }
                else
                {
                    CODDebug.LogException($"Could not resolve all Firebase dependencies: {dependencyStatus}");
                    // Firebase Unity SDK is not safe to use here.
                }
            });
        }*/
    }
}

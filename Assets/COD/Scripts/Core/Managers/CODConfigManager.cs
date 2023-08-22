using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace COD.Core
{
    public class CODConfigManager
    {
        private Action onInit;
        public CODConfigManager(Action onComplete)
        {
            onInit = onComplete;
            var defaults = new Dictionary<string, object>();

            defaults.Add("UpgradableConfig", "{}");
            //CODDebug.Log("CODConfigManager");
         }

        public void GetConfigAsync<T>(string configID, Action<T> onComplete)
        {
            //CODDebug.Log("GetConfigAsync");
            //var saveData = JsonConvert.DeserializeObject<T>(saveJson);

            //onComplete.Invoke(saveData);
        }
        private void OnDefaultValuesSet(Task task)
        {
            //CODDebug.Log("OnDefaultValuesSet");
        }

        private void OnFetchComplete(Task obj)  
        {
            //CODDebug.Log("OnFetchComplete");
        }

        private void OnActivateComplete(Task obj)
        {
            //CODDebug.Log("OnActivateComplete");
            onInit.Invoke();
        }
    }
    public class CODConfigOfflineManager
    {
        public void GetConfigAsync<T>(string configID, Action<T> onComplete)
        {
            var path = $"Assets/COD/Configs/{configID}.json";
            var saveJson = File.ReadAllText(path);
            var saveData = JsonConvert.DeserializeObject<T>(saveJson);

            onComplete.Invoke(saveData);
        }
    }
}

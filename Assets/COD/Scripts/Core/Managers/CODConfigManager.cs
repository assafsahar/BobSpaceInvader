using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

namespace COD.Core
{
    /// <summary>
    /// this class is designed to manage configuration settings it loads from external sources
    /// </summary>
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
            var path = $"Configs/{configID}";
            TextAsset textAsset = Resources.Load<TextAsset>(path);
            if (textAsset != null)
            {

                var saveData = JsonConvert.DeserializeObject<T>(textAsset.text);
                onComplete.Invoke(saveData);
            }
            else
            {
                Debug.LogError($"Config not found for {configID}");
            }
        }
    }
}

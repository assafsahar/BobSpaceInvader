using System;
using System.IO;
using UnityEngine;
using JsonConvert = Newtonsoft.Json.JsonConvert;

namespace COD.Core
{
    /// <summary>
    /// Handles saving and loading of the game's state 
    /// to persistent storage. This could involve player preferences, 
    /// game progress, high scores, and other data that needs to be retained between sessions
    /// </summary>
    public class CODSaveManager
    {
        public void Save(ICODSaveData saveData)
        {
            var saveID = saveData.GetType().FullName;
            //HOGDebug.Log(saveID);
            var saveJson = JsonConvert.SerializeObject(saveData);
            //HOGDebug.Log(saveJson);

            var path = $"{Application.persistentDataPath}/{saveID}.codSave";

            File.WriteAllText(path, saveJson);
        }

        public void Load<T>(Action<T> onComplete) where T : ICODSaveData
        {
            if (!HasData<T>())
            {
                onComplete.Invoke(default);
                return;
            }
            var saveID = typeof(T).FullName;
            var path = $"{Application.persistentDataPath}/{saveID}.codSave";
            var saveJson = File.ReadAllText(path);
            var saveData = JsonConvert.DeserializeObject<T>(saveJson);

            //HOGDebug.Log($"saveID={saveID}");
            //HOGDebug.Log($"saveJson={saveJson}");

            onComplete.Invoke(saveData);
        }

        public bool HasData<T>() where T : ICODSaveData
        {
            var saveID = typeof(T).FullName;
            var path = $"{Application.persistentDataPath}/{saveID}.codSave";
            return File.Exists(path);
        }
    }

    public interface ICODSaveData
    {

    }
}

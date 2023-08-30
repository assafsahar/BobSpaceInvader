using System.IO;
using UnityEditor;
using UnityEngine;

namespace COD.Core
{
    public class CODClearDataTool
    {
        public class ClearDataTool
        {
            [MenuItem("COD/ClearData")]
            public static void ClearAllDataTool()
            {
                var path = Application.persistentDataPath;
                var files = Directory.GetFiles(path);

                foreach (var file in files)
                {
                    if (file.Contains("COD"))
                    {
                        File.Delete(file);
                    }
                }

                PlayerPrefs.DeleteAll();
            }
        }
    }

}

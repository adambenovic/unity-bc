using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

public class SerializationManager : MonoBehaviour
{
    public static string LifelineType = "uml:Lifeline";
    
    public static void LoadElementsFromFile(string path)
    {
        if (path == null || path.Length == 0) return;
        
        // 1) Load JSON from FILE
        string fileContent = File.ReadAllText(path);

        List<string> lifelines = new List<string>();
        dynamic json = JsonConvert.DeserializeObject(fileContent);
        foreach (var element in json)
        {
            if (element["XmiType"] == LifelineType)
            {
                Debug.Log(element["name"]);
                lifelines.Add((string)element["name"]);
            }
        }
    }
}

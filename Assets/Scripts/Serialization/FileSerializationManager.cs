using UnityEngine;
using System.IO;
using System.Collections.Generic;
using MongoDB.Bson;
using Data.MOF;
using Data;

public class FileSerializationManager : MonoBehaviour
{
    public static void LoadElementsFromFile(string path)
    {
        if (path == null || path.Length == 0) return;
        
        // 1) Load JSON from FILE
        string fileContent = File.ReadAllText(path);
        Debug.Log(fileContent);
        List<BsonDocument> bson = MongoDB.Bson.Serialization.BsonSerializer.Deserialize<List<BsonDocument>>(fileContent);

        // 2) - 6)
        XmiCollection elements = SerializationManager.LoadElements(bson);
        Debug.Log("Loaded " + elements.MofElements().Count + " MOF elements from file \"" + path + "\".");
    }
}

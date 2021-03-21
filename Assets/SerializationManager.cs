using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;
using WebSocketSharp;

public class SerializationManager : MonoBehaviour
{
    public static string LifelineType = "uml:Lifeline";
    public static string OccourenceType = "uml:OccurrenceSpecification";
    public static string MessageType = "uml:Message";
    
    public static Dictionary<string, List<string>> LoadElementsFromFile(string path)
    {
        if (path.IsNullOrEmpty()) return new Dictionary<string, List<string>>();

        string fileContent = File.ReadAllText(path);
        Dictionary<string, List<string>> elements = new Dictionary<string, List<string>>();
        dynamic json = JsonConvert.DeserializeObject(fileContent);

        foreach (var element in json)
        {
            if (element["XmiType"] == LifelineType)
            {
                elements.Add((string)element["name"], findAllMethods((string)element["XmiId"], json));
            }
        }

        return elements;
    }

    private static List<string> findAllMethods(string xmiId, dynamic json)
    {
        List<string> methods = new List<string>();
        foreach (var element in json)
        {
            if (element["XmiType"] == OccourenceType && element["covered"][0]["XmiIdRef"] == xmiId)
            {
                string messageId = element["XmiId"];
                foreach (var element2 in json)
                {
                    if (element2["XmiType"] == MessageType && element2["receiveEvent"]["XmiIdRef"] == messageId)
                    {
                        methods.Add((string)element2["name"]);
                    }
                }
            }
        }

        return methods;
    }
}

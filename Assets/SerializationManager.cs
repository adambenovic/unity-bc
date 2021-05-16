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
    
    public static Dictionary<string, RawClass> LoadElementsFromFile(string path)
    {
        Dictionary<string, RawClass> elements = new Dictionary<string, RawClass>();
        if (path.IsNullOrEmpty()) return elements;

        string fileContent = File.ReadAllText(path);
        dynamic json = JsonConvert.DeserializeObject(fileContent);

        foreach (var element in json)
        {
            if (element["XmiType"] == LifelineType)
            {
                List<string> methods = new List<string>();
                List<string> associations = new List<string>();
                findAllMethodsAndAssociations((string) element["XmiId"], json, ref methods, ref associations);
                string name = (string) element["name"];
                elements.Add(name, new RawClass(name, methods, associations));
            }
        }

        return elements;
    }

    private static void findAllMethodsAndAssociations(string xmiId, dynamic json, ref List<string> methods, ref List<string> associations)
    {
        List<string> sendEvents = new List<string>();
        foreach (var element in json)
        {
            if (element["XmiType"] == OccourenceType && element["covered"][0]["XmiIdRef"] == xmiId)
            {
                string messageId = (string)element["XmiId"];
                foreach (var element2 in json)
                {
                    if (element2["XmiType"] == MessageType && element2["receiveEvent"]["XmiIdRef"] == messageId)
                    {
                        methods.Add((string)element2["name"]);
                        sendEvents.Add((string)element2["sendEvent"]["XmiIdRef"]);
                    }
                }
            }
        }

        foreach (var element in json)
        {
            foreach (var sendEvent in sendEvents)
            {
                if (element["XmiType"] == OccourenceType && element["XmiId"] == sendEvent)
                {
                    string lifelineId = (string)element["covered"][0]["XmiIdRef"];
                    foreach (var element2 in json)
                    {
                        if (element2["XmiType"] == LifelineType && element2["XmiId"] == lifelineId)
                        {
                            associations.Add((string)element2["name"]);
                        }
                    }
                }
            }
        }
    }
}

using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using MongoDB.Bson;
using Data;
using Data.MOF;

public class SerializationManager : MonoBehaviour
{
    public static XmiCollection LoadElements(List<BsonDocument> jsonElements)
    {
        XmiCollection container = new XmiCollection();

        // 2) Build MOF from JSON
        foreach (BsonDocument element in jsonElements)
        {
            string str = null;
            try
            {
                str = (string)element.GetValue("XmiType");
                FactoryRegister.Singleton.RegisteredXmiString[str].BuildMofFromJson(element, container);
            }
            catch (KeyNotFoundException e)
            {
                Debug.Log("LOAD 2) \t Unable to build MOF from JSON " + str);
                Debug.Log(e);
            }
        }

        // 3)  Update MOF from JSON
        foreach (BsonDocument element in jsonElements)
        {
            string str = null;
            try
            {
                str = (string)element.GetValue("XmiType");
                FactoryRegister.Singleton.RegisteredXmiString[str].UpdateMofFromJson(element, container);
            }
            catch (KeyNotFoundException e)
            {
                Debug.Log("LOAD 3) \t Unable to update data of MOF from JSON " + str);
                Debug.Log(e);
            }
        }
        
        return container;
    }
}

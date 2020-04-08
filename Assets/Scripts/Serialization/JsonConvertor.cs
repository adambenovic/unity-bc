using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;
using Data;
using Data.MOF;
using Data.DI;
using System;
using System.Reflection;
using MongoDB.Bson;

public class JsonConvertor : MonoBehaviour {

    public static XmiElement UpdateFromJSON(XmiElement xmi, BsonDocument json)
    {
        // TODO Universal 
        return null;
    }

    public static JObject ToJSON(XmiElement xmi)
    {
        // https://stackoverflow.com/questions/21991223/convert-object-of-any-type-to-jobject-with-json-net
        //return JObject.FromObject(xmi);
        Type type = xmi.GetType();
        Dictionary<string, object> result = new Dictionary<string, object>();
        result["XmiId"] = xmi.XmiId;
        result["XmiType"] = xmi.XmiType();
        foreach (FieldInfo fieldInfo in type.GetFields())
        {
            object value = fieldInfo.GetValue(xmi);
            if (!fieldInfo.IsStatic && value != null)
            {
                if (value is IEnumerable && !(value is string))
                {
                    // Multiple References
                    IEnumerable enumerable = (IEnumerable)value;
                    List<object> subResults = new List<object>();
                    foreach (object subObj in enumerable)
                    {
                        if (!(subObj is XmiElement))
                        {
                            continue; // TODO Check if this is correct
                            //throw new SystemException();
                        }
                        XmiElement referencedXmi = (XmiElement)subObj;
                        Dictionary<string, string> reference = new Dictionary<string, string>();
                        reference["XmiIdRef"] = referencedXmi.XmiId;
                        subResults.Add(reference);
                    }
                    result[fieldInfo.Name] = subResults;
                }
                else if (value is XmiElement)
                {
                    // Single Reference
                    XmiElement referencedXmi = (XmiElement)value;
                    Dictionary<string, string> reference = new Dictionary<string, string>();
                    reference["XmiIdRef"] = referencedXmi.XmiId;
                    result[fieldInfo.Name] = reference;
                }
                else if (value is GameObject || value is Quaternion) 
                {
                    // Do not serialize DG Elements
                    // TODO FIXME Quaternion should be possible to serialize
                }
                else
                {
                    // Primitive
                    result[fieldInfo.Name] = value;
                }
            }
        }
        return JObject.FromObject(result);
    }


    public static MofElement FindMofByXmiId(BsonDocument json, XmiCollection container, string attributeName)
    {
        BsonDocument referenceWrapper;
        string xmiId;
        MofElement result;
        try
        {
            referenceWrapper = json.GetValue(attributeName).AsBsonDocument;
            xmiId = (string)referenceWrapper.GetValue("XmiIdRef");
            result = container.GetMofElement(xmiId);
        }
        catch (KeyNotFoundException)
        {
            return null;
        }
        return result;
    }

    public static DiElement FindDiByXmiId(BsonDocument json, XmiCollection container, string attributeName)
    {
        BsonDocument referenceWrapper;
        string xmiId;
        DiElement result;
        try
        {
            referenceWrapper = json.GetValue(attributeName).AsBsonDocument;
            xmiId = (string)referenceWrapper.GetValue("XmiIdRef");
            result = container.GetDiElement(xmiId);
        }
        catch (KeyNotFoundException)
        {
            return null;
        }
        return result;
    }

    public static List<MofElement> FindMofArrayByXmiId(BsonDocument json, XmiCollection container, string arrayName)
    {
        List<MofElement> result = new List<MofElement>();

        BsonArray array;

        try
        {
            array = json.GetValue(arrayName).AsBsonArray;
            foreach (BsonDocument referenceWrapper in json.GetValue(arrayName).AsBsonArray)
            {
                string xmiId = (string)referenceWrapper.GetValue("XmiIdRef");
                result.Add(container.GetMofElement(xmiId));
            }
        }
        catch (KeyNotFoundException)
        {
            return new List<MofElement>();
        }
        return result;
    }

    public static List<DiElement> FindDiArrayByXmiId(BsonDocument json, XmiCollection container, string arrayName)
    {
        List<DiElement> result = new List<DiElement>();

        BsonArray array;

        try
        {
            array = json.GetValue(arrayName).AsBsonArray;
            foreach (BsonDocument referenceWrapper in json.GetValue(arrayName).AsBsonArray)
            {
                string xmiId = (string)referenceWrapper.GetValue("XmiIdRef");
                result.Add(container.GetDiElement(xmiId));
            }
        }
        catch (KeyNotFoundException)
        {
            return new List<DiElement>();
        }
        return result;
    }

}

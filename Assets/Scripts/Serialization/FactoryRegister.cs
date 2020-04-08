using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class FactoryRegister : MonoBehaviour
{
    // This singleton is initialized as Unity GameObject
    // You can check this variable for 'null' to see if at least one FactoryRegister was initialized
    public static FactoryRegister Singleton { get; private set; } = null;

    public readonly Dictionary<string, AbstractFactory> RegisteredXmiString = new Dictionary<string, AbstractFactory>();

    public readonly Dictionary<Type, AbstractFactory> RegisteredClass = new Dictionary<Type, AbstractFactory>();

    public void RegisterXmiString(string key, AbstractFactory obj)
    {
        if (!RegisteredXmiString.ContainsKey(key))
        {
            RegisteredXmiString.Add(key, obj);
            Debug.Log("FACTORY REGISTER\tKey: " + key + "\tValue: " + obj);
        }
    }

    public void RegisterClass(Type key, AbstractFactory obj)
    {
        if (!RegisteredClass.ContainsKey(key))
        {
            RegisteredClass.Add(key, obj);
            Debug.Log("FACTORY REGISTER\tKey: " + key + "\tValue: " + obj);
        }
    }

    void Start()
    {
        if (Singleton == null)
        {
            Singleton = this;
        }
        /*
        Assembly assembly = typeof(AbstractFactory).Assembly;
        var types = assembly.GetTypes().Where(t => t.IsSubclassOf(typeof(AbstractFactory)));
        foreach (System.Type type in types)
        {
            AbstractFactory instance = (AbstractFactory)Activator.CreateInstance(type);
            instance.RegisterFactory();
        }
        foreach (KeyValuePair<string, AbstractFactory> element in FactoryRegister.Singleton.RegisteredXmiString)
        {
            Debug.Log("FACTORY REGISTER\tKey: " + element.Key + "\tValue: " + element.Value.GetType());
        }
        foreach (KeyValuePair<Type, AbstractFactory> element in FactoryRegister.Singleton.RegisteredClass)
        {
            Debug.Log("FACTORY REGISTER\tKey:" + element.Key + "\tValue: " + element.Value.GetType());
        }
        */
    }
}

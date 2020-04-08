using System.Collections.Generic;
using MongoDB.Bson;

using Data;
using Data.MOF;
using UnityEngine;

public abstract class AbstractFactory : MonoBehaviour
{
    public abstract void RegisterFactory(FactoryRegister register);

    public virtual void Start()
    {
        RegisterFactory(GameObject.FindObjectOfType<FactoryRegister>());
    }
    
    public abstract List<MofElement> BuildMofFromJson(BsonDocument json, XmiCollection container);
    public abstract void UpdateMofFromJson(BsonDocument json, XmiCollection container);
}

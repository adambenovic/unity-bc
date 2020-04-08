using System.Collections.Generic;
using UnityEngine;
using Data;
using Data.MOF;
using MongoDB.Bson;

public class LifelineFactory : AbstractFactory
{
    public GameObject PrefabLifeline = null;

    public override void RegisterFactory(FactoryRegister register)
    {
        register.RegisterXmiString("uml:Lifeline", this);
        register.RegisterClass(typeof(UML.Interactions.Lifeline), this);
    }


    public override List<global::Data.MOF.MofElement> BuildMofFromJson(BsonDocument json, XmiCollection container)
    {
        List<global::Data.MOF.MofElement> result = new List<global::Data.MOF.MofElement>();
       
        UML.Interactions.Lifeline mofLifeLine = new UML.Interactions.Lifeline();

        string id = (string)json.GetValue("XmiId");

        mofLifeLine.XmiId = id;

        container.AddMofElement(mofLifeLine);
        result.Add(mofLifeLine);

        return result;
    }

    public override void UpdateMofFromJson(BsonDocument json, XmiCollection container)
    {
        UML.Interactions.Lifeline mofLifeLine = (UML.Interactions.Lifeline)container.GetMofElement((string)json.GetValue("XmiId"));

        mofLifeLine.interaction = (UML.Interactions.Interaction)JsonConvertor.FindMofByXmiId(json, container, "interaction");

        mofLifeLine.ownedComment.Clear();
        foreach (MofElement mof in JsonConvertor.FindMofArrayByXmiId(json, container, "ownedComment"))
            mofLifeLine.ownedComment.Add((UML.CommonStructure.Comment)mof);

        mofLifeLine.ownedElement.Clear();
        foreach (MofElement mof in JsonConvertor.FindMofArrayByXmiId(json, container, "ownedElement"))
            mofLifeLine.ownedElement.Add((UML.CommonStructure.Element)mof);

        mofLifeLine.coveredBy.Clear();
        foreach (MofElement mof in JsonConvertor.FindMofArrayByXmiId(json, container, "coveredBy"))
            mofLifeLine.coveredBy.Add((UML.Interactions.InteractionFragment)mof);

        mofLifeLine.name = json.GetValue("name").ToString();

        mofLifeLine.decomposedAs = (UML.Interactions.PartDecomposition)JsonConvertor.FindMofByXmiId(json, container, "decomposedAs");

        mofLifeLine.owner = (UML.CommonStructure.Element)JsonConvertor.FindMofByXmiId(json, container, "owner");
    }
}


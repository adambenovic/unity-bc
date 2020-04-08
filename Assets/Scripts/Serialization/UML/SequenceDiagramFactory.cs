using System.Collections.Generic;
using UnityEngine;
using Data;
using Data.MOF;
using UML.Interactions;
using MongoDB.Bson;

public class SequenceDiagramFactory : AbstractFactory
{
    public GameObject PrefabSequenceDiagram = null;
    public GameObject PrefabLayerRow = null;

    public override void RegisterFactory(FactoryRegister register)
    {
        register.RegisterXmiString("uml:Interaction", this);
        register.RegisterClass(typeof(UML.Interactions.Interaction), this);
    }
    

    public override List<global::Data.MOF.MofElement> BuildMofFromJson(BsonDocument json, XmiCollection container)
    {
        List<global::Data.MOF.MofElement> result = new List<global::Data.MOF.MofElement>();
       
        UML.Interactions.Interaction MofSequenceDiagram = new UML.Interactions.Interaction();

        string id = (string)json.GetValue("XmiId");
        MofSequenceDiagram.XmiId = id;
        container.AddMofElement(MofSequenceDiagram);
        result.Add(MofSequenceDiagram);

        return result;
    }
    
    public override void UpdateMofFromJson(BsonDocument json, XmiCollection container)
    {
        UML.Interactions.Interaction mofInteraction = (UML.Interactions.Interaction)container.GetMofElement((string)json.GetValue("XmiId"));

        //lifeline
        mofInteraction.lifeline.Clear();
        foreach (MofElement mof in JsonConvertor.FindMofArrayByXmiId(json, container, "lifeline"))
        {
            mofInteraction.lifeline.Add((UML.Interactions.Lifeline)mof);
        }

        // covered
        mofInteraction.covered.Clear();
        foreach (MofElement mof in JsonConvertor.FindMofArrayByXmiId(json, container, "covered"))
        {
            mofInteraction.covered.Add((UML.Interactions.Lifeline)mof);
        }

        // message
        mofInteraction.message.Clear();
        foreach (MofElement mof in JsonConvertor.FindMofArrayByXmiId(json, container, "message"))
        {
            mofInteraction.message.Add((UML.Interactions.Message)mof);
        }

        // owner
        mofInteraction.owner = (UML.CommonStructure.Element)JsonConvertor.FindMofByXmiId(json, container, "owner");

        // fragment
        mofInteraction.fragment.Clear();
        foreach (MofElement mof in JsonConvertor.FindMofArrayByXmiId(json, container, "fragment"))
        {
            mofInteraction.fragment.Add((UML.Interactions.InteractionFragment)mof);
        }

        // formalGate
        mofInteraction.formalGate.Clear();
        foreach (MofElement mof in JsonConvertor.FindMofArrayByXmiId(json, container, "formalGate"))
        {
            mofInteraction.formalGate.Add((UML.Interactions.Gate)mof);
        }

        // generalOrdering
        mofInteraction.generalOrdering.Clear();
        foreach (MofElement mof in JsonConvertor.FindMofArrayByXmiId(json, container, "generalOrdering"))
        {
            mofInteraction.generalOrdering.Add((UML.Interactions.GeneralOrdering)mof);
        }

        // name
        //mofInteraction.name = JsonConvertor.FindMofByXmiId(json, container, "name").ToString();
        mofInteraction.name = json.GetValue("name").ToString();

        // ownedElement
        mofInteraction.ownedElement.Clear();
        foreach (MofElement mof in JsonConvertor.FindMofArrayByXmiId(json, container, "ownedElement"))
        {
            mofInteraction.ownedElement.Add((UML.CommonStructure.Element)mof);
        }

        // ownedComment
        mofInteraction.ownedComment.Clear();
        foreach (MofElement mof in JsonConvertor.FindMofArrayByXmiId(json, container, "ownedComment"))
        {
            mofInteraction.ownedComment.Add((UML.CommonStructure.Comment)mof);
        }
        
    }
}

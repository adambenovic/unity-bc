using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Data;
using UML.Interactions;
using Data.MOF;
using MongoDB.Bson;

public class MessageFactory : AbstractFactory 
{
    public GameObject PrefabMessage = null;

    public override void RegisterFactory(FactoryRegister register)
    {
        register.RegisterXmiString("uml:Message", this);
        register.RegisterClass(typeof(UML.Interactions.Message), this);
        register.RegisterXmiString("uml:OccurrenceSpecification", this);
        register.RegisterClass(typeof(UML.Interactions.OccurrenceSpecification), this);
    }
    
    
    public override List<global::Data.MOF.MofElement> BuildMofFromJson(BsonDocument json, XmiCollection container)
    {
        List<global::Data.MOF.MofElement> result = new List<global::Data.MOF.MofElement>();

        switch ((string)json.GetValue("XmiType"))
        {
            case "uml:Message":
                UML.Interactions.Message mofMessage = new UML.Interactions.Message();
                mofMessage.XmiId = (string)json.GetValue("XmiId");
                container.AddMofElement(mofMessage);
                result.Add(mofMessage);
                break;
            case "uml:OccurrenceSpecification":
                UML.Interactions.OccurrenceSpecification mofOccurrenceSpecification = new UML.Interactions.OccurrenceSpecification();
                mofOccurrenceSpecification.XmiId = (string)json.GetValue("XmiId");
                container.AddMofElement(mofOccurrenceSpecification);
                result.Add(mofOccurrenceSpecification);
                break;
            default:
                Debug.Log("Invalid ELement!" + (string)json.GetValue("XmiType"));
                break;
        }

        return result;
    }

    public override void UpdateMofFromJson(BsonDocument json, XmiCollection container)
    {
        switch ((string)json.GetValue("XmiType"))
        {
            case "uml:Message":
                UML.Interactions.Message message = (UML.Interactions.Message)container.GetMofElement((string)json.GetValue("XmiId"));
                message.messageSort = (UML.Interactions.MessageSort)(json.GetValue("messageSort").AsInt32);
                message.owner = (UML.CommonStructure.Element)(JsonConvertor.FindMofByXmiId(json, container, "owner"));

                message.ownedElement.Clear();
                foreach (MofElement mof in JsonConvertor.FindMofArrayByXmiId(json, container, "ownedElement"))
                    message.ownedElement.Add((UML.CommonStructure.Element)mof);

                message.ownedComment.Clear();
                foreach (MofElement mof in JsonConvertor.FindMofArrayByXmiId(json, container, "ownedComment"))
                    message.ownedComment.Add((UML.CommonStructure.Comment)mof);

                message.name = json.GetValue("name").ToString();
                message.receiveEvent = (UML.Interactions.OccurrenceSpecification)JsonConvertor.FindMofByXmiId(json, container, "receiveEvent");
                message.sendEvent = (UML.Interactions.OccurrenceSpecification)JsonConvertor.FindMofByXmiId(json, container, "sendEvent");
                message.interaction = (UML.Interactions.Interaction)JsonConvertor.FindMofByXmiId(json, container, "interaction");

                break;
            case "uml:OccurrenceSpecification":
                UML.Interactions.OccurrenceSpecification occurrenceSpecification = (UML.Interactions.OccurrenceSpecification)container.GetMofElement((string)json.GetValue("XmiId"));
                occurrenceSpecification.owner = (UML.CommonStructure.Element)JsonConvertor.FindMofByXmiId(json, container, "owner");

                occurrenceSpecification.ownedElement.Clear();
                foreach (MofElement mof in JsonConvertor.FindMofArrayByXmiId(json, container, "ownedElement"))
                    occurrenceSpecification.ownedElement.Add((UML.CommonStructure.Element)mof);

                occurrenceSpecification.ownedComment.Clear();
                foreach (MofElement mof in JsonConvertor.FindMofArrayByXmiId(json, container, "ownedComment"))
                    occurrenceSpecification.ownedComment.Add((UML.CommonStructure.Comment)mof);

                occurrenceSpecification.name = json.GetValue("name").ToString();

                occurrenceSpecification.covered.Clear();
                foreach (MofElement mof in JsonConvertor.FindMofArrayByXmiId(json, container, "covered"))
                    occurrenceSpecification.covered.Add((UML.Interactions.Lifeline)mof);

                occurrenceSpecification.toBefore.Clear();
                foreach (MofElement mof in JsonConvertor.FindMofArrayByXmiId(json, container, "toBefore"))
                    occurrenceSpecification.toBefore.AddLast((UML.Interactions.GeneralOrdering)mof);

                occurrenceSpecification.toAfter.Clear();
                foreach (MofElement mof in JsonConvertor.FindMofArrayByXmiId(json, container, "toAfter"))
                    occurrenceSpecification.toAfter.AddLast((UML.Interactions.GeneralOrdering)mof);

                occurrenceSpecification.generalOrdering.Clear();
                foreach (MofElement mof in JsonConvertor.FindMofArrayByXmiId(json, container, "generalOrdering"))
                    occurrenceSpecification.generalOrdering.Add((UML.Interactions.GeneralOrdering)mof);

                occurrenceSpecification.enclosingInteraction = (UML.Interactions.Interaction)JsonConvertor.FindMofByXmiId(json, container, "enclosingInteraction");

                break;
            default:
                Debug.Log("Invalid Element!" + (string)json.GetValue("XmiType"));
                break;
        }
    }
}

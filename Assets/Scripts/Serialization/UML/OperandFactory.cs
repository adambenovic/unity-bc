using System.Collections.Generic;
using Data;
using Data.MOF;
using Data.DI;
using MongoDB.Bson;
using UnityEngine;

public class OperandFactory : AbstractFactory
{
    public GameObject PrefabOperand = null;

    public override void RegisterFactory(FactoryRegister register)
    {
        register.RegisterXmiString("uml:InteractionOperand", this);
        register.RegisterClass(typeof(UML.Interactions.InteractionOperand), this);
        register.RegisterXmiString("uml:InteractionConstraint", this);
        register.RegisterClass(typeof(UML.Interactions.InteractionConstraint), this);
        register.RegisterXmiString("uml:OpaqueExpression", this);
        register.RegisterClass(typeof(UML.Values.OpaqueExpression), this);
    }
    

    public override List<MofElement> BuildMofFromJson(BsonDocument json, XmiCollection container)
    {
        List<global::Data.MOF.MofElement> result = new List<global::Data.MOF.MofElement>();

        switch ((string)json.GetValue("XmiType"))
        {
            case "uml:InteractionOperand":
                UML.Interactions.InteractionOperand operand = new UML.Interactions.InteractionOperand();
                operand.XmiId = (string)json.GetValue("XmiId");
                container.AddMofElement(operand);
                result.Add(operand);
                break;
            case "uml:InteractionConstraint":
                UML.Interactions.InteractionConstraint interactionConstraint = new UML.Interactions.InteractionConstraint();
                interactionConstraint.XmiId = (string)json.GetValue("XmiId");
                container.AddMofElement(interactionConstraint);
                result.Add(interactionConstraint);
                break;
            case "uml:OpaqueExpression":
                UML.Values.OpaqueExpression opaqueExpression = new UML.Values.OpaqueExpression();
                opaqueExpression.XmiId = (string)json.GetValue("XmiId");
                container.AddMofElement(opaqueExpression);
                result.Add(opaqueExpression);
                break;
            default:
                Debug.Log("Invalid Element!" + (string)json.GetValue("XmiType"));
                break;
        }

        return result;
    }
public override void UpdateMofFromJson(BsonDocument json, XmiCollection container)
    {
        switch ((string)json.GetValue("XmiType"))
        {
            case "uml:InteractionOperand":
                UML.Interactions.InteractionOperand operand = (UML.Interactions.InteractionOperand)container.GetMofElement((string)json.GetValue("XmiId"));
                
                operand.covered.Clear();
                foreach (MofElement mof in JsonConvertor.FindMofArrayByXmiId(json, container, "covered"))
                    operand.covered.Add((UML.Interactions.Lifeline)mof);
                
                operand.diElement.Clear();
                foreach (DiElement di in JsonConvertor.FindDiArrayByXmiId(json, container, "diElement"))
                    operand.diElement.Add((DiElement)di);

                operand.enclosingInteraction = (UML.Interactions.Interaction)JsonConvertor.FindMofByXmiId(json, container, "enclosingInteraction");

                operand.enclosingOperand = (UML.Interactions.InteractionOperand)JsonConvertor.FindMofByXmiId(json, container, "enclosingOperand");

                operand.fragment.Clear();
                foreach (MofElement mof in JsonConvertor.FindMofArrayByXmiId(json, container, "fragment"))
                    operand.fragment.AddLast((UML.Interactions.InteractionFragment)mof);

                operand.generalOrdering.Clear();
                foreach (MofElement mof in JsonConvertor.FindMofArrayByXmiId(json, container, "generalOrdering"))
                    operand.generalOrdering.Add((UML.Interactions.GeneralOrdering)mof);

                operand.guard = (UML.Interactions.InteractionConstraint)JsonConvertor.FindMofByXmiId(json, container, "guard");

                operand.name = json.GetValue("name").ToString();

                operand.ownedComment.Clear();
                foreach (MofElement mof in JsonConvertor.FindMofArrayByXmiId(json, container, "ownedComment"))
                    operand.ownedComment.Add((UML.CommonStructure.Comment)mof);

                operand.ownedElement.Clear();
                foreach (MofElement mof in JsonConvertor.FindMofArrayByXmiId(json, container, "ownedElement"))
                    operand.ownedElement.Add((UML.CommonStructure.Element)mof);

                operand.owner = (UML.CommonStructure.Element)JsonConvertor.FindMofByXmiId(json, container, "owner");

                break;
            case "uml:InteractionConstraint":
                UML.Interactions.InteractionConstraint interactionConstraint = (UML.Interactions.InteractionConstraint)container.GetMofElement((string)json.GetValue("XmiId"));
                
                interactionConstraint.constrainedElement.Clear();
                foreach (MofElement mof in JsonConvertor.FindMofArrayByXmiId(json, container, "constrainedElement"))
                    interactionConstraint.constrainedElement.AddLast((UML.CommonStructure.Element)mof);

                interactionConstraint.diElement.Clear();
                foreach (DiElement di in JsonConvertor.FindDiArrayByXmiId(json, container, "diElement"))
                    interactionConstraint.diElement.Add((DiElement)di);

                interactionConstraint.name = json.GetValue("name").ToString();

                interactionConstraint.ownedComment.Clear();
                foreach (MofElement mof in JsonConvertor.FindMofArrayByXmiId(json, container, "ownedComment"))
                    interactionConstraint.ownedComment.Add((UML.CommonStructure.Comment)mof);

                interactionConstraint.ownedElement.Clear();
                foreach (MofElement mof in JsonConvertor.FindMofArrayByXmiId(json, container, "ownedElement"))
                    interactionConstraint.ownedElement.Add((UML.CommonStructure.Element)mof);

                interactionConstraint.owner = (UML.CommonStructure.Element)JsonConvertor.FindMofByXmiId(json, container, "owner");

                interactionConstraint.specification = (UML.Values.ValueSpecification)JsonConvertor.FindMofByXmiId(json, container, "specification");

                break;
            case "uml:OpaqueExpression":
                UML.Values.OpaqueExpression opaqueExpression = (UML.Values.OpaqueExpression)container.GetMofElement((string)json.GetValue("XmiId"));

                opaqueExpression.body = json.GetValue("body").ToString();

                opaqueExpression.diElement.Clear();
                foreach (DiElement di in JsonConvertor.FindDiArrayByXmiId(json, container, "diElement"))
                    opaqueExpression.diElement.Add((DiElement)di);

//                opaqueExpression.language = json.GetValue("language").ToString();

                opaqueExpression.name = json.GetValue("name").ToString();
                Debug.Log("The nejm iz: " + opaqueExpression.name);

                opaqueExpression.ownedComment.Clear();
                foreach (MofElement mof in JsonConvertor.FindMofArrayByXmiId(json, container, "ownedComment"))
                    opaqueExpression.ownedComment.Add((UML.CommonStructure.Comment)mof);

                opaqueExpression.ownedElement.Clear();
                foreach (MofElement mof in JsonConvertor.FindMofArrayByXmiId(json, container, "ownedElement"))
                    opaqueExpression.ownedElement.Add((UML.CommonStructure.Element)mof);

                opaqueExpression.owner = (UML.CommonStructure.Element)JsonConvertor.FindMofByXmiId(json, container, "owner");

                break;
            default:
                Debug.Log("Invalid ELement! " + (string)json.GetValue("XmiType"));
                break;
        }
        //return result;
    }
}

using System.Collections.Generic;
using Data;
using Data.DI;
using Data.MOF;
using UML.Interactions;
using MongoDB.Bson;
using UnityEngine;

public class FragmentFactory : AbstractFactory
{
    public GameObject PrefabFragment = null;

    public override void RegisterFactory(FactoryRegister register)
    {
        register.RegisterXmiString("uml:CombinedFragment", this);
        register.RegisterClass(typeof(UML.Interactions.CombinedFragment), this);
    }

    public override List<global::Data.MOF.MofElement> BuildMofFromJson(BsonDocument json, XmiCollection container)
    {
        List<global::Data.MOF.MofElement> result = new List<global::Data.MOF.MofElement>();

        switch ((string)json.GetValue("XmiType"))
        {
            case "uml:CombinedFragment":
                UML.Interactions.CombinedFragment mofFragment = new UML.Interactions.CombinedFragment();
                mofFragment.XmiId = (string)json.GetValue("XmiId");
                container.AddMofElement(mofFragment);
                result.Add(mofFragment);
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
            case "uml:CombinedFragment":
                UML.Interactions.CombinedFragment fragment = (UML.Interactions.CombinedFragment)container.GetMofElement((string)json.GetValue("XmiId"));

                fragment.cfragmentGate.Clear();
                foreach (MofElement mof in JsonConvertor.FindMofArrayByXmiId(json, container, "cfragmentGate"))
                    fragment.cfragmentGate.Add((UML.Interactions.Gate)mof);

                fragment.covered.Clear();
                foreach (MofElement mof in JsonConvertor.FindMofArrayByXmiId(json, container, "covered"))
                    fragment.covered.Add((UML.Interactions.Lifeline)mof);

                fragment.diElement.Clear();
                foreach (DiElement di in JsonConvertor.FindDiArrayByXmiId(json, container, "diElement"))
                    fragment.diElement.Add((DiElement)di);

                fragment.enclosingInteraction = (UML.Interactions.Interaction)JsonConvertor.FindMofByXmiId(json, container, "enclosingInteraction");

                fragment.enclosingOperand = (UML.Interactions.InteractionOperand)JsonConvertor.FindMofByXmiId(json, container, "enclosingOperand");

                fragment.generalOrdering.Clear();
                foreach (MofElement mof in JsonConvertor.FindMofArrayByXmiId(json, container, "generalOrdering"))
                    fragment.generalOrdering.Add((UML.Interactions.GeneralOrdering)mof);

                fragment.interactionOperator = (UML.Interactions.InteractionOperatorKind)(json.GetValue("interactionOperator").AsInt32);

                fragment.name = json.GetValue("name").ToString();

                fragment.operand.Clear();
                foreach (MofElement mof in JsonConvertor.FindMofArrayByXmiId(json, container, "operand"))
                    fragment.operand.Add((UML.Interactions.InteractionOperand)mof);

                fragment.ownedComment.Clear();
                foreach (MofElement mof in JsonConvertor.FindMofArrayByXmiId(json, container, "ownedComment"))
                    fragment.ownedComment.Add((UML.CommonStructure.Comment)mof);

                fragment.ownedElement.Clear();
                foreach (MofElement mof in JsonConvertor.FindMofArrayByXmiId(json, container, "ownedElement"))
                    fragment.ownedElement.Add((UML.CommonStructure.Element)mof);

                fragment.owner = (UML.CommonStructure.Element)JsonConvertor.FindMofByXmiId(json, container, "owner");

                break;
            default:
                Debug.Log("Invalid ELement! " + (string)json.GetValue("XmiType"));
                break;
        }
    }
    
}

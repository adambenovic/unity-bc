using Data.DI;
using Data.MOF;
using System.Collections.Generic;
using UnityEngine;

namespace Data
{
    /// <summary>
    /// Semantic and graphic elements use same XMI ID space.
    /// To prevent existence of MOF and DI element with same ID,
    /// both of them are redundantly stored in single collection.
    /// </summary>
    /// 
    public class XmiCollection
    {
        private Dictionary<string, XmiElement> allElements = new Dictionary<string, XmiElement>();
        private Dictionary<string, MofElement> mofElements = new Dictionary<string, MofElement>();
        private Dictionary<string,  DiElement> diElements  = new Dictionary<string,  DiElement>();

        //AddForMofElementsArray
        public int Add(MofElement[] otherMofElementsArray)
        {
            int count = 0;

            foreach (MofElement element in otherMofElementsArray)
            {
                if (AddMofElement(element))
                {
                    count++;
                }
            }

            return count;
        }

        //AddForDiElementsArray
        public int Add(DiElement[] otherDiElementsArray)
        {
            int count = 0;

            foreach (DiElement element in otherDiElementsArray)
            {
                if (AddDiElement(element))
                {
                    count++;
                }
            }

            return count;
        }

        //AddRangeForMofElementsList
        public int Add(List<MofElement> otherMofElementsList)
        {
            int count = 0;

            foreach (var element in otherMofElementsList)
            {
                if (AddMofElement(element))
                {
                    count++;
                }
            }

            return count;
        }

        //AddRangeForDiElementsList
        public int Add(List<DiElement> otherDiElementsList)
        {
            int count = 0;

            foreach (var element in otherDiElementsList)
            {
                if (AddDiElement(element))
                {
                    count++;
                }
            }

            return count;
        }

        //AddRangeForMofElements
        public int Add(Dictionary<string, MofElement> otherMofElements)
        {
            int count = 0;

            foreach (KeyValuePair<string, MofElement> entry in otherMofElements)
            {
                if (AddMofElement(entry.Value))
                {
                    count++;
                }
               
            }

            return count;
        }

        //AddRangeForDiElements
        public int Add(Dictionary<string, DiElement> otherDiElements)
        {
            int count = 0;

            foreach (KeyValuePair<string, DiElement> entry in otherDiElements)
            {
                if (AddDiElement(entry.Value))
                {
                    count++;
                }
                
            }
            return count;
        }

        //AddRangeForContainers
        public int Add(XmiCollection container)
        {
            int count = Add(container.mofElements);
            int count2 = Add(container.diElements);

            return (count + count2);
        }

        public static XmiCollection ProjectContainer()
        {
            return GameObject.Find("Project").GetComponent<XmiCollection>();
        }

        public bool AddMofElement(MofElement mofElement)
        {
            if (allElements.ContainsKey(mofElement.XmiId))
                return false;   // TODO Throw Exception instead
            allElements.Add(mofElement.XmiId, mofElement);
            mofElements.Add(mofElement.XmiId, mofElement);
            return true;
        }

        public bool AddDiElement(DiElement diElement)
        {
            if (allElements.ContainsKey(diElement.XmiId))
                return false;   // TODO Throw Exception instead
            allElements.Add(diElement.XmiId, diElement);
            diElements.Add(diElement.XmiId, diElement);
            return true;
        }

      
        public MofElement GetMofElement(string xmi_id)
        {
            if (mofElements.ContainsKey(xmi_id))
                return mofElements[xmi_id];
            return null;
        }

        public DiElement GetDiElement(string xmi_id)
        {
            if (diElements.ContainsKey(xmi_id))
                return diElements[xmi_id];
            return null;
        }
    
        public bool RemoveMofElement(MofElement mofElement)
        {
            if (!allElements.ContainsKey(mofElement.XmiId))
                return false;   // TODO Throw Exception instead
            allElements.Remove(mofElement.XmiId);
            mofElements.Remove(mofElement.XmiId);
            return true;
        }

        public bool RemoveDiElement(DiElement diElement)
        {
            if (!allElements.ContainsKey(diElement.XmiId))
                return false;   // TODO Throw Exception instead
            allElements.Remove(diElement.XmiId);
            diElements.Remove(diElement.XmiId);
            return true;
        }

        public bool RemoveMofElement(string mofElementString)
        {
            MofElement mofElement = GetMofElement(mofElementString);

            
            if (mofElement == null)
                return false;   // TODO Throw Exception instead

            allElements.Remove(mofElement.XmiId);
            mofElements.Remove(mofElement.XmiId);

            return true;
        }

        public bool RemoveDiElement(string diElementString)
        {
            DiElement diElement = GetDiElement(diElementString);
            if (diElement == null)
                return false;   // TODO Throw Exception instead

            allElements.Remove(diElement.XmiId);
            mofElements.Remove(diElement.XmiId);

            return true;
        }

        public Dictionary<string, MofElement> MofElements()
        {
            return new Dictionary<string, MofElement>(mofElements);
        }

        public Dictionary<string, DiElement> DiElements()
        {
            return new Dictionary<string, DiElement>(diElements);
        }

    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Data.MOF;

namespace Data.DI
{
    public abstract class DiElement : XmiElement
    {
        public readonly List<MofElement> mofElement = new List<MofElement>();
        public GameObject dgElement = null;

        public DiElement owningElement = null;
    }
}

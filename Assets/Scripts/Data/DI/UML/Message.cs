using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Data.DI.UML
{
    public class Message : DiElement2D
    {
        public override string XmiType()
        {
            return "uml:Message";
        }
    }
}

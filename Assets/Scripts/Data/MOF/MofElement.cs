using System;
using System.Collections.Generic;
using System.Text;

using Data.DI;

namespace Data.MOF
{
    public abstract class MofElement : XmiElement
    {
        public readonly List<DiElement> diElement = new List<DiElement>();

    }
}

using System.Collections.Generic;

namespace Data.DI
{
    public class DiElement3D : DiElement
    {
        public readonly List<DiElement> ownedElement = new List<DiElement>();
    }
}

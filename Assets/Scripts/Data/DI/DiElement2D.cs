using System.Collections.Generic;

namespace Data.DI
{
    public abstract class DiElement2D : DiElement
    {
        public readonly List<DiElement2D> ownedElement = new List<DiElement2D>();
    }
}

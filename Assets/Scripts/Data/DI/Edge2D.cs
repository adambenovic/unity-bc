using System.Collections.Generic;
using UnityEngine;

namespace Data.DI
{
    public class Edge2D : DiElement2D
    {
        public readonly List<Vector2> waypoint = new List<Vector2>();
    }
}

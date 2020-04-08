using System.Collections.Generic;
using UnityEngine;

namespace Data.DI
{
    public class Edge3D : DiElement3D
    {
        public readonly List<Vector3> waypoint = new List<Vector3>();
    }
}

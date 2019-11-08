using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;

public class Extruder : MonoBehaviour
{
    public float m_Height = 5f;
    public bool m_FlipNormals = false;

    public void Extrude(Vector3[] points)
    {
        var m_Mesh = GetComponent<ProBuilderMesh>();
        m_Mesh.CreateShapeFromPolygon(points, m_Height, m_FlipNormals);
        m_Mesh.CenterPivot(GetComponent<MeshFilter>().sharedMesh.GetIndices(0));
    }
}

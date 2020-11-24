using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KaplaCSG;

public class HalfEdgeDebug : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // Get the mesh
        MeshFilter mf = this.gameObject.GetComponent<MeshFilter>();
        Mesh m = mf.mesh;

        List<HEFace> faces = new List<HEFace>();
        List<HEVertex> vertices = new List<HEVertex>();
        List<HalfEdge> halfEdges = new List<HalfEdge>();

        // Build the HalfEdge data structure from the mesh!
        HalfEdge.CreateStructureFromMesh(m, halfEdges, faces, vertices);
        mf.mesh = HalfEdge.CreateMeshFromHalfEdge(faces, vertices, halfEdges);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KaplaCSG;

public class HalfEdgeDebug : MonoBehaviour
{
    List<Vector3> iPoints = new List<Vector3>();
    // Start is called before the first frame update
    void Start()
    {
        // Get the mesh
        MeshFilter mf = this.gameObject.GetComponent<MeshFilter>();
        Mesh m = mf.mesh;


        // Build the HalfEdge data structure from the mesh!
        //HalfEdge.CreateStructureFromMesh(m, halfEdges, faces, vertices);
        HalfEdgeMesh heMesh = new HalfEdgeMesh(m);
        
        // Lets try to cut it with a plane, then draw the gizmos of the intersection points
        bool[] visited = new bool[heMesh.halfEdges.Count];
        Plane plane = new Plane(new Vector3(1.0f, 2.0f, 0.0f).normalized, new Vector3(0.0f, 0.0f, 0.0f));
        List<HalfEdge> newHalfEdges = new List<HalfEdge>();
        int s = heMesh.halfEdges.Count;
        for (int i = 0; i < s; i++)
        {
            if (visited[i])
                continue;
            // mark this and the opposite as visited
            HalfEdge he = heMesh.halfEdges[i];
            HalfEdge heOpp = heMesh.halfEdges[he.oppositeIndex];
            visited[he.index] = true;
            visited[heOpp.index] = true;

            // Check if intersection with plane
            Vector3 v0 = heMesh.vertices[he.verIndex].v;
            Vector3 v1 = heMesh.vertices[heOpp.verIndex].v;

            // First Set vertex plane config
            Vector3 c0 = v0 - plane.point;
            Vector3 c1 = v1 - plane.point;
            float c0_dot = Vector3.Dot(c0, plane.normal);
            float c1_dot = Vector3.Dot(c1, plane.normal);
            if (Mathf.Abs(c0_dot) < 0.00001f)
            {
                heMesh.vertices[he.verIndex].config = PlaneConfig.On;
            } else if (c0_dot <= -0.00001f)
            {
                heMesh.vertices[he.verIndex].config = PlaneConfig.Right;
            }
            else if (c0_dot >= 0.00001f)
            {
                heMesh.vertices[he.verIndex].config = PlaneConfig.Left;
            }
            if (Mathf.Abs(c1_dot) < 0.00001f)
            {
                heMesh.vertices[heOpp.verIndex].config = PlaneConfig.On;
            }
            else if (c1_dot <= -0.00001f)
            {
                heMesh.vertices[heOpp.verIndex].config = PlaneConfig.Right;
            }
            else if (c1_dot >= 0.00001f)
            {
                heMesh.vertices[heOpp.verIndex].config = PlaneConfig.Left;
            }

            float t;
            Vector3 iPoint = Plane.LinePlaneIntersect(plane, v0, v1, out t);
            if (t > 1.0f - 0.00001f|| t < 0.0f + 0.00001f)
            {
                // No intersection on line segment OR parallel with plane
                continue;
            }
            Debug.Log(t); 
            // add new intersection vertex to half-edge structure
            HEVertex iVert = new HEVertex();
            iVert.v = iPoint;
            iVert.heIndex = (short)heMesh.halfEdges.Count;
            iVert.config = PlaneConfig.On;
            heMesh.vertices.Add(iVert);
            HalfEdge[] newHEs = HalfEdge.CreateFromTwo(he, heOpp,(short)heMesh.halfEdges.Count,(short)(heMesh.vertices.Count - 1));
            heMesh.halfEdges.Add(newHEs[0]);
            heMesh.halfEdges.Add(newHEs[1]);

            iPoints.Add(iPoint);
        }

        heMesh.Triangulate();
        // TODO:
        // Split in two and triangulate the cap!

         mf.mesh = heMesh.GetMesh();
        //  mf.mesh = HalfEdge.CreateMeshFromHalfEdge(faces, vertices, halfEdges);
        //  mf.mesh.RecalculateNormals(); // Doesn't smooth the mesh :)

        // Test p3 intersection
        Plane p1 = new Plane(new Vector3(1.0f, 0.0f, 0.0f), new Vector3(0.0f, 0.0f, 0.0f));
        Plane p2 = new Plane(new Vector3(0.0f, 1.0f, 0.0f), new Vector3(0.0f, 5.0f, 0.0f));
        Plane p3 = new Plane(new Vector3(0.0f, 0.0f, 1.0f), new Vector3(0.0f, 0.0f, 3.0f));
        Vector3 inter = Plane.PlanePlanePlaneIntersect(p1, p2, p3);
        Debug.Log("intersect point = " + inter);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        foreach (Vector3 p in iPoints)
        {
          //  Gizmos.DrawSphere(gameObject.transform.TransformPoint(p), 0.05f);
          
        }

    }
}

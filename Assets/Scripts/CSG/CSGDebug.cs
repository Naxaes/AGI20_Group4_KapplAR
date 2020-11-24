using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KaplaCSG;
public class CSGDebug : MonoBehaviour
{
    /*
    List<HalfEdge> halfEdges;
    short polygonIndex = 0;
    List<Vector3> vertices = new List<Vector3>();
    List<int> vertexIndices = new List<int>();
    Vector3 clipPlaneN = new Vector3(1.0f, -1.0f, 0.0f);
    Vector3 clipPlaneP = new Vector3(0.5f, 0.0f, 0.0f);

    Mesh m;
    MeshFilter mf;
    // Use this for initialization
    void Start()
    {
        clipPlaneN = clipPlaneN.normalized;
        mf = this.gameObject.GetComponent<MeshFilter>();
        m = new Mesh();
        mf.mesh = m;

        drawTriangle();
        Intersect();
    }

    List<short> Intersect()
    {
        List<short> iIndices = new List<short>();
        bool[] visited = new bool[halfEdges.Count];

        List<HalfEdge> newHEs = new List<HalfEdge>();
        short c = (short)halfEdges.Count;
        // Find where our triangle intersect the plane
        for (int i = 0; i < c; i++)
        {
            HalfEdge he = halfEdges[i];

            if (visited[he.index])
                continue;
            visited[he.index] = true;

            float t;
            Vector3 e = PlaneEdgeIntersect(vertices[he.vertexIndex], vertices[halfEdges[he.oppositeIndex].vertexIndex], out t);
            Debug.Log("he = " + he.index + ". e = " + e + ". t = " + t + "\n p0 = " + vertices[he.vertexIndex] + ". p1 = " + vertices[halfEdges[he.oppositeIndex].vertexIndex]);
            if (0.0f <= t && t <= 1.0f)
            {
                // Intersection
                // Add new vertex and create pair of halfedges for it
                // Mark it as visited first! Since opposite index is updated
                visited[he.oppositeIndex] = true;
                HalfEdge[] h = HalfEdge.CreateFromTwo(he, halfEdges[he.oppositeIndex], (short)(halfEdges.Count), (short)vertices.Count);
                // two new
                halfEdges.Add(h[0]);
                halfEdges.Add(h[1]);
                vertices.Add(e);
                vertexIndices.Add(vertices.Count - 1);
                Debug.Log(he.oppositeIndex);
            }
        }

        foreach (HalfEdge he in halfEdges)
        {
            Debug.Log("he = " + he.index + " vert = " + vertices[he.vertexIndex]);
        }
        // add new HEs
        foreach (HalfEdge h in newHEs)
        {
            halfEdges.Add(h);
        }

        return iIndices;
    }

    // t:   < 0.0 behind p0, > 1.0 infornt of p1
    Vector3 PlaneEdgeIntersect(Vector3 p0, Vector3 p1, out float t)
    {
        // dot(n, P - P_n) = 0 -> P = P0 + t(P1 - P0) -> dot(n, P0 + t(P1 - P0) - Pn) = 0, solve for t
        Vector3 e = p1 - p0;
        float d = Vector3.Dot(clipPlaneN, e);
        if(Mathf.Abs(d) > Mathf.Epsilon)
        {
            Vector3 w = p0 - clipPlaneP;
            t = -Vector3.Dot(clipPlaneN, w) / d; // Where we are on the line segment
            e *= t;
            return p0 + e;
        }
        t = float.MaxValue;
        return e;
    }

    //This draws a triangle
    void drawTriangle()
    {
        //We need two arrays one to hold the vertices and one to hold the triangles

        //lets add 3 vertices in the 3d space
        vertices.Add(new Vector3(0, 0, 0));
        vertices.Add(new Vector3(1, 0, 0));
        vertices.Add(new Vector3(0, 1, 0));
        //define the order in which the vertices in the VerteicesArray shoudl be used to draw the triangle
        vertexIndices.Add(0);
        vertexIndices.Add(1);
        vertexIndices.Add(2);
        //add these two triangles to the mesh
        m.vertices = vertices.ToArray();
        m.triangles = vertexIndices.ToArray();

        // Lets build the halfEdge array given the triangle
        // At most the complete graph so length^2 is an upper bound
        // Need a method to go from vertex data to halfedge index
        halfEdges = new List<HalfEdge>();
        // There are three edges
        // v0 - v1, v0 - v2, v1 - v2. In counterclockvise order V0 V1 V2
        HalfEdge h0_0 = new HalfEdge();
        halfEdges.Add(h0_0);
        HalfEdge h0_1 = new HalfEdge();
        halfEdges.Add(h0_1);
        HalfEdge h1_0 = new HalfEdge();
        halfEdges.Add(h1_0);
        HalfEdge h1_1 = new HalfEdge();
        halfEdges.Add(h1_1);
        HalfEdge h2_0 = new HalfEdge();
        halfEdges.Add(h2_0);
        HalfEdge h2_1 = new HalfEdge();
        halfEdges.Add(h2_1);
        h0_0.vertexIndex = 0;
        h0_1.vertexIndex = 0;
        h1_0.vertexIndex = 1;
        h1_1.vertexIndex = 1;
        h2_0.vertexIndex = 2;
        h2_1.vertexIndex = 2;

        h0_0.index = 0;
        h0_1.index = 1;
        h1_0.index = 2;
        h1_1.index = 3;
        h2_0.index = 4;
        h2_1.index = 5;

        h0_0.nextIndex = 1;
        h0_1.nextIndex = 0;

        h1_0.nextIndex = 3;
        h1_1.nextIndex = 2;

        h2_0.nextIndex = 5;
        h2_1.nextIndex = 4;

        h0_0.oppositeIndex = h1_1.index;
        h1_1.oppositeIndex = h0_0.index;

        h1_0.oppositeIndex = h2_1.index;
        h2_1.oppositeIndex = h1_0.index;

        h2_0.oppositeIndex = h0_1.index;
        h0_1.oppositeIndex = h2_0.index;

        h0_0.polygonIndex = polygonIndex;
        h0_1.polygonIndex = polygonIndex;
        h1_0.polygonIndex = polygonIndex;
        h1_1.polygonIndex = polygonIndex;
        h2_0.polygonIndex = polygonIndex;
        h2_1.polygonIndex = polygonIndex;
    }

    void OnDrawGizmosSelected()
    {
        // Draw a yellow sphere at the transform's position
        Gizmos.color = Color.yellow;
        foreach (Vector3 v in vertices)
            Gizmos.DrawSphere(v, 0.1f);
    }
    */
}

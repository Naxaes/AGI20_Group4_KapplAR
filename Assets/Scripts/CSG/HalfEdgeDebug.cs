using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KaplaCSG;

public class HalfEdgeDebug : MonoBehaviour
{
    List<Vector3> iPoints = new List<Vector3>();
    public MeshFilter meshFilter;
    Mesh mesh;
    public HalfEdgeMesh heMesh;

    public void InitHalfEdgeMesh()
    {
        heMesh = new HalfEdgeMesh(mesh);
    }

    public void CutWithPlane(Plane plane)
    {
        bool[] visited = new bool[heMesh.halfEdges.Count];
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
            float eps = 0.000001f;
            if (Mathf.Abs(c0_dot) < eps)
            {
                heMesh.vertices[he.verIndex].config = PlaneConfig.On;
            }
            else if (c0_dot <= -eps)
            {
                heMesh.vertices[he.verIndex].config = PlaneConfig.Right;
            }
            else if (c0_dot >= eps)
            {
                heMesh.vertices[he.verIndex].config = PlaneConfig.Left;
            }
            if (Mathf.Abs(c1_dot) < eps)
            {
                heMesh.vertices[heOpp.verIndex].config = PlaneConfig.On;
            }
            else if (c1_dot <= -eps)
            {
                heMesh.vertices[heOpp.verIndex].config = PlaneConfig.Right;
            }
            else if (c1_dot >= eps)
            {
                heMesh.vertices[heOpp.verIndex].config = PlaneConfig.Left;
            }

            float t;
            Vector3 iPoint = Plane.LinePlaneIntersect(plane, v0, v1, out t);
            if (t > 1.0f - eps || t < 0.0f + eps)
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
            HalfEdge[] newHEs = HalfEdge.CreateFromTwo(he, heOpp, (short)heMesh.halfEdges.Count, (short)(heMesh.vertices.Count - 1));
            heMesh.halfEdges.Add(newHEs[0]);
            heMesh.halfEdges.Add(newHEs[1]);

            iPoints.Add(iPoint);
        }

        heMesh.Triangulate();
        HalfEdgeMesh rightMesh = new HalfEdgeMesh();
        HalfEdgeMesh leftMesh = new HalfEdgeMesh();
        heMesh.SplitInLeftAndRightMesh(leftMesh, rightMesh);

        GameObject copy = Instantiate(gameObject);
        HalfEdgeDebug copyDebug = copy.GetComponent<HalfEdgeDebug>();

        rightMesh.CapClipPlane(plane.normal);
        copyDebug.heMesh = rightMesh;
        copyDebug.meshFilter.mesh = rightMesh.GetMesh();
        //  MeshFilter mfCpy = copy.GetComponent<MeshFilter>();
        leftMesh.CapClipPlane(-plane.normal);
        meshFilter.mesh = leftMesh.GetMesh();

        heMesh.CreateStructureFromMesh(mesh);
      //  copyDebug.heMesh.CreateStructureFromMesh(copyDebug.meshFilter.mesh);

    }

    // Start is called before the first frame update
    void Start()
    {
        // Get the mesh
        meshFilter = this.gameObject.GetComponent<MeshFilter>();
        mesh = meshFilter.mesh;
        InitHalfEdgeMesh();
    }



    float r = 0.0f;
    float timer = 0.0f;
    // Update is called once per frame
    void Update()
    {
        if (timer > 0.0f)
        {
            timer -= Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.A) && timer <= 0.0f)
        {
            Debug.Log("Do cut ayy");
            Plane p = new Plane(new Vector3(1.0f ,1.0f + r, 0.0f).normalized, new Vector3(0.0f, 0.0f, 0.0f));
            CutWithPlane(p);
            r += 1.0f;
            timer = 1.0f;
        }
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

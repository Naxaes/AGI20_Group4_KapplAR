using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KaplaCSG;

public class SliceableWithPlane : MonoBehaviour
{
    List<Vector3> iPoints = new List<Vector3>();
    MeshFilter meshFilter;
    Mesh mesh;
    HalfEdgeMesh heMesh;
    public GameObject objectToCopy;
    public GameObject cutPlane;

    public void InitHalfEdgeMesh()
    {
        heMesh = new HalfEdgeMesh(mesh);
    }

    public void CutWithPlane(Plane plane)
    {
       
        float sTime = Time.realtimeSinceStartup;
        GameObject child = gameObject.transform.GetChild(0).gameObject;
        meshFilter = child.GetComponent<MeshFilter>();
        mesh = meshFilter.mesh;
        InitHalfEdgeMesh();
        bool[] visited = new bool[heMesh.halfEdges.Count];
        List<HalfEdge> newHalfEdges = new List<HalfEdge>();
        int s = heMesh.halfEdges.Count;
        int rightCount = 0;
        int leftCount = 0;
        Vector3 newVertCoord = Vector3.zero;
        int onCount = 0;
        /*
        Dictionary<Vector3, Vector2> uvMap = new Dictionary<Vector3, Vector2>();
        List<Vector2> uvs = new List<Vector2>();
        mesh.GetUVs(0, uvs);
        for (int i = 0; i < mesh.vertices.Length; i++)
        {
            if (!uvMap.ContainsKey(mesh.vertices[i])) {
                uvMap.Add(mesh.vertices[i], uvs[i]);
            }
        }*/
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
            float eps = 0.0001f;
            if (Mathf.Abs(c0_dot) < eps)
            {
                heMesh.vertices[he.verIndex].config = PlaneConfig.On;
            }
            else if (c0_dot <= -eps)
            {
                heMesh.vertices[he.verIndex].config = PlaneConfig.Right;
                rightCount++;
            }
            else if (c0_dot >= eps)
            {
                heMesh.vertices[he.verIndex].config = PlaneConfig.Left;
                leftCount++;
            }
            if (Mathf.Abs(c1_dot) < eps)
            {
                heMesh.vertices[heOpp.verIndex].config = PlaneConfig.On;
            }
            else if (c1_dot <= -eps)
            {
                rightCount++;
                heMesh.vertices[heOpp.verIndex].config = PlaneConfig.Right;
            }
            else if (c1_dot >= eps)
            {
                leftCount++;
                heMesh.vertices[heOpp.verIndex].config = PlaneConfig.Left;
            }

            float t;
            Vector3 iPoint = Plane.LinePlaneIntersect(plane, v0, v1, out t);
            if (t > 1.0f - eps || t < 0.0f + eps)
            {
                // No intersection on line segment OR parallel with plane
                continue;
            }
            // Debug.Log(t);
            // add new intersection vertex to half-edge structure
            HEVertex iVert = new HEVertex();
            iVert.v = iPoint;
            onCount += 1;
            newVertCoord += iPoint;
            iVert.heIndex = (short)heMesh.halfEdges.Count;
            iVert.config = PlaneConfig.On;
            heMesh.vertices.Add(iVert);
            HalfEdge[] newHEs = HalfEdge.CreateFromTwo(he, heOpp, (short)heMesh.halfEdges.Count, (short)(heMesh.vertices.Count - 1));
            heMesh.halfEdges.Add(newHEs[0]);
            heMesh.halfEdges.Add(newHEs[1]);
            iPoints.Add(iPoint);
        }
        if (leftCount == 0 || rightCount == 0)
        {
            // Do nothing!!!!
            return;
        }
        heMesh.Triangulate();
        HalfEdgeMesh rightMesh = new HalfEdgeMesh();
        HalfEdgeMesh leftMesh = new HalfEdgeMesh();
        heMesh.SplitInLeftAndRightMesh(leftMesh, rightMesh);

        GameObject copy = Instantiate(objectToCopy);
        GameObject childCopy = copy.transform.GetChild(0).gameObject;
        SliceableWithPlane copyDebug = copy.GetComponent<SliceableWithPlane>();

        newVertCoord /= (float)onCount;

        rightMesh.CapClipPlane(plane.normal, newVertCoord);
        copyDebug.heMesh = rightMesh;
        copyDebug.meshFilter =  childCopy.GetComponent<MeshFilter>();
        copyDebug.meshFilter.mesh = rightMesh.GetMesh();
        //  MeshFilter mfCpy = copy.GetComponent<MeshFilter>();
        leftMesh.CapClipPlane(-plane.normal, newVertCoord);
        meshFilter.mesh = leftMesh.GetMesh();
        /*
        List<Vector2> newUVs = new List<Vector2>();
        foreach (Vector3 v in meshFilter.mesh.vertices)
        {
            if (!uvMap.ContainsKey(v))
            {
                newUVs.Add(Vector2.zero);
            } else
            {
                newUVs.Add(uvMap[v]);
            }
        }
        meshFilter.mesh.SetUVs(0, newUVs);
        */
        copyDebug.cutPlane = cutPlane;
        //heMesh.CreateStructureFromMesh(mesh);
        //copyDebug.heMesh.CreateStructureFromMesh(copyDebug.meshFilter.mesh);
        child.GetComponent<MeshCollider>().sharedMesh = null;
        child.GetComponent<MeshCollider>().sharedMesh = meshFilter.mesh;
        
        childCopy.GetComponent<MeshCollider>().sharedMesh = null;
        childCopy.GetComponent<MeshCollider>().sharedMesh = copyDebug.meshFilter.mesh;
        //  child.GetComponent<MeshCollider>().gameObject.SetActive(true);
        float eTime = Time.realtimeSinceStartup - sTime;
        Debug.Log("Time taken = " + eTime);
    }

    // Start is called before the first frame update
    void Start()
    {
        // Get the mesh
        //  meshFilter = this.gameObject.GetComponent<MeshFilter>();
        //  mesh = meshFilter.mesh;
        //  InitHalfEdgeMesh();
    }



    float r = 0.0f;
    float timer = 1.0f;
    // Update is called once per frame
    public Plane p;
    void Update()
    {
        if (timer > 0.0f)
        {
            timer -= Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.A) && timer <= 0.0f)
        {
            GameObject child = gameObject.transform.GetChild(0).gameObject;
            p = new Plane(child.transform.InverseTransformDirection(cutPlane.transform.up).normalized, child.transform.InverseTransformPoint(cutPlane.transform.position));
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
            //    Gizmos.DrawSphere(gameObject.transform.TransformPoint(p), 0.05f);

        }

    }
}

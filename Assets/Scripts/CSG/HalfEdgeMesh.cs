﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KaplaCSG;

public class HalfEdgeMesh
{
    public List<HEFace> faces = new List<HEFace>();
    public List<HEVertex> vertices = new List<HEVertex>();
    public List<HalfEdge> halfEdges = new List<HalfEdge>();

    public HalfEdgeMesh(Mesh mesh)
    {
        CreateStructureFromMesh(mesh);
    }

    public HalfEdgeMesh()
    {

    }

    public void CreateStructureFromMesh(Mesh mesh)
    {
        int[] meshTriangles = mesh.triangles;
        Vector3[] meshVertices = mesh.vertices;
        Vector3[] meshNormals = mesh.normals;

        // One face per triangle. Add normals
        for (int i = 0; i < meshTriangles.Length; i += 3)
        {
            HEFace f = new HEFace();
            // Take the mean of the triangle normals
            Vector3 n = meshNormals[meshTriangles[i]] + meshNormals[meshTriangles[i + 1]] + meshNormals[meshTriangles[i + 2]];
            n = n / 3.0f;
            f.normal = n.normalized;
            faces.Add(f);
        }

        // Simplify mesh vertices i.e. remove duplicates.
        // There are duplicate vertices since they have different normals
        // Use a dictionary to store the vertices and their new indices
        Dictionary<Vector3, int> d = new Dictionary<Vector3, int>();
        List<Vector3> newVerts = new List<Vector3>();
        List<int> newTris = new List<int>();
        int newIdx = 0;
        for (int i = 0; i < meshVertices.Length; i++)
        {
            if (!d.ContainsKey(meshVertices[i]))
            {
                d[meshVertices[i]] = newIdx;
                newVerts.Add(meshVertices[i]);
                newIdx++;
            }
        }
        // Update the triangle indices to the new vertex indices.
        for (int i = 0; i < meshTriangles.Length; i++)
        {
            newTris.Add(d[meshVertices[meshTriangles[i]]]);
        }
        // Set to updated values
        meshVertices = newVerts.ToArray();
        meshTriangles = newTris.ToArray();

        // Init the vertices with vertex data, but not half-edge
        for (int i = 0; i < meshVertices.Length; i++)
        {
            HEVertex vert = new HEVertex
            {
                v = meshVertices[i]
            };
            vertices.Add(vert);
        }

        // Dictionary for opposite lookup. Build in first loop over triangles, used in the second.
        // key is v0v1
        Dictionary<string, List<short>> edgeMap = new Dictionary<string, List<short>>();

        // Start with creating the halfedges and filling in everything except oppositite
        for (int i = 0; i < meshTriangles.Length; i += 3)
        {
            // Stored 3-by-3 indices. e.g. 0,1,2 forms the first triangle.
            short i0 = (short)i;
            short i1 = (short)(i + 1);
            short i2 = (short)(i + 2);

            // Triangles are in clockwise order in Unity
            HalfEdge h0 = new HalfEdge();
            HalfEdge h1 = new HalfEdge();
            HalfEdge h2 = new HalfEdge();

            h0.index = i0;
            h1.index = i1;
            h2.index = i2;

            h0.faceIndex = i0;
            h1.faceIndex = i0;
            h2.faceIndex = i0;

            h0.verIndex = (short)meshTriangles[i];
            h1.verIndex = (short)meshTriangles[i + 1];
            h2.verIndex = (short)meshTriangles[i + 2];

            // Set the vertices halfedge index. It doesn't matter if we overwrite previously set values.
            vertices[meshTriangles[i]].heIndex = i0;
            vertices[meshTriangles[i + 1]].heIndex = i1;
            vertices[meshTriangles[i + 2]].heIndex = i2;
            // next entries. Unity wants clockwise order of vertices
            h0.nextIndex = h1.index;
            h1.nextIndex = h2.index;
            h2.nextIndex = h0.index;

            // Add them!
            halfEdges.Add(h0);
            halfEdges.Add(h1);
            halfEdges.Add(h2);

            // set face HalfEdge index. i0 is a multiple of 3
            faces[i0 / 3].heIndex = i0;
            // Fill edgemap, three times :)
            string sum;
            // This check is required so the ordering is consistent
            if (h0.verIndex >= h1.verIndex)
            {
                sum = h0.verIndex.ToString() + h1.verIndex.ToString();
            }
            else
            {
                sum = h1.verIndex.ToString() + h0.verIndex.ToString();
            }
            if (!edgeMap.ContainsKey(sum))
            {
                List<short> l = new List<short>();
                // add the face index
                l.Add(i0);
                edgeMap.Add(sum, l);
            }
            else
            {
                edgeMap[sum].Add(i0);
            }

            if (h1.verIndex >= h2.verIndex)
            {
                sum = h1.verIndex.ToString() + h2.verIndex.ToString();
            }
            else
            {
                sum = h2.verIndex.ToString() + h1.verIndex.ToString();
            }
            if (!edgeMap.ContainsKey(sum))
            {
                List<short> l = new List<short>();
                // add the face index
                l.Add(i1);
                edgeMap.Add(sum, l);
            }
            else
            {
                edgeMap[sum].Add(i1);
            }

            if (h0.verIndex >= h2.verIndex)
            {
                sum = h0.verIndex.ToString() + h2.verIndex.ToString();
            }
            else
            {
                sum = h2.verIndex.ToString() + h0.verIndex.ToString();
            }
            if (!edgeMap.ContainsKey(sum))
            {
                List<short> l = new List<short>();
                // add the face index
                l.Add(i2);
                edgeMap.Add(sum, l);
            }
            else
            {
                edgeMap[sum].Add(i2);
            }
        }

        // Fill the opposite entries
        for (int i = 0; i < meshTriangles.Length; i += 3)
        {
            // each edge is shared between exactly two faces 
            // Stored 3-by-3 indices. e.g. 0,1,2 forms the first triangle.
            short i0 = (short)i;
            short i1 = (short)(i + 1);
            short i2 = (short)(i + 2);
            HalfEdge h0 = halfEdges[i0];
            HalfEdge h1 = halfEdges[i1];
            HalfEdge h2 = halfEdges[i2];
            // First edge
            // Use our edgeMap to find opposites
            string sum;
            if (h0.verIndex >= h1.verIndex)
            {
                sum = h0.verIndex.ToString() + h1.verIndex.ToString();
            }
            else
            {
                sum = h1.verIndex.ToString() + h0.verIndex.ToString();
            }
            List<short> l = edgeMap[sum];

            short h0Idx = l[0];
            short h1Idx = l[1];
            halfEdges[h0Idx].oppositeIndex = h1Idx;
            halfEdges[h1Idx].oppositeIndex = h0Idx;

            // Second edge
            if (h1.verIndex >= h2.verIndex)
            {
                sum = h1.verIndex.ToString() + h2.verIndex.ToString();
            }
            else
            {
                sum = h2.verIndex.ToString() + h1.verIndex.ToString();
            }
            l = edgeMap[sum];

            h0Idx = l[0];
            h1Idx = l[1];
            halfEdges[h0Idx].oppositeIndex = h1Idx;
            halfEdges[h1Idx].oppositeIndex = h0Idx;

            // Third edge
            if (h0.verIndex >= h2.verIndex)
            {
                sum = h0.verIndex.ToString() + h2.verIndex.ToString();
            }
            else
            {
                sum = h2.verIndex.ToString() + h0.verIndex.ToString();
            }
            l = edgeMap[sum];

            h0Idx = l[0];
            h1Idx = l[1];
            halfEdges[h0Idx].oppositeIndex = h1Idx;
            halfEdges[h1Idx].oppositeIndex = h0Idx;
        }
    }

    public void Triangulate()
    {
        // Update each face to contain three vertices. Adds new faces when necessary.
        int origFaceCount = faces.Count;
        for (int i = 0; i < origFaceCount; i++)
        {
            List<short> faceEdges = HalfEdge.FaceHalfEdges(faces[i], halfEdges);
            if (faceEdges.Count == 3)
            {
                // Already a triangle
                continue;
            }
            
            // There are two cases, 4 or 5 vertices, for 4 vertices we can just do it over the diagonal
            if (faceEdges.Count == 4)
            {
                HEFace newFace = new HEFace();
                newFace.normal = faces[i].normal;

                // Create new edge over diagonal
                // Make function to find the next diagonal that forms a triangle
                HalfEdge newEdge = new HalfEdge();
                HalfEdge from = halfEdges[faces[i].heIndex];
                HalfEdge to = halfEdges[halfEdges[from.nextIndex].nextIndex];

                Debug.Log("4!");
            } else if (faceEdges.Count == 5)
            {
                // Algorithm;
                // Get the two vertices on the plane
                List<short> onPlaneIdx = GetConfigEdges(faces[i], PlaneConfig.On);
                List<short> leftPlaneIdx = GetConfigEdges(faces[i], PlaneConfig.Left);
                List<short> rightPlaneIdx = GetConfigEdges(faces[i], PlaneConfig.Right);
                Debug.Log("on = " + onPlaneIdx.Count + ". Left = " + leftPlaneIdx.Count + ". Right = " + rightPlaneIdx.Count);
                // Either there are two on the left side or on the right side.  (Always 2 On)
                // This will be the left place idx face. We need to create two new half-edges between the on edges
                // Lets say we have l1-l2-o1-r1-o2  then the right triangle is o1-r1-o2
                // Should make a function that takes a face and two vertices (half edges rather) and splits it in two
                List<short> onEdgesIndices = GetConfigEdges(faces[i], PlaneConfig.On);
                SplitFace(faces[i], halfEdges[onEdgesIndices[0]], halfEdges[onEdgesIndices[1]]);
                // TODO: Make a verify function that verifies the half-edge data structure...
                // Check that the new faces are atleast the right size..
                List<short> f1 = HalfEdge.FaceHalfEdges(faces[i], halfEdges);
                List<short> f2 = HalfEdge.FaceHalfEdges(faces[faces.Count - 1], halfEdges);
                // One of the new faces have 4 vertices, so we need to triangulate it / split into two
                if (f1.Count == 4)
                {
                    SplitFace(faces[i], halfEdges[faces[i].heIndex], halfEdges[halfEdges[halfEdges[faces[i].heIndex].nextIndex].nextIndex]);
                } else
                {
                    int idx = faces.Count - 1;
                    SplitFace(faces[idx], halfEdges[faces[idx].heIndex], halfEdges[halfEdges[halfEdges[faces[idx].heIndex].nextIndex].nextIndex]);
                }


                Debug.Log(i + ". f1 = " + f1.Count + ". f2 = " + f2.Count);
            } else
            {
                // Don't go here...
                // shouldn't be possible
            }

        }
    }

    // Every face should be a triangle! So call Triangulate first
    public void SplitInLeftAndRightMesh(HalfEdgeMesh left, HalfEdgeMesh right)
    {
        // Have to re-index everything :(
        Dictionary<Vector3, short> leftVertDict = new Dictionary<Vector3, short>();
        Dictionary<Vector3, short> rightVertDict = new Dictionary<Vector3, short>();

        // First add the vertices, and get their indices
        short lVertIdx = 0;
        short rVertIdx = 0;
        foreach (HEVertex v in vertices)
        {
            HEVertex newVL = new HEVertex { v = v.v };
            HEVertex newVR = new HEVertex { v = v.v };
            if (v.config == PlaneConfig.Left && !leftVertDict.ContainsKey(v.v))
            {
                leftVertDict.Add(v.v, lVertIdx);
                left.vertices.Add(newVL);
                lVertIdx++;
            } else if (v.config == PlaneConfig.Right && !rightVertDict.ContainsKey(v.v))
            {
                rightVertDict.Add(v.v, rVertIdx);
                right.vertices.Add(newVR);
                rVertIdx++;
            } else
            {
                if (!leftVertDict.ContainsKey(v.v))
                {
                    leftVertDict.Add(v.v, lVertIdx);
                    left.vertices.Add(newVL);
                    lVertIdx++;
                }
                if (!rightVertDict.ContainsKey(v.v))
                {
                    rightVertDict.Add(v.v, rVertIdx);
                    right.vertices.Add(newVR);
                    rVertIdx++;
                }
            }
        }
        // The dicts should map old idx -> new idx...
        Dictionary<short, short> leftEdgeIdxMap = new Dictionary<short, short>();
        Dictionary<short, short> rightEdgeIdxMap = new Dictionary<short, short>();
        Dictionary<short, short> leftFaceIdxMap = new Dictionary<short, short>();
        Dictionary<short, short> rightFaceIdxMap = new Dictionary<short, short>();

        // Build the idx maps
        foreach (HalfEdge h in halfEdges)
        {
            short lEdgeIdx = 0;
            short lFaceIdx = 0;
            short rEdgeIdx = 0;
            short rFaceIdx = 0;
            PlaneConfig pConfig = GetFaceConfig(faces[h.faceIndex]);
            if (pConfig == PlaneConfig.Left)
            {
                leftEdgeIdxMap.Add(h.index, lEdgeIdx);
                left.halfEdges.Add(h);
                lEdgeIdx++;
                if (!leftFaceIdxMap.ContainsKey(h.faceIndex))
                {
                    left.faces.Add(faces[h.faceIndex]);
                    left.faces[lFaceIdx].heIndex = (short)(lEdgeIdx - 1);
                    leftFaceIdxMap.Add(h.faceIndex, lFaceIdx);
                    lFaceIdx++;
                }
            } else if (pConfig == PlaneConfig.Right)
            {
                rightEdgeIdxMap.Add(h.index, rEdgeIdx);
                right.halfEdges.Add(h);
                rEdgeIdx++;
                if (!rightFaceIdxMap.ContainsKey(h.faceIndex))
                {
                    right.faces.Add(faces[h.faceIndex]);
                    right.faces[rFaceIdx].heIndex = (short)(rEdgeIdx - 1); 
                    rightFaceIdxMap.Add(h.faceIndex, rFaceIdx);
                    rFaceIdx++;
                }
            } else // On, not possible for a face?
            {
                // ...
            }
        }
        // Update all the indices and stuff
        // Edge case is if both vertices are ON the plane, then they don't have an opposite edge
        // I think I will leave it and deal with it when triangulating the cap
        foreach (KeyValuePair<short, short> kvp in leftEdgeIdxMap)
        {
            // The edge
            left.halfEdges[kvp.Value].index = kvp.Value;
            left.halfEdges[kvp.Value].faceIndex = leftFaceIdxMap[kvp.Key];
            left.halfEdges[kvp.Value].nextIndex = leftEdgeIdxMap[halfEdges[kvp.Key].nextIndex];
            left.halfEdges[kvp.Value].verIndex = leftVertDict[vertices[halfEdges[kvp.Key].verIndex].v];
            // face done in loop above

            // Update vertices edge index, doesn't matter if we overwrite
            left.vertices[left.halfEdges[kvp.Value].verIndex].heIndex = kvp.Value;
        }
        foreach (KeyValuePair<short, short> kvp in rightEdgeIdxMap)
        {
            // The edge
            right.halfEdges[kvp.Value].index = kvp.Value;
            right.halfEdges[kvp.Value].faceIndex = rightFaceIdxMap[kvp.Key];
            right.halfEdges[kvp.Value].nextIndex = rightEdgeIdxMap[halfEdges[kvp.Key].nextIndex];
            right.halfEdges[kvp.Value].verIndex = rightVertDict[vertices[halfEdges[kvp.Key].verIndex].v];
            // face done in loop above

            // Update vertices edge index, doesn't matter if we overwrite
            right.vertices[right.halfEdges[kvp.Value].verIndex].heIndex = kvp.Value;
        }
    }

    private PlaneConfig GetFaceConfig(HEFace face)
    {
        // If any vertex is left / right the whole face is it
        List<short> heIdx = HalfEdge.FaceHalfEdges(face, halfEdges);
        foreach (short s in heIdx)
        {
            if (vertices[halfEdges[s].verIndex].config == PlaneConfig.Left)
                return PlaneConfig.Left;
            else if (vertices[halfEdges[s].verIndex].config == PlaneConfig.Right)
                return PlaneConfig.Right;
        }
        return PlaneConfig.On;
    }

    // h0 and h1 are halfedges in face.
    // Creates an halfedge between the vertices of h0 and h1
    // And creates a new face.
    private void SplitFace(HEFace face, HalfEdge h0, HalfEdge h1)
    {
        HEFace newFace = new HEFace();
        newFace.normal = face.normal;
        // walk from h0 to h1, this will be the vertices of the new face
        List<short> newFaceEdges = new List<short>();
        HalfEdge h1TOh0 = new HalfEdge(); // part of face when walking from h0 to h1
        HalfEdge h0TOh1 = new HalfEdge();
        // Update face halfedge indices
        newFace.heIndex = h0.index;
        face.heIndex = h1.index;
        // Set new half edge data
        h1TOh0.index = (short)halfEdges.Count;
        h0TOh1.index = (short)(halfEdges.Count + 1);
        h1TOh0.faceIndex = (short)faces.Count; // part of new face
        h0TOh1.faceIndex = h0.faceIndex; // old face
        h1TOh0.verIndex = h1.verIndex;
        h0TOh1.verIndex = h0.verIndex;
        h1TOh0.nextIndex = h0.index;
        h0TOh1.nextIndex = h1.index;
        h1TOh0.oppositeIndex = h0TOh1.index;
        h0TOh1.oppositeIndex = h1TOh0.index;
        // Update the next of the edges before h0 and h1
        // this is hard...
     //   Debug.Log("1");
        short b4h1 = h0.nextIndex; // Bless thy names
        while (halfEdges[b4h1].nextIndex != h1.index)
        {
            b4h1 = halfEdges[b4h1].nextIndex;
        }
      //  Debug.Log("2");
        halfEdges[b4h1].nextIndex = h1TOh0.index;
     //   Debug.Log("2.1");
        short b4h0 = h1.nextIndex; // Bless thy names
        while (halfEdges[b4h0].nextIndex != h0.index)
        {
       //     Debug.Log("2.3 : " + halfEdges[b4h0].nextIndex + " . " + (halfEdges.Count + 1));
            b4h0 = halfEdges[b4h0].nextIndex;
        }
     //   Debug.Log("3");
        halfEdges[b4h0].nextIndex = h0TOh1.index;
        // Update the face index of edges in the new face h0-..-..-h1toh0
        short first = h0.index;
        h0.faceIndex = (short)faces.Count;
        short next = h0.nextIndex;
        // Walk the next index until we return to the first.
       // Debug.Log("4");
        while (next != h1TOh0.index)
        {
            halfEdges[next].faceIndex = (short)faces.Count;
            next = halfEdges[next].nextIndex;
        }
        halfEdges.Add(h1TOh0);
        halfEdges.Add(h0TOh1);
        faces.Add(newFace);
    }

    // Return indices of HalfEdges with vertices that have the given config
    List<short> GetConfigEdges(HEFace face, PlaneConfig config)
    {
        List<short> he = HalfEdge.FaceHalfEdges(face, halfEdges);
        List<short> res = new List<short>();

        foreach (short s in he)
        {
            if (vertices[halfEdges[s].verIndex].config == config)
            {
                res.Add(s);
            }
        }
        return res;
    }

    // Reacreate a unity mesh with vertices, triangles, and normals.
    // TODO: IT NOW ASSUMES A FACE IS A TRIANGLE
    // TODO: FAN TRIANGULATION? Can we assume that all generated faces will be simple convex??
    public Mesh GetMesh()
    {
        Mesh meshRes = new Mesh();
        List<int> meshTriangles = new List<int>();
        List<Vector3> meshVertices = new List<Vector3>();
        List<Vector3> meshNormals = new List<Vector3>();
        // Foreach face. Walk it and save the vertex and normal data toghether with triangle indices.
        int triIdx = 0;
        foreach (HEFace face in faces)
        {
            // Each face has a normal and an index into halfEdges
            // Get the face halfedges
            List<short> faceHalfEdges = HalfEdge.FaceHalfEdges(face, halfEdges);
            if (faceHalfEdges.Count != 3)
            {
                Debug.Log("Face contains = " + faceHalfEdges.Count);
                Debug.Assert(faceHalfEdges.Count == 3);
            }
            foreach (short i in faceHalfEdges)
            {
                meshVertices.Add(vertices[halfEdges[i].verIndex].v);
                //    Debug.Log(vertices[halfEdges[i].verIndex].v);
                meshNormals.Add(face.normal);
                meshTriangles.Add(triIdx);
                triIdx++;
            }
        }

        meshRes.vertices = meshVertices.ToArray();
        meshRes.triangles = meshTriangles.ToArray();
        meshRes.normals = meshNormals.ToArray();
        return meshRes;
    }

}
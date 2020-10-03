using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawLineBetweenJoints : MonoBehaviour
{
    private LineRenderer lineRenderer;
    public Transform[] nextJoint;
    public bool useWrist;
    public bool usePalm;

    // Start is called before the first frame update
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    /// <summary>
    /// Draws out the skeleton with line renderers. 
    /// if useWrist is true it will need one extra postion, and if usePalm is true it will use two extra positions.
    /// </summary>
    void LateUpdate()
    {
        lineRenderer.SetPosition(0, nextJoint[0].position);
        lineRenderer.SetPosition(1, nextJoint[1].position);
        lineRenderer.SetPosition(2, nextJoint[2].position);
        lineRenderer.SetPosition(3, nextJoint[3].position);

        if (useWrist)
        {
            lineRenderer.SetPosition(4, nextJoint[4].position);
        }
        if (usePalm)
        {
            lineRenderer.SetPosition(4, nextJoint[4].position);
            lineRenderer.SetPosition(5, nextJoint[5].position);
        }
    }
}

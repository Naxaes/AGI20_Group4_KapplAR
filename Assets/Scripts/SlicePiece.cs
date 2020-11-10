using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.UIElements;

public class SlicePiece : MonoBehaviour
{

    public GameObject sliceSFX;
    public bool active;
    GameObject VFXSlice;

    // Start is called before the first frame update
    void Start()
    {

    }

    bool mouseHit = false;
    RaycastHit mouseRayHit;

    private void FixedUpdate()
    {

    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("Raycast!");
            mouseHit = MouseRaycastGamePieces(out mouseRayHit);
        }
        if (mouseHit)
        {
            Debug.Log("Plane split! " + Time.realtimeSinceStartup);
            PlaneSplit();
            mouseHit = false;
        }
    }
    private bool MouseRaycastGamePieces(out RaycastHit hit)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        int virtualSceneMask = 1 << 8;
        float maxDistance = 200.0f;
        if (Physics.Raycast(ray, out hit, maxDistance, virtualSceneMask))
        {
            if (hit.collider.gameObject.CompareTag("Game Piece"))
            {
                return true;
            }
        }

        return false;
    }

    public GameObject plane;
    public void PlaneSplit()
    {
       // GameObject clone = Instantiate(this.gameObject);
        Mesh m_this = this.gameObject.GetComponent<MeshFilter>().mesh;
        MeshCollider mc_this = this.gameObject.GetComponent<MeshCollider>();

        // MeshFilter mf_clone = clone.transform.Find("Cube").GetComponent<MeshFilter>();

        Vector3 plane_normal = this.gameObject.transform.InverseTransformDirection(plane.transform.up);
        Vector3 plane_pos = this.gameObject.transform.InverseTransformPoint(plane.transform.position);
        Vector3[] vertices = m_this.vertices;
        Debug.Log("plane pos = " + plane_pos);
        Debug.Log("plane normal = " + plane_normal);
        for (var i = 0; i < vertices.Length; i++)
        {

            // Check if hit point is in bounds of our mesh
            // +
            // Create vector from plane to vertex
            // Check what side of plane
            Vector3 pv = vertices[i] - plane_pos;
           // Debug.Log("i = " + i + " v[i] = " + vertices[i]);
           // Debug.Log("i = " + i + " pv = " + pv);
           // Debug.Log("i = " + i + " " + Vector3.Dot(plane_normal, pv));
            if (Vector3.Dot(plane_normal, pv) <= 0.0f)
            {
              //  if 
                // On the remove side of the plane: check where to move the vertex by raycasting
                RaycastHit testHit;
                // need to be in world space
                Ray ray = new Ray();
                ray.direction = -this.gameObject.transform.right;
                ray.origin = this.gameObject.transform.TransformPoint(vertices[i]);
                int slicePlaneMask = 1 << 9;
                Physics.Raycast(ray, out testHit, 200, slicePlaneMask);
                // check if the hit is inside our mesh 
                // Create tiny sphere collider att intersection point and check if it is inside the mesh collider
                int numColliders = 5; // 1 should be enough
                Collider[] hitColliders = new Collider[numColliders];
                int nCols = Physics.OverlapSphereNonAlloc(testHit.point, 0.01f, hitColliders, 1 << 8);
                bool insideMesh = false;
                
                if (nCols == 1)
                {
                    insideMesh = hitColliders[0].gameObject.Equals(this.gameObject);
                } else
                {
                    Debug.Log("nCols = " + nCols);
                }
                if (insideMesh)
                {
                    vertices[i] = this.transform.InverseTransformPoint(testHit.point);
                } else { }
            }
        }
        mc_this.sharedMesh = null;
        m_this.vertices = vertices;
        m_this.RecalculateBounds();
        mc_this.sharedMesh = m_this;
    }

    /*
     * "Split / cut / slice" a Kapla along the zy plane at the point p.x coordinate in local space.
     * The split is done by creating a clone of the plank and then scaling the clone and the original.
     * p is the hit point in world space.
     */
    public void Split(Vector3 p)
    {
        GameObject clone = Instantiate(this.gameObject);
        clone.SetActive(true);

        // Transform world pos of hit to local space, x will be before scaling, i.e. [-0.5, 0.5]
        p = this.gameObject.transform.InverseTransformPoint(p);
        // Scale the hit pos
        Vector3 locScale = this.gameObject.transform.localScale;
        float locScaleX = this.gameObject.transform.localScale.x;
        p.x *= locScaleX;
        // Align it to 0 at the left
        p.x += locScaleX / 2.0f;
        // p.x is now the x sale of one of the objects and its complement the other
        Vector3 newScaleClone = new Vector3(locScaleX - p.x, locScale.y, locScale.z);
        Vector3 newScaleOrig = new Vector3(p.x, locScale.y, locScale.z);
        // Ratio of hit
        float r = p.x / locScaleX;
        // Multiply ratio and ratio complement by scale/2 (mid)
        float mid = locScaleX / 2.0f;
        float s1 = mid * r;
        float s2 = mid * (1.0f - r);

        Rigidbody rbOrig = this.gameObject.GetComponent<Rigidbody>();
        Rigidbody rbClone = clone.GetComponent<Rigidbody>();
        // We want to offset the positions of the two objects by s1 and -s2 along the local x-axis
        // First convert (1.0f,0.0f, 0.0f) to world space, then multiply by s1 or -s2.
        Vector3 d = this.gameObject.transform.TransformDirection(new Vector3(1.0f, 0.0f, 0.0f));
        Vector3 clonePosOffset = d * s1;
        Vector3 origPosOffset = d * -s2;
        clone.transform.localScale = newScaleClone;
        clone.transform.position = this.gameObject.transform.position + clonePosOffset;
        this.gameObject.transform.localScale = newScaleOrig;
        this.gameObject.transform.position += origPosOffset;

        // Add a small impulse force in d or -d direction
         rbOrig.AddForce(-d*1.0f, ForceMode.Impulse);
         rbClone.AddForce(d * 1.0f, ForceMode.Impulse);
    }

 


    public void UpdateSliceIndicator(Vector3 p)
    {
        p = this.gameObject.transform.InverseTransformPoint(p);
        // Scale the hit pos
        float locScaleX = this.gameObject.transform.localScale.x;
        p.x *= locScaleX;
        // offset the vfx quad in the x-axis
        Vector3 d = this.gameObject.transform.TransformDirection(new Vector3(1.0f, 0.0f, 0.0f));
        Vector3 origPosOffset = d * p.x;
        Vector3 sfxPos = this.gameObject.transform.position + origPosOffset;

        d = this.gameObject.transform.TransformDirection(new Vector3(1.0f, 0.0f, 0.0f));
        origPosOffset = d * p.x;
        sfxPos = this.gameObject.transform.position + origPosOffset;
        VFXSlice.transform.position = sfxPos;
        VFXSlice.transform.rotation = this.gameObject.transform.rotation;
    }

    public void StopSliceIndicatorAnimation()
    {
        if (active) {
            active = false;
            Destroy(VFXSlice);
        }
    }

    public void StartSliceIndicatorAnimation(Vector3 p) {
        active = true;
        // Transform world pos of hit to local space, x will be before scaling, i.e. [-0.5, 0.5]
        p = this.gameObject.transform.InverseTransformPoint(p);
        // Scale the hit pos
        float locScaleX = this.gameObject.transform.localScale.x;
        p.x *= locScaleX;
        // offset the vfx quad in the x-axis
        Vector3 d = this.gameObject.transform.TransformDirection(new Vector3(1.0f, 0.0f, 0.0f));
        Vector3 origPosOffset = d * p.x;
        Vector3 sfxPos = this.gameObject.transform.position + origPosOffset;
        VFXSlice = Instantiate(sliceSFX, sfxPos, this.gameObject.transform.rotation);
        Transform[] allChildren = VFXSlice.GetComponentsInChildren<Transform>();
        float animTime = 10.0f;
        int i = 0;
        foreach (Transform child in allChildren)
        {
            if (i == 0)
            {
                // Proper hack for skipping parent transform
                i++;
                continue;
            }
            child.gameObject.GetComponent<Renderer>().sharedMaterial.SetFloat("_AnimTimeStart", Time.timeSinceLevelLoad);
            child.gameObject.GetComponent<Renderer>().sharedMaterial.SetFloat("_AnimTimeEnd", Time.timeSinceLevelLoad + animTime);
        }

        d = this.gameObject.transform.TransformDirection(new Vector3(1.0f, 0.0f, 0.0f));
        origPosOffset = d * p.x;
        sfxPos = this.gameObject.transform.position + origPosOffset;
        VFXSlice.transform.position = sfxPos;
        VFXSlice.transform.rotation = this.gameObject.transform.rotation;
    }
}



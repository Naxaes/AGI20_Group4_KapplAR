using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

public class SlicePiece : MonoBehaviour
{

    public GameObject sliceSFX;
    public bool active;
    Vector3 hitPoint;
    RaycastHit splitHit;

    // Start is called before the first frame update
    void Start()
    {

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

    // Update is called once per frame
    void Update()
    {
        if (active)
        {
           // UpdateSliceIndicator();
        }
    }

    void FixedUpdate()
    {
        /*
        if (mouseDown)
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Debug.DrawRay(ray.origin, ray.direction * 10, Color.yellow, 5, false);
            int virtualSceneMask = 1 << 8;
            float maxDist = 1000.0f;
            if (Physics.Raycast(ray, out hit, maxDist, virtualSceneMask))
            {
                if (this.gameObject.GetInstanceID() == hit.collider.gameObject.GetInstanceID() && !animationActive)
                {
                    startAnimation = true;
                    hitPoint = hit.point;
                    splitHit = hit;
                }
            }
            mouseDown = false;
        }
        if (animationDone)
        {
            Split(hitPoint);
            animationDone = false;
        }
        */
    }

    GameObject VFXSlice;

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

    /*
    public void SplitAnimation(RaycastHit hit)
    {
        animationActive = true;
        float runTime = 0.0f;

        Vector3 p = hit.point;
        // Transform world pos of hit to local space, x will be before scaling, i.e. [-0.5, 0.5]
        p = this.gameObject.transform.InverseTransformPoint(p);
        // Scale the hit pos
        float locScaleX = this.gameObject.transform.localScale.x;
        p.x *= locScaleX;
        // offset the vfx quad in the x-axis
        Vector3 d = this.gameObject.transform.TransformDirection(new Vector3(1.0f, 0.0f, 0.0f));
        Vector3 origPosOffset = d * p.x;
        Vector3 sfxPos = this.gameObject.transform.position + origPosOffset;
        GameObject obj = Instantiate(sliceSFX, sfxPos, this.gameObject.transform.rotation);
        Transform[] allChildren = obj.GetComponentsInChildren<Transform>();
        float animTime = 1.0f;
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

        while (runTime <= animTime)
        {
            runTime += Time.deltaTime;
            d = this.gameObject.transform.TransformDirection(new Vector3(1.0f, 0.0f, 0.0f));
            origPosOffset = d * p.x;
            sfxPos = this.gameObject.transform.position + origPosOffset;
            obj.transform.position = sfxPos;
            obj.transform.rotation = this.gameObject.transform.rotation;
            yield return null;
        }
        // Fade TODO
        Destroy(obj);
        animationDone = true;
        animationActive = false;
    }
    */
}



using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.Experimental.XR;
using System.Collections.Specialized;
using UnityEngine.XR.ARSubsystems;

public class ARTapToPlaceObject : MonoBehaviour
{
    private ARRaycastManager raycastManager;
    public GameObject objectToPlace;
    public GameObject placementIndicator;

    // Pose holds rotation and position of 3D object
    private Pose placementPose;
    private bool placementPoseIsValid = false;

    // Start is called before the first frame update
    void Start()
    {
        raycastManager = FindObjectOfType<ARRaycastManager>();
    }

    // Update is called once per frame
    void Update()
    {
        //UpdatePlacementPose();
        UpdatePlacementIndicator();

        if (placementPoseIsValid && Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            PlaceObject();
        }
        
        if (Input.touchCount == 2 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            RayCastScene();
        }
    }

    private void RayCastScene()
    {
        /*
        var screenCenter = Camera.current.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));
        var hits = new List<ARRaycastHit>();
        Physics.Raycast(screenCenter, out RaycastHit hit);
        placementPoseIsValid = hits.Count > 0;
        */
        Ray ray = Camera.current.ScreenPointToRay(Input.GetTouch(0).position);
        int virtualSceneMask = 1 << 8;
        float maxDistance = 200.0f;
        if (Physics.Raycast(ray, out RaycastHit hit, maxDistance, virtualSceneMask))
        {
            Destroy(hit.collider.gameObject);
        }
    }

    /*
     * Physics engine related updates
     */
    private void FixedUpdate()
    {
        UpdatePlacementPose();
        
    }

    private void PlaceObject()
    {
        Instantiate(objectToPlace, placementPose.position, placementPose.rotation);
    }

    private void UpdatePlacementIndicator()
    {
        if (placementPoseIsValid)
        {
            placementIndicator.SetActive(true);
            placementIndicator.transform.SetPositionAndRotation(placementPose.position, placementPose.rotation);
        }
        else
        {
            placementIndicator.SetActive(false);
        }
    }


    private void UpdatePlacementPose()
    {


        // Shoot ray into virual scene (Game object placement).
        Ray ray = Camera.current.ScreenPointToRay(new Vector3(Screen.width / 2.0f, Screen.height / 2.0f));
        int virtualSceneMask = 1 << 8;
        float maxDistance = 200.0f;
        bool virtualHit = Physics.Raycast(ray, out RaycastHit hit, maxDistance, virtualSceneMask);
        if (virtualHit)
        {
            // The offset needs to be doubled or half of the container will still be inside.
            // the 0.01 objects is a quick fix for stopping objects being placed inside eachother.
            Vector3 offset = (hit.point - hit.collider.bounds.center) * 2.01f;
            placementPose.position = hit.collider.gameObject.transform.position + offset;
            placementPose.rotation = hit.collider.gameObject.transform.rotation;

            // Do we want to place inside another box?
            Collider[] hitColliders = Physics.OverlapBox(placementPose.position,  (new Vector3(2.0f,2.0f,2.0f)), placementPose.rotation, virtualSceneMask);
            if (hitColliders.Length == 0)
                placementPoseIsValid = true;
            else
                placementPoseIsValid = false;

            return;
        }

        // Shoot ray from center of screen and check plane collision
        var screenCenter = Camera.current.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));
        var hits = new List<ARRaycastHit>();
        raycastManager.Raycast(screenCenter, hits, TrackableType.PlaneEstimated);

        placementPoseIsValid = hits.Count > 0;
        if (placementPoseIsValid)
        {
            placementPose = hits[0].pose;

            var cameraForward = Camera.current.transform.forward;
            var cameraBearing = new Vector3(cameraForward.x, 0, cameraForward.z).normalized;
            placementPose.rotation = Quaternion.LookRotation(cameraBearing);
        }

    }
}

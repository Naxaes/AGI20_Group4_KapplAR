using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.Experimental.XR;
using System.Collections.Specialized;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.UIElements;
using UnityEngine.PlayerLoop;
using UnityEngine.EventSystems;

public class ARInteraction : MonoBehaviour
{
    private ARRaycastManager raycastManager;
    public GameObject kaplaToPlace;
    public GameObject floorToPlace;
    public GameObject placementIndicator;
    public GameObject floorPlacementIndicator;

    // Placement indicator materials
    public Material validPlacementMaterial;
    public Material invalidPlacementMaterial;

    // Pose holds rotation and position of 3D object
    private Pose placementPose;
    private bool placementPoseIsValid = false;
    // Floor pose
    private Pose floorPose;
    private bool floorIsPlaced = false;
    private bool floorPoseIsValid = false;

    // Start is called before the first frame update
    void Start()
    {
        raycastManager = FindObjectOfType<ARRaycastManager>();
        placementPose.rotation = placementIndicator.transform.rotation;
        floorPose.rotation = floorPlacementIndicator.transform.rotation;

        placementIndicator.SetActive(false);

        UpdatePlacementIndicator();
        UpdateFloorIndicator();
    }

    // Update is called once per frame
    void Update()
    {
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            if (floorIsPlaced)
            {
                UpdatePlacementIndicator();

                if (placementPoseIsValid && Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Began)
                {
                    PlaceObject(ref kaplaToPlace, ref placementPose, true);
                }

            }
            else
            {
                UpdateFloorIndicator();

                if (floorPoseIsValid && Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Began)
                {
                    PlaceObject(ref floorToPlace, ref floorPose);
                    floorIsPlaced = true;
                    floorPlacementIndicator.SetActive(false);
                    placementIndicator.SetActive(true);
                }
            }
        }
    }

    /*
    * Physics engine related updates
    */
    private void FixedUpdate()
    {
        if (floorIsPlaced)
        {
            UpdatePlacementPose();
        } else
        {
            UpdateFloorPose();
        }
    }

    void RotatePlacementIndicator()
    {
        Vector3 curr = placementPose.rotation.eulerAngles;
        Quaternion q = Quaternion.Euler(curr + new Vector3(90.0f, 0.0f, 0.0f));

        placementPose.rotation = q;
    }

    private void RayCastDelete()
    {
       
        Ray ray = Camera.current.ScreenPointToRay(Input.GetTouch(0).position);
        int virtualSceneMask = 1 << 8;
        float maxDistance = 200.0f;
        if (Physics.Raycast(ray, out RaycastHit hit, maxDistance, virtualSceneMask))
        {
            Destroy(hit.collider.gameObject);
        }
    }

    private void PlaceObject(ref GameObject gameObject, ref Pose pose, bool saveObj = false)
    {
        if (saveObj)
        {
            GameObject newPiece = Instantiate(gameObject, pose.position, pose.rotation);
            newPiece.transform.SetParent(GameObject.Find("GamePieces").transform);
        } else
        {
            Instantiate(gameObject, pose.position, pose.rotation);
        }
    }

    void UpdateFloorIndicator()
    {
        if (floorPoseIsValid)
        {
            floorPlacementIndicator.SetActive(true);
            floorPlacementIndicator.transform.SetPositionAndRotation(floorPose.position, floorPose.rotation);
        }
        else
        {
            floorPlacementIndicator.SetActive(false);
        }
    }

    private void UpdatePlacementIndicator()
    {
        placementIndicator.transform.SetPositionAndRotation(placementPose.position, placementPose.rotation);
        if (placementPoseIsValid)
        {
            placementIndicator.GetComponentInChildren<MeshRenderer>().material = validPlacementMaterial;
            placementIndicator.transform.SetPositionAndRotation(placementPose.position, placementPose.rotation);
        }
        else
        {
            placementIndicator.GetComponentInChildren<MeshRenderer>().material = invalidPlacementMaterial;
        }
    }

    private void UpdateFloorPose()
    {
        floorPose.position = Camera.current.transform.position + 60.0f * Camera.current.transform.forward + new Vector3(0.0f, -5.0f, 0.0f);
        floorPoseIsValid = true;


        // If you want to use raycasting to place objects.
        /*
        // Shoot ray from center of screen and check plane collision
        var screenCenter = Camera.current.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));
        var hits = new List<ARRaycastHit>();
        raycastManager.Raycast(screenCenter, hits, TrackableType.PlaneEstimated);

        floorPoseIsValid = hits.Count > 0;
        if (floorPoseIsValid)
        {
            floorPose.position = hits[0].pose.position;

            var cameraForward = Camera.current.transform.forward;
            var cameraBearing = new Vector3(cameraForward.x, 0, cameraForward.z).normalized;
            floorPose.rotation = Quaternion.LookRotation(cameraBearing);
        }
        */
    }

    private void UpdatePlacementPose()
    {

        placementPose.position = Camera.current.transform.position + 25.0f * Camera.current.transform.forward;
        placementPose.rotation = placementIndicator.transform.rotation;
        int virtualSceneMask = 1 << 8;
        Collider[] hitColliders = Physics.OverlapBox(placementPose.position, kaplaToPlace.transform.localScale / 2.0f, placementPose.rotation, virtualSceneMask);
        if (hitColliders.Length == 0)
            placementPoseIsValid = true;
        else
            placementPoseIsValid = false;

        return;
    }
}

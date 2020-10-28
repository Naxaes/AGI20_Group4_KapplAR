﻿using System.Collections;
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
    //public GameObject kaplaToPlace;
    public GameObject floorToPlace;
    private GameObject placementIndicator;
    private Bloc currentBloc;
    public GameObject floorPlacementIndicator;

    private Level level;

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


    const float kTapDurationThreshold = 0.125f;
    const float kTapMoveThreshold = 15.0f;
    float fingerOnScreenDuration = 0f;
    bool fingerHasMoved = false;
    bool shouldReactToTapEvents = false;


    // Start is called before the first frame update
    void Start()
    {
        raycastManager = FindObjectOfType<ARRaycastManager>();

        level = GetComponent<Level>();
        currentBloc = level.inventory.currentItem;
        placementIndicator = Instantiate(currentBloc.gameObject, new Vector3(), new Quaternion());
        placementIndicator.GetComponent<Rigidbody>().isKinematic = true;
        placementIndicator.GetComponent<Rigidbody>().detectCollisions = false;

        placementPose.rotation = placementIndicator.transform.rotation;
        floorPose.rotation = floorPlacementIndicator.transform.rotation;


        placementIndicator.SetActive(false);

        UpdatePlacementIndicator();
        UpdateFloorIndicator();
    }

   public void PlacePlank()
    {
        if (placementPoseIsValid && floorIsPlaced && level.inventory.UseItem())
        {
            placementPose.rotation = placementIndicator.transform.rotation;
            PlaceObject(ref level.inventory.currentItem.gameObject, ref placementPose, true);
        } else if (!floorIsPlaced)
        {
            PlaceObject(ref floorToPlace, ref floorPose);
            floorIsPlaced = true;
            floorPlacementIndicator.SetActive(false);
            placementIndicator.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!floorIsPlaced)
        {
            UpdateFloorIndicator();
        } else
        {
            UpdatePlacementIndicator();
        }
        
        if (Input.touchCount > 0 && !EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                if(floorIsPlaced)
                {
                    shouldReactToTapEvents = true;
                } else
                {
                    PlacePlank();
                }
            }
            if (touch.phase == TouchPhase.Stationary && shouldReactToTapEvents)
            {
            }
            if ((touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled) && shouldReactToTapEvents)
            {
                if(!fingerHasMoved)
                {
                    PlacePlank();
                }
                fingerHasMoved = false;
                shouldReactToTapEvents = false;
               
            }
            if (touch.phase == TouchPhase.Moved && shouldReactToTapEvents)
            {
                float dx = Input.GetTouch(0).deltaPosition.x;
                float dy = Input.GetTouch(0).deltaPosition.y;
                // y-axis is UP -> dx for rotation around it.
                RotateInstant(new Vector3(dy / 3.0f, dx / 3.0f, 0f));
                fingerHasMoved = true;
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

    void RotateInstant(Vector3 angles)
    {
        Quaternion startRotation = placementIndicator.transform.localRotation;
        Quaternion endRotation;
        // Rotate around the camera axis.
        endRotation = Quaternion.AngleAxis(angles.y, Camera.current.transform.up) * startRotation;
        endRotation = Quaternion.AngleAxis(angles.x, Camera.current.transform.right) * endRotation;

        placementIndicator.transform.rotation = endRotation;
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
        if(level.inventory.currentItem != currentBloc)
        {
            Destroy(placementIndicator);
            currentBloc = level.inventory.currentItem;
            placementIndicator = Instantiate(currentBloc.gameObject, new Vector3(), new Quaternion());
            placementIndicator.GetComponent<Rigidbody>().isKinematic = true;
            placementIndicator.GetComponent<Rigidbody>().detectCollisions = false;
        }

        placementIndicator.transform.position = placementPose.position;
        if (placementPoseIsValid && level.inventory.inventory[currentBloc] > 0)
        {
            placementIndicator.GetComponentInChildren<MeshRenderer>().material = validPlacementMaterial;
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
        Collider[] hitColliders = Physics.OverlapBox(placementPose.position, level.inventory.currentItem.gameObject.transform.localScale / 2.0f, placementPose.rotation, virtualSceneMask);
        if (hitColliders.Length == 0)
            placementPoseIsValid = true;
        else
            placementPoseIsValid = false;

        return;
    }
}

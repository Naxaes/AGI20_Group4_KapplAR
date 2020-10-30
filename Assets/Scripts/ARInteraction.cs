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

public enum InteractionMode
{
    Placement,
    Cutting
}

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

    private InteractionMode interactionMode =  InteractionMode.Placement;

    private GameObject objectToSlice = null;
    private Vector3 slicePostion;

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

    public void SetInteractioModeCutting()
    {
        interactionMode = InteractionMode.Cutting;
        placementIndicator.SetActive(false);
    }

    public void SetInteractionModePlacement()
    {
        interactionMode = InteractionMode.Placement;
        placementIndicator.SetActive(true);
    }

   public void PlacePlank()
    {
        if (placementPoseIsValid && floorIsPlaced)
        {
            placementPose.rotation = placementIndicator.transform.rotation;
            PlaceObject(ref kaplaToPlace, ref placementPose, true);
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
        switch (interactionMode) {
            case InteractionMode.Placement:
                if (floorIsPlaced)
                {
                    UpdatePlacementIndicator();
                    if (Input.GetTouch(0).phase == TouchPhase.Moved && !EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
                    {
                        float dx = Input.GetTouch(0).deltaPosition.x;
                        float dy = Input.GetTouch(0).deltaPosition.y;
                        // y-axis is UP -> dx for rotation around it.
                        RotateInstant(new Vector3(dy / 3.0f, dx / 3.0f, 0f));
                    }
                } else
                {
                    UpdateFloorIndicator();
                }
                break;
            case InteractionMode.Cutting:
                if (objectToSlice != null)
                {
                    if (Input.GetTouch(0).phase == TouchPhase.Began && !EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
                    {
                        objectToSlice.GetComponent<SlicePiece>().Split(slicePostion);
                    }
                }
                break;

        }
    }

    /*
    * Physics engine related updates
    */
     private void FixedUpdate()
    {
        switch (interactionMode)
        {
            case InteractionMode.Placement:
                if (floorIsPlaced)
                {
                    UpdatePlacementPose();
                }
                else
                {
                    UpdateFloorPose();
                }
                break;
            case InteractionMode.Cutting:
                RaycastHit rayHit;
                bool hit = RaycastGamePieces(out rayHit);
                if (hit)
                {
                    slicePostion = rayHit.point;
                    SliceAnimation(rayHit);
                }
                else
                {
                    if (objectToSlice != null)
                    {
                        objectToSlice.GetComponent<SlicePiece>().StopSliceIndicatorAnimation();
                        objectToSlice = null;
                    }
                }

                break;
        }
    }

    private void SliceAnimation(RaycastHit hit)
    {
        // First check if we have an animation acitve
        if (objectToSlice == null)
        {
            objectToSlice = hit.collider.gameObject;
            // Start the animation
            objectToSlice.GetComponent<SlicePiece>().StartSliceIndicatorAnimation(hit.point);
        } // Check if we hit a new object
        else if (objectToSlice.GetInstanceID() != hit.collider.gameObject.GetInstanceID())
        {
            objectToSlice.GetComponent<SlicePiece>().StopSliceIndicatorAnimation();
            objectToSlice = hit.collider.gameObject;
            objectToSlice.GetComponent<SlicePiece>().StartSliceIndicatorAnimation(hit.point);
        } // Otherwise update position of ray hit
        else
        {
            objectToSlice.GetComponent<SlicePiece>().UpdateSliceIndicator(hit.point);
        }
    }

    private bool RaycastGamePieces(out RaycastHit hit)
    {
        Vector3 screenCenter  = Camera.current.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));
        Ray ray = Camera.current.ScreenPointToRay(screenCenter);
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
        placementIndicator.transform.position = placementPose.position;
        if (placementPoseIsValid)
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
        Collider[] hitColliders = Physics.OverlapBox(placementPose.position, kaplaToPlace.transform.localScale / 2.0f, placementPose.rotation, virtualSceneMask);
        if (hitColliders.Length == 0)
            placementPoseIsValid = true;
        else
            placementPoseIsValid = false;

        return;
    }
}

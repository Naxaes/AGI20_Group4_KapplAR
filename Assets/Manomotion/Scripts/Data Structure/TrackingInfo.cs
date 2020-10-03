using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;
using System.Runtime.InteropServices;



/// <summary>
/// Contains information about position and tracking of the hand
/// </summary> 
[StructLayout(LayoutKind.Sequential)]
public struct TrackingInfo
{
	/// <summary>
	/// Provides tracking information regarding the bounding box that contains the hand.
	/// it yields normalized values between 0..1
	/// </summary>
	public BoundingBox bounding_box;
	
	/// <summary>
	/// Provides tracking information regarding the reference point for the Pinch class (POI). This information is primarily used in the cursor gizmo.
	/// it yields a normalized Vector3 information with the depth being 0 at the moment.
	/// a temporary solution would be to use the width of the bounding box as the depth Z value.
	/// </summary>
	public Vector3 poi;

	/// <summary>
	/// Provides tracking information regarding the bounding box center that contains the hand, this information is primarily used in the cursor gizmo.
	/// it yields a normalized Vector3 information with the depth being 0 at the moment.
	/// a temporary solution would be to combine it with the depth estimation value (see below).
	/// </summary>
	public Vector3 palm_center;

	// <summary>
	// Provides tracking information regarding the bounding box center that contains the hand, this information is primarily used in the cursor gizmo.
	// it yields a normalized Vector3 information with the depth being 0 at the moment.
	// a temporary solution would be to combine it with the depth estimation value (see below).
    // </summary>
	//public Vector3 wrist;
	
	/// <summary>
	/// Information about the wrist in BETA mode.
	/// </summary>
	public WristInfo wristInfo;

	/// <summary>
	/// Provides tracking information regarding an estimation  of the hand depth. 
	/// it yields a 0-1 float value depending based on the distance of the hand from the camera.
	/// </summary>
	public float depth_estimation;

	/// <summary>
	/// Rotation of the hand
	/// normalized values between 0..1
	/// </summary>
	public float rotation;

	/// <summary>
	/// Amount of contour points in this frame, used to know how many of the values in the array are valid
	/// </summary>
	public int amount_of_contour_points;

	/// <summary>
	/// Position of the fingertips, to get a specific fingertip use @FingerTipIndex enum
	/// normalized values between 0..1
	/// </summary>
	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
	public Vector3[] finger_tips;

	/// <summary>
	/// Position of points on the contour of the hand
	/// normalized values between 0..1
	/// </summary>
	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 200)]
	public Vector3[] contour_points;

	/// <summary>
	/// Contains the positions of the 21 joints
	/// </summary>
	public SkeletonInfo skeleton;
}


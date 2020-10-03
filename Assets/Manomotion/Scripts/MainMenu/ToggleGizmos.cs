using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleGizmos : MonoBehaviour
{
	GizmoManager _gizmoManager;

	private void Start()
	{
		_gizmoManager = GetComponent<GizmoManager>();
	}

	/// <summary>
	/// Toggles the boolean value of showing the hand states
	/// </summary>
	public void ToggleShowHandStates()
	{
		_gizmoManager.ShowHandStates = !_gizmoManager.ShowHandStates;
	}

	/// <summary>
	/// Toggles the boolean value of showing the manoclass
	/// </summary>
	public void ToggleShowManoclass()
	{
		_gizmoManager.ShowManoClass = !_gizmoManager.ShowManoClass;
	}

	/// <summary>
	/// Toggles the boolean value of showing the handside of the detected hand;
	/// </summary>
	public void ToggleShowHandSide()
	{
		_gizmoManager.ShowHandSide = !_gizmoManager.ShowHandSide;
	}

	/// <summary>
	/// Toggles the boolean value of showing the continuous gesture of the detected hand;
	/// </summary>
	public void ToggleShowContinuousGestures()
	{
		_gizmoManager.ShowContinuousGestures = !_gizmoManager.ShowContinuousGestures;
	}

	/// <summary>
	/// Toggles the boolean value of showing Pick Trigger Gesture
	/// </summary>
	public void ToggleShowPickTriggerGesture()
	{
		_gizmoManager.ShowPickTriggerGesture = !_gizmoManager.ShowPickTriggerGesture;
	}

	/// <summary>
	/// Toggles the boolean value of showing Drop Trigger Gesture
	/// </summary>
	public void ToggleShowDropTriggerGesture()
	{
		_gizmoManager.ShowDropTriggerGesture = !_gizmoManager.ShowDropTriggerGesture;
	}

	/// <summary>
	/// Toggles the boolean value of showing Click Trigger Gesture
	/// </summary>
	public void ToggleShowClickTriggerGesture()
	{
		_gizmoManager.ShowClickTriggerGesture = !_gizmoManager.ShowClickTriggerGesture;
	}

	/// <summary>
	/// Toggles the boolean value of showing Grab Trigger Gesture
	/// </summary>
	public void ToggleShowGrabTriggerGesture()
	{
		_gizmoManager.ShowGrabTriggerGesture = !_gizmoManager.ShowGrabTriggerGesture;
	}

	/// <summary>
	/// Toggles the boolean value of showing Release Trigger Gesture
	/// </summary>
	public void ToggleShowReleaseTriggerGesture()
	{
		_gizmoManager.ShowReleaseTriggerGesture = !_gizmoManager.ShowReleaseTriggerGesture;
	}

	/// <summary>
	/// Toggles the show smoothing slider condition.
	/// </summary>
	/// 
	public void ToggleShowSmoothingSlider()
	{
		_gizmoManager.ShowSmoothingSlider = !_gizmoManager.ShowSmoothingSlider;
	}

	/// <summary>
	/// Toggles the show warnings condition.
	/// </summary>
	public void ToggleShowWarnings()
	{
		_gizmoManager.ShowWarnings = !_gizmoManager.ShowWarnings;
	}

	/// <summary>
	/// Toggles the show depth estimation condition.
	/// </summary>
	public void ToggleShowDepthEstimation()
	{
		_gizmoManager.ShowDepthEstimation = !_gizmoManager.ShowDepthEstimation;
	}

	/// <summary>
	/// Toggles the boolean value of geting the skeleton points in 3D or 2D.
	/// By toggling the value it will also update the manomotion session to start calculating it or not.
	/// </summary>
	public void ToggleShow3DSkeleton()
	{
		_gizmoManager.ShowSkeleton3d = !_gizmoManager.ShowSkeleton3d;
		ManomotionManager.Instance.ShouldCalculateSkeleton3D(_gizmoManager.ShowSkeleton3d);
	}

	/// <summary>
	/// Toggles the boolean value of make use of gestures, trigger and continious.
	/// By toggling the value it will also update the manomotion session to start calculating it or not.
	/// </summary>
	public void ToggleShowGestures()
	{
		_gizmoManager.ShowGestures = !_gizmoManager.ShowGestures;
		ManomotionManager.Instance.ShouldCalculateGestures(_gizmoManager.ShowGestures);
	}

	/// <summary>
	/// Toggles the boolean value of make use of fast mode.
	/// By toggling the value it will also update the manomotion session to start calculating it or not.
	/// </summary>
	public void ToggleFastMode()
	{
		_gizmoManager.Fastmode = !_gizmoManager.Fastmode;
		ManomotionManager.Instance.ShouldRunFastMode(_gizmoManager.Fastmode);
	}
}

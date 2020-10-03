﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class GizmoManager : MonoBehaviour
{
	#region Singleton
	private static GizmoManager _instance;
	public static GizmoManager Instance
	{
		get
		{
			return _instance;
		}

		set
		{
			_instance = value;
		}
	}
	#endregion

	public Color disabledStateColor;

	[SerializeField]
	private Image[] stateImages;

	[SerializeField]
	private GameObject handStatesGizmo;
	[SerializeField]
	private GameObject manoClassGizmo;
	[SerializeField]
	private GameObject handSideGizmo;
	[SerializeField]
	private GameObject continuousGestureGizmo;
	[SerializeField]
	private GameObject triggerTextPrefab;
	[SerializeField]
	private GameObject flagHolderGizmo;
	[SerializeField]
	private GameObject smoothingSliderControler;
	[SerializeField]
	private GameObject depthEstimationGizmo;

	[SerializeField]
	private Text currentSmoothingValue;

    [SerializeField]
    private bool _showHandStates;
    [SerializeField]
    private bool _showManoClass;
    [SerializeField]
    private bool _showHandSide;
    [SerializeField]
    private bool _showContinuousGestures;
    [SerializeField]
    private bool _showWarnings;
    [SerializeField]
    private bool _showPickTriggerGesture;
    [SerializeField]
    private bool _showDropTriggerGesture;
    [SerializeField]
    private bool _showClickTriggerGesture;
    [SerializeField]
    private bool _showGrabTriggerGesture;
    [SerializeField]
    private bool _showReleaseTriggerGesture;
    [SerializeField]
    private bool _showSmoothingSlider;
    [SerializeField]
    private bool _showDepthEstimation;

    [SerializeField]
    private bool skeleton3d;
    [SerializeField]
    private bool gestures;
	[SerializeField]
	private bool fastMode;

	private GameObject topFlag;
    private GameObject leftFlag;
    private GameObject rightFlag;

    private Text manoClassText;
    private Text handSideText;
    private Text continuousGestureText;

	private TextMeshProUGUI depthEstimationValue;
	private Image depthFillAmmount;

    private Transform rotationIconTransform;
    private Transform rotationTriangleTransform;

	#region Properties

	public bool ShowContinuousGestures
	{
		get
		{
			return _showContinuousGestures;
		}

		set
		{
			_showContinuousGestures = value;
		}
	}

	public bool ShowManoClass
	{
		get
		{
			return _showManoClass;
		}

		set
		{
			_showManoClass = value;
		}
	}

	public bool ShowHandSide
	{
		get
		{
			return _showHandSide;
		}

		set
		{
			_showHandSide = value;
		}
	}

	public bool ShowHandStates
	{
		get
		{
			return _showHandStates;
		}

		set
		{
			_showHandStates = value;
		}
	}

	public bool ShowWarnings
	{
		get
		{
			return _showWarnings;
		}

		set
		{
			_showWarnings = value;
		}
	}

	public bool ShowPickTriggerGesture
	{
		get
		{
			return _showPickTriggerGesture;
		}

		set
		{
			_showPickTriggerGesture = value;
		}
	}

	public bool ShowDropTriggerGesture
	{
		get
		{
			return _showDropTriggerGesture;
		}

		set
		{
			_showDropTriggerGesture = value;
		}
	}

	public bool ShowClickTriggerGesture
	{
		get
		{
			return _showClickTriggerGesture;
		}

		set
		{
			_showClickTriggerGesture = value;
		}
	}

	public bool ShowGrabTriggerGesture
	{
		get
		{
			return _showGrabTriggerGesture;
		}

		set
		{
			_showGrabTriggerGesture = value;
		}
	}

	public bool ShowReleaseTriggerGesture
	{
		get
		{
			return _showReleaseTriggerGesture;
		}

		set
		{
			_showReleaseTriggerGesture = value;
		}
	}

	public bool ShowSmoothingSlider
	{
		get
		{
			return _showSmoothingSlider;
		}

		set
		{
			_showSmoothingSlider = value;
		}
	}

	public bool ShowDepthEstimation
	{
		get
		{
			return _showDepthEstimation;
		}
		set
		{
			_showDepthEstimation = value;
		}
	}

    public bool ShowSkeleton3d
    {
        get
        {
            return skeleton3d;
        }
        set
        {
            skeleton3d = value;
        }
    }

    public bool ShowGestures
    {
        get
        {
            return gestures;
        }
        set
        {
            gestures = value;
        }
    }

	public bool Fastmode
	{
		get
		{
			return fastMode;
		}
		set
		{
			fastMode = value;
		}
	}

	#endregion

	void Start()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }

        SetGestureDescriptionParts();
        HighlightStatesToStateDetection(0);
        InitializeFlagParts();
        InitializeTriggerPool();
        SetFeaturesToCalculate();
    }

    /// <summary>
    /// Sets which features that should be calculated
    /// </summary>
    private void SetFeaturesToCalculate()
    {
        ManomotionManager.Instance.ShouldCalculateGestures(ShowGestures);
        ManomotionManager.Instance.ShouldCalculateSkeleton3D(ShowSkeleton3d);
    }

    void Update()
	{
		GestureInfo gestureInfo = ManomotionManager.Instance.Hand_infos[0].hand_info.gesture_info;
		TrackingInfo trackingInfo = ManomotionManager.Instance.Hand_infos[0].hand_info.tracking_info;
		Warning warning = ManomotionManager.Instance.Hand_infos[0].hand_info.warning;
		Session session = ManomotionManager.Instance.Manomotion_Session;

		DisplayContinuousGestures(gestureInfo.mano_gesture_continuous);
		DisplayManoclass(gestureInfo.mano_class);
		DisplayTriggerGesture(gestureInfo.mano_gesture_trigger, trackingInfo);
		DisplayHandState(gestureInfo.state);
		DisplayHandSide(gestureInfo.hand_side);
		DisplayApproachingToEdgeFlags(warning);
		DisplayCurrentsmoothingValue(session);
		DisplaySmoothingSlider();
        DisplayDepthEstimation(trackingInfo.depth_estimation);
	}

	#region Display Methods

	/// <summary>
	/// Displays the depth estimation of the detected hand.
	/// </summary>
	/// <param name="depthEstimation">Requires the float value of depth estimation.</param>
	void DisplayDepthEstimation(float depthEstimation)
	{
		depthEstimationGizmo.SetActive(ShowDepthEstimation);

		if (!depthEstimationValue)
		{
			depthEstimationValue = depthEstimationGizmo.transform.Find("DepthValue").gameObject.GetComponent<TextMeshProUGUI>();
		}
		if (!depthFillAmmount)
		{
			depthFillAmmount = depthEstimationGizmo.transform.Find("CurrentLevel").gameObject.GetComponent<Image>();
		}
		if (ShowDepthEstimation)
		{
            if (depthEstimation == -1)
            {
                depthEstimation = 0;
            }

            depthEstimationValue.text = depthEstimation.ToString("F2");
			depthFillAmmount.fillAmount = depthEstimation;
		}
	}

	/// <summary>
	/// Displays in text value the current smoothing value of the session
	/// </summary>
	/// <param name="session">Session.</param>
	void DisplayCurrentsmoothingValue(Session session)
	{
		if (smoothingSliderControler.activeInHierarchy)
		{
			currentSmoothingValue.text = "Gesture smoothing: " + session.gesture_smoothing_controller.ToString("F2");
		}
	}

	/// <summary>
	/// Displays information regarding the detected manoclass
	/// </summary>
	/// <param name="manoclass">Manoclass.</param>
	void DisplayManoclass(ManoClass manoclass)
	{
		manoClassGizmo.SetActive(ShowManoClass);
		if (ShowManoClass)
		{
			switch (manoclass)
			{
				case ManoClass.NO_HAND:
					manoClassText.text = "Manoclass: No Hand";
					break;
				case ManoClass.GRAB_GESTURE_FAMILY:
					manoClassText.text = "Manoclass: Grab Class";
					break;
				case ManoClass.PINCH_GESTURE_FAMILY:
					manoClassText.text = "Manoclass: Pinch Class";
					break;
				case ManoClass.POINTER_GESTURE_FAMILY:
					manoClassText.text = "Manoclass: Pointer Class";
					break;
				default:
					manoClassText.text = "Manoclass: No Hand";
					break;
			}
		}
	}

	/// <summary>
	/// Displays information regarding the detected manoclass
	/// </summary>
	/// <param name="manoGestureContinuous">Requires a continuous Gesture.</param>
	void DisplayContinuousGestures(ManoGestureContinuous manoGestureContinuous)
	{
		continuousGestureGizmo.SetActive(ShowContinuousGestures);
		if (ShowContinuousGestures)
		{
			switch (manoGestureContinuous)
			{
				case ManoGestureContinuous.CLOSED_HAND_GESTURE:
					continuousGestureText.text = "Continuous: Closed Hand";
					break;
				case ManoGestureContinuous.OPEN_HAND_GESTURE:
					continuousGestureText.text = "Continuous: Open Hand";
					break;
				case ManoGestureContinuous.HOLD_GESTURE:
					continuousGestureText.text = "Continuous: Hold";
					break;
				case ManoGestureContinuous.OPEN_PINCH_GESTURE:
					continuousGestureText.text = "Continuous: Open Pinch";
					break;
				case ManoGestureContinuous.POINTER_GESTURE:
					continuousGestureText.text = "Continuous: Pointing";
					break;
				case ManoGestureContinuous.NO_GESTURE:
					continuousGestureText.text = "Continuous: None";
					break;
				default:
					continuousGestureText.text = "Continuous: None";
					break;
			}
		}
	}

	/// <summary>
	/// Displays the hand side.
	/// </summary>
	/// <param name="handside">Requires a ManoMotion Handside.</param>
	void DisplayHandSide(HandSide handside)
	{
		handSideGizmo.SetActive(ShowHandSide);
		if (ShowHandSide)
		{
			switch (handside)
			{
				case HandSide.Palmside:
					handSideText.text = "Handside: Palm Side";
					break;
				case HandSide.Backside:
					handSideText.text = "Handside: Back Side";
					break;
				case HandSide.None:
					handSideText.text = "Handside: None";
					break;
				default:
					handSideText.text = "Handside: None";
					break;
			}
		}
	}

	///// <summary>
	///// Updates the visual information that showcases the hand state (how open/closed) it is
	///// </summary>
	///// <param name="gesture_info"></param>
	void DisplayHandState(int handstate)
	{
		handStatesGizmo.SetActive(ShowHandStates);
		if (ShowHandStates)
		{
			HighlightStatesToStateDetection(handstate);
		}
	}

	private ManoGestureTrigger previousTrigger;

	/// <summary>
	/// Display Visual information of the detected trigger gesture.
	/// In the case where a click is intended (Open pinch, Closed Pinch, Open Pinch) we are clearing out the visual information that are generated from the pick/drop
	/// </summary>
	/// <param name="triggerGesture">Requires an input of trigger gesture.</param>
	void DisplayTriggerGesture(ManoGestureTrigger triggerGesture, TrackingInfo trackingInfo)
	{
		if (triggerGesture != ManoGestureTrigger.NO_GESTURE)
		{
			if (_showPickTriggerGesture)
			{
				if (triggerGesture == ManoGestureTrigger.PICK)
				{
					TriggerDisplay(trackingInfo, ManoGestureTrigger.PICK);
				}
			}

			if (_showDropTriggerGesture)
			{
				if (triggerGesture == ManoGestureTrigger.DROP)
				{
					if (previousTrigger != ManoGestureTrigger.CLICK)
					{
						TriggerDisplay(trackingInfo, ManoGestureTrigger.DROP);
					}
				}
			}

			if (_showClickTriggerGesture)
			{
				if (triggerGesture == ManoGestureTrigger.CLICK)
				{
					TriggerDisplay(trackingInfo, ManoGestureTrigger.CLICK);
					if (GameObject.Find("PICK"))
					{
						GameObject.Find("PICK").SetActive(false);
					}
				}
			}

			if (_showGrabTriggerGesture)
			{
				if (triggerGesture == ManoGestureTrigger.GRAB_GESTURE)
					TriggerDisplay(trackingInfo, ManoGestureTrigger.GRAB_GESTURE);
			}

			if (_showReleaseTriggerGesture)
			{
				if (triggerGesture == ManoGestureTrigger.RELEASE_GESTURE)
					TriggerDisplay(trackingInfo, ManoGestureTrigger.RELEASE_GESTURE);
			}
		}

		previousTrigger = triggerGesture;
	}

	public List<GameObject> triggerObjectPool = new List<GameObject>();
	public int amountToPool = 20;

	/// <summary>
	/// Initializes the object pool for trigger gestures.
	/// </summary>
	private void InitializeTriggerPool()
	{
		for (int i = 0; i < amountToPool; i++)
		{
			GameObject newTriggerObject = Instantiate(triggerTextPrefab);
			newTriggerObject.transform.SetParent(transform);
			newTriggerObject.SetActive(false);
			triggerObjectPool.Add(newTriggerObject);
		}
	}

	/// <summary>
	/// Gets the current pooled trigger object.
	/// </summary>
	/// <returns>The current pooled trigger.</returns>
	private GameObject GetCurrentPooledTrigger()
	{
		for (int i = 0; i < triggerObjectPool.Count; i++)
		{
			if (!triggerObjectPool[i].activeInHierarchy)
			{
				return triggerObjectPool[i];
			}
		}
		return null;
	}

	/// <summary>
	/// Displays the visual information of the performed trigger gesture.
	/// </summary>
	/// <param name="bounding_box">Bounding box.</param>
	/// <param name="triggerGesture">Trigger gesture.</param>
	void TriggerDisplay(TrackingInfo trackingInfo, ManoGestureTrigger triggerGesture)
	{
		if (GetCurrentPooledTrigger())
		{
			GameObject triggerVisualInformation = GetCurrentPooledTrigger();

			triggerVisualInformation.SetActive(true);
			triggerVisualInformation.name = triggerGesture.ToString();
			triggerVisualInformation.GetComponent<TriggerGizmo>().InitializeTriggerGizmo(triggerGesture);
			triggerVisualInformation.GetComponent<RectTransform>().position = Camera.main.ViewportToScreenPoint(trackingInfo.palm_center);
		}
	}

	/// <summary>
	/// Visualizes the current hand state by coloring white the images up to that value and turning grey the rest
	/// </summary>
	/// <param name="stateValue">Requires a hand state value to assign the colors accordingly </param>
	void HighlightStatesToStateDetection(int stateValue)
	{
		for (int i = 0; i < stateImages.Length; i++)
		{
			if (i > stateValue)
			{
				stateImages[i].color = disabledStateColor;
			}
			else
			{
				stateImages[i].color = Color.white;
			}
		}
	}

	/// <summary>
	/// Highlights the edges of the screen according to the warning given by the ManoMotion Manager
	/// </summary>
	/// <param name="warning">Requires a warning.</param>
	void DisplayApproachingToEdgeFlags(Warning warning)
	{
		if (_showWarnings)
		{
			if (!flagHolderGizmo.activeInHierarchy)
			{
				flagHolderGizmo.SetActive(true);
			}

			rightFlag.SetActive(warning == Warning.WARNING_APPROACHING_RIGHT_EDGE);
			topFlag.SetActive(warning == Warning.WARNING_APPROACHING_UPPER_EDGE);
			leftFlag.SetActive(warning == Warning.WARNING_APPROACHING_LEFT_EDGE);
		}
		else
		{
			if (flagHolderGizmo.activeInHierarchy)
			{
				flagHolderGizmo.SetActive(false);
			}
		}
	}

	/// <summary>
	/// Displayes the smoothing slider.
	/// </summary>
	/// <param name="display">If set to <c>true</c> display.</param>
	public void ShouldDisplaySmoothingSlider(bool display)
	{
		smoothingSliderControler.SetActive(display);
	}

	/// <summary>
	/// Displays the smoothing slider that controls the level of delay applied to the calculations for Tracking Information.
	/// </summary>
	public void DisplaySmoothingSlider()
	{
		smoothingSliderControler.SetActive(_showSmoothingSlider);
	}

	/// <summary>
	/// Initializes the components of the Manoclass,Continuous Gesture and Trigger Gesture Gizmos
	/// </summary>
	void SetGestureDescriptionParts()
	{
		manoClassText = manoClassGizmo.transform.Find("Description").GetComponent<Text>();
		handSideText = handSideGizmo.transform.Find("Description").GetComponent<Text>();
		continuousGestureText = continuousGestureGizmo.transform.Find("Description").GetComponent<Text>();
	}

	/// <summary>
	/// Initializes the components for the visual illustration of warnings related to approaching edges flags.
	/// </summary>
	void InitializeFlagParts()
	{
		topFlag = flagHolderGizmo.transform.Find("Top").gameObject;
		rightFlag = flagHolderGizmo.transform.Find("Right").gameObject;
		leftFlag = flagHolderGizmo.transform.Find("Left").gameObject;
	}



	#endregion
}
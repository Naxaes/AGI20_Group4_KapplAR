﻿using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.XR.ARFoundation;



[RequireComponent(typeof(Light))]

public class LightEstimation : MonoBehaviour
{

    [SerializeField]
    private ARCameraManager arCameraManager;


    Light mainLight;

    public float? brightness { get;  set; }

    
    public float? colorTemperature { get;  set; }

    public Color? colorCorrection { get;  set; }


    public Vector3? mainLightDirection { get;  set; }

    public Color? mainLightColor { get;  set; }


    public float? mainLightIntensityLumens { get;  set; }

    
    //These dont work unfortunately in 2020
    public SphericalHarmonicsL2? sphericalHarmonics { get;  set; }

    void Awake()
    {
        mainLight = GetComponent<Light>();
    }


    void OnEnable()
    {
      
        if (arCameraManager != null)
            arCameraManager.frameReceived += FrameChanged;
    }
    void OnDisable()
    {
        
        if (arCameraManager != null)
            arCameraManager.frameReceived -= FrameChanged;
    }
    
    void FrameChanged(ARCameraFrameEventArgs obj)
    {
        if (obj.lightEstimation.averageBrightness.HasValue)
        {
            brightness = obj.lightEstimation.averageBrightness.Value;
            mainLight.intensity = brightness.Value;
        }

        if (obj.lightEstimation.averageColorTemperature.HasValue)
        {
            colorTemperature = obj.lightEstimation.averageColorTemperature.Value;
            mainLight.colorTemperature = colorTemperature.Value;
        }

        if (obj.lightEstimation.colorCorrection.HasValue)
        {
            colorCorrection = obj.lightEstimation.colorCorrection.Value;
            mainLight.color = colorCorrection.Value;
        }

        /*
         * 
         * Not working yet
        if (asd.averageIntensityInLumens.HasValue)
        {
        
            asd.averageIntensityInLumens = mainLightIntensityLumens.Value;
        }*/
    }
}
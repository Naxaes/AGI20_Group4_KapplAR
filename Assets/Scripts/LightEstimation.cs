using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.XR.ARFoundation;



[RequireComponent(typeof(Light))]

public class LightEstimation : MonoBehaviour
{

    [SerializeField]
    ARCameraManager arCameraManager;
    public ARCameraManager cameraManager
    {
        get { return arCameraManager; }
        set
        {
            if (arCameraManager == value)
                return;

            if (arCameraManager != null)
                arCameraManager.frameReceived -= FrameChanged;

            
            arCameraManager = value;


            if (arCameraManager != null & enabled)
                arCameraManager.frameReceived += FrameChanged;
        }
    }


    Light mainLight;

    public float? brightness { get; private set; }


    public float? colorTemperature { get; private set; }

    public Color? colorCorrection { get; private set; }




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
        else
        {
            brightness = null;
        }

        if (obj.lightEstimation.averageColorTemperature.HasValue)
        {
            colorTemperature = obj.lightEstimation.averageColorTemperature.Value;
            mainLight.colorTemperature = colorTemperature.Value;
        }
        else
        {
            colorTemperature = null;
        }

        if (obj.lightEstimation.colorCorrection.HasValue)
        {
            colorCorrection = obj.lightEstimation.colorCorrection.Value;
            mainLight.color = colorCorrection.Value;
        }
        else
        {
            colorCorrection = null;
        }

    }
}
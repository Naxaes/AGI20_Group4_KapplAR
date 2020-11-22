using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunShaderUpdate : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 camPos = Camera.current.transform.position;
        this.GetComponent<Renderer>().sharedMaterial.SetVector("_CameraPos", camPos);
    }
}

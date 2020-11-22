using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunRotation : MonoBehaviour
{
    public Transform target;
    public float rotationSpeed;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.RotateAround(new Vector3(target.position.x, this.gameObject.transform.position.y, target.position.z), Vector3.up, rotationSpeed * Time.deltaTime);
    }
}

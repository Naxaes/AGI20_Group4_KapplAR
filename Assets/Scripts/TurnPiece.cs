using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems; 

public class TurnPiece : MonoBehaviour
{

    bool rotating = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TurnXAxis()
    {
        if(!rotating && !EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
        StartCoroutine( Rotate( new Vector3(2.0f, 0f, 0f) ) );
    }

    public void TurnYAxis()
    {
        if(!rotating && !EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
        StartCoroutine(Rotate(new Vector3(0f, 2.0f, 0f)));
    }

    IEnumerator Rotate(Vector3 angles)
    {
        rotating = true;
        Quaternion startRotation = transform.rotation;
        Quaternion endRotation = Quaternion.Euler(angles) * startRotation;
        float duration = 0.002f;

        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            transform.rotation = Quaternion.Slerp(startRotation, endRotation, t / duration);
            yield return null;
        }
        transform.rotation = endRotation;
        rotating = false;
    }

    
}

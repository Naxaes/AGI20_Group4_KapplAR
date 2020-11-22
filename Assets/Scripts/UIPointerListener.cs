using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


/* 
 * Listen to button events and  
 */
public class UIPointerListener : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{

    bool isButtonDown = false;
    bool isRotating = false;
    public GameObject indicator;
    public bool rotateX;
    public bool rotateY;
    float timeMoved;
    Vector2 prevTouchPos;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timeMoved += Time.deltaTime;
        if (Input.GetTouch(0).phase == TouchPhase.Began)
        {
            prevTouchPos = Input.GetTouch(0).position;
            timeMoved = 0.0f;
        }

        if (isButtonDown)
        {
            if (!isRotating && rotateX)
                TurnXAxis();
            else if (!isRotating && rotateY)
                TurnYAxis();
        }

        if (Input.GetTouch(0).phase == TouchPhase.Moved && timeMoved >= 0.02)
        {
            float dx = prevTouchPos.x - Input.GetTouch(0).position.x;
            float dy = prevTouchPos.y - Input.GetTouch(0).position.y;
            if (Mathf.Abs(dx) > Mathf.Abs(dy))
            {
                TurnYAxis(-dx / 1.5f, timeMoved);
            } else
            {
                TurnXAxis(dy / 1.5f, timeMoved);
            }
            prevTouchPos = Input.GetTouch(0).position;
            timeMoved = 0.0f;
        }
    }


    public void OnPointerDown(PointerEventData eventData) {
        isButtonDown = true; 
    }

    public void OnPointerUp(PointerEventData eventData) {
        isButtonDown = false;
    }

    public void TurnXAxis(float x, float duration)
    {
        if (!isRotating && !EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
            StartCoroutine(Rotate(new Vector3(x, 0f, 0f), duration));
    }

    public void TurnZAxis(float x, float duration)
    {
        if (!isRotating && !EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
            StartCoroutine(Rotate(new Vector3(0.0f, 0f, x), duration));
    }

    public void TurnXAxis()
    {
        if (!isRotating && !EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
            StartCoroutine(Rotate(new Vector3(2.0f, 0f, 0f), 0.02f));
    }

    public void TurnXYAxis(float x, float y)
    {
        if (!isRotating && !EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
            StartCoroutine(Rotate(new Vector3(x, y, 0.0f), 0.02f));
    }

    public void TurnXZAxis(float x, float z)
    {
        if (!isRotating && !EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
            StartCoroutine(Rotate(new Vector3(x, 0.0f, z), 0.02f));
    }

    public void TurnYAxis(float y, float duration)
    {
        if (!isRotating && !EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
            StartCoroutine(Rotate(new Vector3(0f, y, 0f), duration));
    }

    public void TurnYAxis()
    {
        if (!isRotating && !EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
            StartCoroutine(Rotate(new Vector3(0f, 2.0f, 0f), 0.02f));
    }

    IEnumerator Rotate(Vector3 angles, float duration)
    {
        isRotating = true;
        Quaternion startRotation = indicator.transform.rotation;
        Quaternion endRotation = Quaternion.Euler(angles) * startRotation;

        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            indicator.transform.rotation = Quaternion.Slerp(startRotation, endRotation, t / duration);
            yield return null;
        }
        indicator.transform.rotation = endRotation;
        isRotating = false;
    }
}

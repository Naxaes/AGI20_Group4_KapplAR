using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetReachedByPiece : MonoBehaviour
{
    const float VALIDATION_TIME = 2f;
    bool hasEntered = false;
    float secondsBeforeValidation = VALIDATION_TIME;
    bool hasTouchedThisTrame = false;

    public delegate void TargetReachedEvent();
    public static event TargetReachedEvent onValidation;
    public static event TargetReachedEvent onEnter;
    public static event TargetReachedEvent onStay;
    public static event TargetReachedEvent onExit;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(!hasTouchedThisTrame)
        {
            secondsBeforeValidation = VALIDATION_TIME;
            if(hasEntered)
            {
                onExit?.Invoke();
                hasEntered = false;
            }
        }
        hasTouchedThisTrame = false;
    }

    private void OnTriggerStay(Collider other)
    {
        GameObject piece = other.gameObject;
        if(piece.name == "Game piece" && !hasTouchedThisTrame)
        {
            if(!hasEntered)
            {
                hasEntered = true;
                onEnter.Invoke();
            }

            secondsBeforeValidation -= Time.deltaTime;
            hasTouchedThisTrame = true;
            onStay?.Invoke();
            
        }
        if(secondsBeforeValidation <= 0)
        {
            onValidation?.Invoke();
        }
    }


}

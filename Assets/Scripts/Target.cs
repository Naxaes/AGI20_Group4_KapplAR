using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
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
    private Vector3 initialScale;


    // Start is called before the first frame update
    void Start()
    {
        initialScale = transform.GetChild(0).localScale;
    }

    void Update()
    {
        Debug.Log(initialScale * (Convert.ToSingle(Math.Cos(Time.time)) * 1f + 1.00f));
        transform.GetChild(0).localScale = initialScale * (Convert.ToSingle(Math.Cos(2*Time.time)) * 0.05f + 1.00f);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        TargetReachedTest();
    }

    void OnTriggerStay(Collider other)
    {
        GameObject piece = other.gameObject;
        if(piece.tag == "Game Piece" && !hasTouchedThisTrame)
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

    void TargetReachedTest()
    {
        if (!hasTouchedThisTrame)
        {
            secondsBeforeValidation = VALIDATION_TIME;
            if (hasEntered)
            {
                onExit?.Invoke();
                hasEntered = false;
            }
        }
        hasTouchedThisTrame = false;
    }


}

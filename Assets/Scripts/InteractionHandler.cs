using System.Collections;
using System.Collections.Generic;
using UnityEngine;

interface IInteractionHandler
{
    void InteractionUpdate();
    void InteractionFixedUpdate();
    void InteractionStart();
}

public class InteractionHandler : MonoBehaviour
{

    IInteractionHandler handler;

    void setInteractionHander(IInteractionHandler h)
    {
        handler = h;
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        handler.InteractionUpdate();
    }

    void FixedUpdate()
    {
        handler.InteractionFixedUpdate();
    }
}

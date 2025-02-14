﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TerrainEvents : MonoBehaviour
{
    Rigidbody rb;
    GameObject gameFloor;
    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("StartEarthquake", 10.0f, 20.0f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartEarthquake()
    {
        if(gameFloor == null)
        {
            gameFloor = GameObject.FindGameObjectWithTag("Game Floor");
        }
        StartCoroutine(EarthquakeCoroutine());
    }

    IEnumerator EarthquakeCoroutine()
    {
        const float duration = 5f;

        Vector3 initialGamefloorPosition = gameFloor.transform.position;
        for (float t = 0f; t < duration; t += Time.deltaTime)
        {
            Vector3 transformation = Random.onUnitSphere * 0.15f;
            gameFloor.transform.position = (initialGamefloorPosition + transformation);

            yield return null;
        }
        gameFloor.transform.position =initialGamefloorPosition;

        //if(rb == null)
        //{
        //    rb = GameObject.FindGameObjectWithTag("Game Floor").GetComponent<Rigidbody>();
        //}
        //Vector3 initialGamefloorPosition = rb.position;
        //for (float t = 0f; t < duration; t += Time.deltaTime)
        //{
        //    Vector3 transformation = Random.onUnitSphere * 0.1f;
        //    rb.MovePosition(initialGamefloorPosition + transformation);

        //    yield return null;
        //}
        //rb.MovePosition(initialGamefloorPosition);
        yield break;
    }

    
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockCollision : MonoBehaviour
{
    // Start is called before the first frame update
    float collisionTimer = 3.0f;
    bool activateTimer = false;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (activateTimer)
        {
            collisionTimer -= Time.deltaTime;
        }
        if (collisionTimer <= 0.0f)
        {
            collisionTimer = 3.0f;
            activateTimer = false;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        //Debug.Log("collision: " + collision.gameObject.tag+" "+ collision.gameObject.name);
        if (collision.gameObject.CompareTag("Game Piece"))
        {
            activateTimer = true;
            Vibration.VibratePop();
        }
    }
} 
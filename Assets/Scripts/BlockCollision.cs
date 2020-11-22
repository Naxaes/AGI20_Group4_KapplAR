using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockCollision : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnCollisionEnter(Collision collision)
    {
        //Debug.Log("collision: " + collision.gameObject.tag+" "+ collision.gameObject.name);
        if (collision.gameObject.CompareTag("Game Piece"))
        {
            Vibration.VibratePop();
        }
    }
} 
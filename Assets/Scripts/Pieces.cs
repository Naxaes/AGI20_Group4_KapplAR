using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pieces : MonoBehaviour
{

    public GameObject particlePrefab;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Destroy()
    {
        foreach(Transform child in transform)
        {
            Instantiate(particlePrefab, child.position, child.rotation);
            GameObject.Destroy(child.gameObject);
        }
    }
}

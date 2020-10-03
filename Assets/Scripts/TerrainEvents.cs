using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainEvents : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartEarthquake()
    {
        Transform gameFloor = GameObject.Find("Game Floor").transform;
        Transform terrain = gameFloor.Find("Terrain");
        Transform environment = gameFloor.Find("Environment");
        StartCoroutine(EarthquakeCoroutine(terrain,environment));
    }

    IEnumerator EarthquakeCoroutine(Transform terrain, Transform environment)
    {
        const float duration = 5f;
        Vector3 initialTerrainPosition = terrain.position;
        Vector3 initialEnvironmentPosition = environment.position;
        for (float t = 0f; t < duration; t += Time.deltaTime)
        {
            Vector3 transformation = Random.onUnitSphere * 0.05f;
            terrain.position.Scale(transformation + Vector3.one);
            environment.position.Scale(transformation + Vector3.one);

            yield return null;
        }
        terrain.position = initialTerrainPosition;
        environment.position = initialEnvironmentPosition;
        yield break;
    }
}

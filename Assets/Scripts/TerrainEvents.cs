using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TerrainEvents : MonoBehaviour
{
    Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
        rb =GameObject.FindGameObjectWithTag("Game Floor").GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartEarthquake()
    {
        StartCoroutine(EarthquakeCoroutine());
    }

    IEnumerator EarthquakeCoroutine()
    {
        const float duration = 5f;
        Vector3 initialGamefloorPosition = rb.position;
        for (float t = 0f; t < duration; t += Time.deltaTime)
        {
            Vector3 transformation = Random.onUnitSphere * 0.1f;
            rb.MovePosition(initialGamefloorPosition + transformation);

            yield return null;
        }
        rb.MovePosition(initialGamefloorPosition);
        yield break;
    }

    public void StartWind()
    {
        Rigidbody[] gamePiecesRb = GameObject.FindGameObjectsWithTag("Game Piece").Select(x => x.GetComponent<Rigidbody>()).ToArray();
        StartCoroutine(WindCoroutine(gamePiecesRb));
    }

    IEnumerator WindCoroutine(Rigidbody [] gamePiecesRb)
    {
        const float duration = 5f;
        Debug.Log(gamePiecesRb.Length);
        for (float t = 0f; t < duration; t += Time.deltaTime)
        {
            foreach (Rigidbody gamePieceRb in gamePiecesRb)
            {
                Debug.Log(gamePieceRb);
                gamePieceRb.AddForce(new Vector3(0f, 0f, 10000f), new ForceMode());

                yield return null;
            }
        }
        yield break;
    }
}

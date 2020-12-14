using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;

public class wind : MonoBehaviour
{

    const float RefValue = 0.1f;

    const int sampleWindow = 512;

    AudioClip audioClip;
    float[] waveData;
    const string micName = "Android camcorder input";

    const float treshold = 0f;

    public GameObject windParticles;

    ParticleSystem windParticleSystem;

    Quaternion lastCameraPosition;

    // Start is called before the first frame update
    void Start()
    {
        // audioSource = GetComponent<AudioSource>();
        var devices = Microphone.devices;
        foreach (var d in devices)
        {
            Debug.Log(d);
        }
        // int maxfreq, minfreq;
        // Microphone.GetDeviceCaps("Android camcorder input", out minfreq ,out maxfreq);
        audioClip = Microphone.Start(null, true, 5, AudioSettings.outputSampleRate);
        waveData = new float[sampleWindow];
        windParticleSystem = windParticles.GetComponent<ParticleSystem>();
        lastCameraPosition = Camera.current.transform.rotation;
        // audioSource.outputAudioMixerGroup = audioMixerGroup;
        // _samples = new float[QSamples];
        // _fSample = AudioSettings.outputSampleRate;
        // audioSource.PlayDelayed(1);
    }

    // Update is called once per frame
    void Update()
    {
        float audioLvl = getAudioLevel();
        var main = windParticleSystem.main;
        if (audioLvl > treshold)
        {
            Debug.Log(audioLvl);
            if(!windParticleSystem.isPlaying) windParticleSystem.Play();
            main.startSpeed = Mathf.Clamp(audioLvl,0,10);
            StartWind(1000);
        }
        else
        {
            windParticleSystem.Stop();
            main.startSpeed = 0;
        }
        float rotationY = (Camera.current.transform.rotation.eulerAngles.y - lastCameraPosition.eulerAngles.y) % 360;
        windParticles.transform.Rotate(0f, rotationY, 0f, Space.World);
        lastCameraPosition = Camera.current.transform.rotation;

        // if(!audioSource.isPlaying) 
        // Debug.Log(getAudioLevel());
    }

    float getAudioLevel()
    {
        float rmsVal;
        float dbVal;
        int micPosition = (Microphone.GetPosition(micName) - (sampleWindow + 1)) % audioClip.samples;
        audioClip.GetData(waveData, micPosition);

        float sum = 0;
        for (int i = 0; i < sampleWindow; ++i)
        {
            sum += waveData[i] * waveData[i];
        }

        rmsVal = Mathf.Sqrt(sum / sampleWindow); // rms = square root of average
        dbVal = 20 * Mathf.Log10(rmsVal / RefValue); // calculate dB
        if (dbVal < -160) dbVal = -160; // clamp it to -160dB min

        return dbVal;
    }

    public void StartWind(float force)
    {
        Rigidbody[] gamePiecesRb = GameObject.FindGameObjectsWithTag("Game Piece").Select(x => x.GetComponent<Rigidbody>()).ToArray();
        foreach (Rigidbody gamePieceRb in gamePiecesRb)
            {
                gamePieceRb.AddForce(Camera.current.transform.forward * force, new ForceMode());
            }
        // StartCoroutine(WindCoroutine(gamePiecesRb));
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
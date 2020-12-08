using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    //Here add more sounds by placing them inside the Resources/Sounds folder
    public enum Sound
    {
        placeKapla,
        //Add more effects or whatever

    }

    private AudioSource audioSource;

    private float volume = .5f;

    private void Awake()
    {
        Instance = this;
        audioSource = GetComponent<AudioSource>();

       

    }
    public void PlaySound()
    { 
        
        audioSource.PlayOneShot(Resources.Load<AudioClip>(Sound.placeKapla.ToString()));

    }

     public void IncreaseVolume()
    {
        volume += .1f;
        volume = Mathf.Clamp01(volume);
    }

    public void DecreaseVolume()
    {
        volume -= .1f;
        volume = Mathf.Clamp01(volume);
    }

    public float GetVolume()
    {
        return volume;
    }

}

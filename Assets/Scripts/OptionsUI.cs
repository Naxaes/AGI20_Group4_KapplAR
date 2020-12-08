using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro; //A library because we need refernces to the text objects
public class OptionsUI : MonoBehaviour
{
    [SerializeField] private SoundManager soundManager;
    private TextMeshProUGUI soundVolume;
    private TextMeshProUGUI musicVolume;


    [SerializeField] private MusicManager musicManager;

    private void Awake()
    {
        soundVolume = transform.Find("soundVolume").GetComponent<TextMeshProUGUI>();

        musicVolume = transform.Find("musicVolume").GetComponent<TextMeshProUGUI>();

        transform.Find("increaseSoundBtn").GetComponent<Button>().onClick.AddListener(()=> {
           // Debug.Log("increasing sound By button...!");
            soundManager.IncreaseVolume();
            UpdateRelevantText();
        
        });

        transform.Find("decreaseSoundBtn").GetComponent<Button>().onClick.AddListener(() => {

            soundManager.DecreaseVolume();
            UpdateRelevantText();
        });

        transform.Find("increaseMusicBtn").GetComponent<Button>().onClick.AddListener(() => {
            musicManager.IncreaseVolume();
            UpdateRelevantText();


        });

        transform.Find("decreaseMusicBtn").GetComponent<Button>().onClick.AddListener(() => {
            musicManager.DecreaseVolume();
            UpdateRelevantText();


        });

       

    }


    private void Start()
    {
        gameObject.SetActive(false);
        //Debug.Log("increasing sound Start");

        UpdateRelevantText();
    }

    private void UpdateRelevantText()
    {
        soundVolume.SetText(Mathf.RoundToInt(soundManager.GetVolume()*10).ToString());
        //Debug.Log("increasing sound Update");

        musicVolume.SetText(Mathf.RoundToInt(musicManager.GetVolume() * 10).ToString());

    }

    public void ToggleOptionsMenu()
    {
        gameObject.SetActive(!gameObject.activeSelf);

        if (gameObject.activeSelf)
        {
            Time.timeScale = 0f;
        }
        else
        {
            Time.timeScale = 1f;
        }
    
    
    }

    

}

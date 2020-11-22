using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartMenu : MonoBehaviour
{

    public GameObject LevelList;
    public GameObject LevelDisplay;
    private List<GameObject> Levels = new List<GameObject>();
    private const float LevelWidth = 0.72f;
    private int SelectedLevel;

    // Start is called before the first frame update
    void Start()
    {
        foreach(Transform child in LevelList.transform)
        {
            Levels.Add(child.gameObject);
        }
        SetSelectedLevel(0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadSelectedLevel()
    {
        Levels[SelectedLevel].GetComponent<Level>().Reset();
    }

    private void SetSelectedLevel(int newSelectedLevel)
    {
        SelectedLevel = newSelectedLevel;
        LevelDisplay.GetComponent<Text>().text = Levels[SelectedLevel].GetComponent<Level>().LevelName;
    }

    public void OnLevelSelectionScroll(Vector2 pos)
    {
        //Debug.Log(pos.x);
        int newSelectedLevel = (int) Math.Min(Levels.Count - 1, Math.Max(0, Math.Floor( (pos.x + LevelWidth/2 ) / LevelWidth)));
        if(SelectedLevel != newSelectedLevel)
        {
            SetSelectedLevel(newSelectedLevel);
        }
    }
}

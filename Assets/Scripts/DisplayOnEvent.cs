using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayOnEvent : MonoBehaviour
{
    int currentInteractionMode = 0;
    static string[] modes = { "Place", "Slice" };

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeInteractionMode()
    {
        transform.Find("Change Mode").Find("Mode").GetComponent<Text>().text = modes[++currentInteractionMode % modes.Length];
    }

    private void OnEnable()
    {
        Target.onValidation += ()=> DisplayMessage(true);
        Target.onEnter += () =>  DisplayCounter(true);
        Target.onExit += () =>  DisplayCounter(false);
    }

    private void OnDisable()
    {
        Target.onValidation -= ()=>DisplayMessage(true);
        Target.onEnter -= () =>  DisplayCounter(true);
        Target.onExit -= () =>  DisplayCounter(false);

    }

    public void DisplayMessage(bool condition)
    {
        transform.Find("WinMessage").gameObject.SetActive(condition);
        if(condition) DisplayCounter(false);
    }

    public void DisplayCounter(bool condition)
    {
        transform.Find("Counter").gameObject.SetActive(condition);
        if(condition)
        {
            StartCoroutine(StartCounter());
        } else
        {
            StopCoroutine(StartCounter());
            Text text = transform.Find("Counter").gameObject.GetComponent<Text>();
            text.text = "2.00";
        }
    }

    IEnumerator StartCounter()
    {
        for(float t = 2f; t >= 0; t-= Time.deltaTime)
        {
            Text text = transform.Find("Counter").gameObject.GetComponent<Text>();
            text.text = t.ToString("F2");
            yield return null;
        }
    }

}

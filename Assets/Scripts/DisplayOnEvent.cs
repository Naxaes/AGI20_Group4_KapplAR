using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayOnEvent : MonoBehaviour
{


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnEnable()
    {
        TargetReachedByPiece.onValidation += ()=> DisplayMessage(true);
        TargetReachedByPiece.onEnter += () =>  DisplayCounter(true);
        TargetReachedByPiece.onExit += () =>  DisplayCounter(false);
    }

    private void OnDisable()
    {
        TargetReachedByPiece.onValidation -= ()=>DisplayMessage(true);
        TargetReachedByPiece.onEnter -= () =>  DisplayCounter(true);
        TargetReachedByPiece.onExit -= () =>  DisplayCounter(false);

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

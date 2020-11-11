using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Level2 : Level
{
    public override void Reset()
    {
        SceneManager.LoadScene("Level2");
    }

    // Start is called before the first frame update
    void Awake()
    {
        LevelName = "Level 2";
        inventory = new Inventory();
        inventory.AddItem(new WoodBloc(), 10);
        inventory.AddItem(new JellyBloc(), 0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

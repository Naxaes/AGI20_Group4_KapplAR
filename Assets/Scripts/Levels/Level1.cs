using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Level1 : Level
{
    public override void Reset()
    {
        SceneManager.LoadScene("Level1");
    }

    // Start is called before the first frame update
    void Awake()
    {
        LevelName = "Level 1";
        inventory = new Inventory();
        inventory.AddItem(new WoodBloc(), 100);
        inventory.AddItem(new JellyBloc(), 5);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

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
        inventory = new Inventory();
        inventory.AddItem(new WoodBloc(), 100);
        inventory.AddItem(new BlocTest2(), 1);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

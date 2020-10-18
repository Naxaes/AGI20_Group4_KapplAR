using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level1 : Level
{

    // Start is called before the first frame update
    void Awake()
    {
        inventory = new Inventory();
        inventory.AddItem(new WoodBloc(), 5);
        inventory.AddItem(new BlocTest2(), 1);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

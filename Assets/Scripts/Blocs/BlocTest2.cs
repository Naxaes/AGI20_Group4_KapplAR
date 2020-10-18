using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlocTest2 : Bloc
{
    public BlocTest2()
    {
        blocId = 2;
        gameObject = (GameObject)Resources.Load("Prefabs/Game piece");
        label = "Test2";
    }
}

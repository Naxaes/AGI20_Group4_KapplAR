using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JellyBloc : Bloc
{
    public JellyBloc()
    {
        blocId = 2;
        gameObject = (GameObject)Resources.Load("Prefabs/Jelly piece");
        label = "Jelly";
    }
}

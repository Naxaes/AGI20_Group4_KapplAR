using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WoodBloc : Bloc
{
    public WoodBloc()
    {
        blocId = 1;
        gameObject = (GameObject) Resources.Load("Prefabs/Blocks/Game piece");
        label = "Kapla";
    }
}

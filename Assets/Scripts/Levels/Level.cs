using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Level : MonoBehaviour
{
    public void Start()
    {
        Vibration.Init();
    }
    public Inventory inventory { get; protected set; }
    public string LevelName { get; protected set; }

    public abstract void Reset();
}

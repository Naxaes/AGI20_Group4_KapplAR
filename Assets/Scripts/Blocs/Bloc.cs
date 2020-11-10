using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Bloc : IEquatable<Bloc>
{
    public GameObject gameObject;
    public int blocId { get; protected set; }
    public String label { get; protected set;}

    public bool Equals(Bloc other)
    {
        return this.blocId == other.blocId;
    }
}

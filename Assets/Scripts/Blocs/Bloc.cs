using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Bloc : IEquatable<Bloc>
{
    public GameObject gameObject;
    protected int blocId;
    protected String label;

    public bool Equals(Bloc other)
    {
        return this.blocId == other.blocId;
    }
}

using System;
using UnityEngine;

public class TypePopupAttribute : PropertyAttribute
{
    public int Selected { get; set; }
    public Type Type { get; private set; }

    public TypePopupAttribute(Type type) 
    { 
        Selected = 0;
        Type = type;
    }
}

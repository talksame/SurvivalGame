using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour {

    public Type type;

    public enum Type
    {
        Normal,
        Wall,
        Foundation,
        Pillar
    }
}

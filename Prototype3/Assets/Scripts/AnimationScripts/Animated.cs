/******************************************************************************
 * Author: Brad Dixon
 * FileName: Animated.cs
 * Creation Date: 4/21/2025
 * Brief: Contains values for an animated object
 * ***************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Animated
{
    public Animator animator;

    public enum ParamType
    {
        Float,
        Int,
        Bool,
        Trigger
    }

    public ParamType paramType;

    public string animationName;
}

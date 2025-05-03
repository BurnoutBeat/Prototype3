/******************************************************************************
 * Author: Brad Dixon
 * FileName: Animated.cs
 * Creation Date: 4/21/2025
 * Brief: Contains values for an animated object
 * ***************************************************************************/
using UnityEngine;

[System.Serializable]
public class Animated
{
    public Animator animator;
    public AnimationClip animation;

    public string paramName;

    public string animationName;
}

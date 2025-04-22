/******************************************************************************
 * Author: Brad Dixon
 * File Name: AnimationController.cs
 * Creation Date: 4/21/2025
 * Brief: Plays an animation when called
 * ***************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    [SerializeField] private List<Animated> clips = new List<Animated>();
    SpriteRenderer animSprite;
    
    private void Awake()
    {
        animSprite = GetComponent<SpriteRenderer>();
    }

    /// <summary>
    /// Plays the corresponding animtion tied to the name
    /// </summary>
    /// <param name="name"></param>
    public void PlayAnim(string name)
    {
        Animated clip = clips[clips.FindIndex(i => i.animationName == name)];
        clip.animator.Play(clip.animation.name);
    }

    /// <summary>
    /// Passes a parameter to a animation
    /// </summary>
    /// <param name="name"></param>
    public void SetParameter(string name)
    {
        Animated clip = clips[clips.FindIndex(i => i.animationName == name)];
        switch(clip.paramType)
        {
            case Animated.ParamType.Float:
                clip.animator.SetFloat(clip.paramName, clip.floatValue);
                break;
            case Animated.ParamType.Int:
                clip.animator.SetInteger(clip.paramName, clip.intValue);
                break;
            case Animated.ParamType.Bool:
                clip.animator.SetBool(clip.paramName, clip.boolValue);
                break;
            case Animated.ParamType.Trigger:
                clip.animator.SetTrigger(clip.paramName);
                break;
            default:
                Debug.Log("Switch didn't work");
                break;
        }
    }
}

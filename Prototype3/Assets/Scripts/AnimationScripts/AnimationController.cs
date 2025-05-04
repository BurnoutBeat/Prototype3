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
    public static AnimationController Instance;
    [SerializeField] private List<Animated> clips = new List<Animated>();
    SpriteRenderer animSprite;
    
    /// <summary>
    /// Makes a singleton instance
    /// </summary>
    private void Awake()
    {
        //Makes sure there is one singleton instance
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
        animSprite = GetComponent<SpriteRenderer>();
    }

    /// <summary>
    /// Plays the corresponding animtion tied to the name
    /// </summary>
    /// <param name="name"></param>
    public void PlayAnim(string name)
    {
        Animated clip = clips[clips.FindIndex(i => i.animationName == name)];
        //clip.animator.gameObject.GetComponent<SpriteRenderer>().enabled = true;
        //clip.animator.enabled = true;
        clip.animator.Play(clip.animation.name);
    }

    /// <summary>
    /// Allows to set a custom float for the parameter
    /// </summary>
    /// <param name="name"></param>
    /// <param name="value"></param>
    public void SetParameter(string name, float value)
    {
        Animated clip = clips[clips.FindIndex(i => i.animationName == name)];
        clip.animator.SetFloat(clip.paramName, value);
    }

    /// <summary>
    /// Allows to set a custom int for the parameter
    /// </summary>
    /// <param name="name"></param>
    /// <param name="value"></param>
    public void SetParameter(string name, int value)
    {
        Animated clip = clips[clips.FindIndex(i => i.animationName == name)];
        clip.animator.SetInteger(clip.paramName, value);
    }

    /// <summary>
    /// Allows to set a custom bool for the parameter
    /// </summary>
    /// <param name="name"></param>
    /// <param name="value"></param>
    public void SetParameter(string name, bool value)
    {
        Animated clip = clips[clips.FindIndex(i => i.animationName == name)];
        clip.animator.SetBool(clip.paramName, value);
    }

    /// <summary>
    /// Passes a trigger as the anim parameter
    /// </summary>
    /// <param name="name"></param>
    /// <param name="value"></param>
    public void SetParameter(string name)
    {
        Animated clip = clips[clips.FindIndex(i => i.animationName == name)];
        clip.animator.SetTrigger(clip.paramName);
    }

    /// <summary>
    /// For this to work, add a trigger parameter to the animator called stop.
    /// This goes from the any state to an animation clip of nothing
    /// </summary>
    /// <param name="name"></param>
    public void StopAnimation(string name)
    {
        Animated clip = clips[clips.FindIndex(i => i.animationName == name)];
        clip.animator.SetTrigger("stop");
    }
}

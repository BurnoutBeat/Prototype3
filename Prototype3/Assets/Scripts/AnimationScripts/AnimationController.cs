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
}

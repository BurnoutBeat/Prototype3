using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class animTesting : MonoBehaviour
{
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            AnimationController.Instance.PlayAnim("Test2");
        }

        if(Input.GetKeyDown(KeyCode.Q))
        {
            AnimationController.Instance.StopAnimation("Test2");
        }

        if(Input.GetKeyDown(KeyCode.Z))
        {
            AnimationController.Instance.SetParameter("Test", true);
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            AnimationController.Instance.SetParameter("Test", false);
        }
    }
}

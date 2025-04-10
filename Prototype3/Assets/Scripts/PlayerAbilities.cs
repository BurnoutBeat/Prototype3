using UnityEngine;
using System.Collections;

public class PlayerAbilities : MonoBehaviour
{
    [SerializeField] Rigidbody rb;
    [SerializeField] GameObject player;

    public void Dash(float power)
    {
        rb.AddForce(player.transform.forward * power, ForceMode.Force);
    }
}
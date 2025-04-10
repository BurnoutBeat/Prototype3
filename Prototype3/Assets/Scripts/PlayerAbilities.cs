using UnityEngine;
using System.Collections;

public class PlayerAbilities : MonoBehaviour
{
    [SerializeField] Rigidbody rb;
    [SerializeField] GameObject player;
    [SerializeField] float hangTime = 1f;

    public void Dash(float power)
    {
        rb.useGravity = false;
        rb.AddForce(player.transform.forward * power, ForceMode.Force);

        StartCoroutine(DashStop());
    }

    private IEnumerator DashStop()
    {
        yield return new WaitForSeconds(hangTime);

        rb.useGravity = true;
    }
}
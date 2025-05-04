using UnityEngine;
using System.Collections;

public class PlayerAbilities : MonoBehaviour
{
    [SerializeField] Rigidbody rb;
    [SerializeField] float dashDistance = 10f;
    [SerializeField] float dashSpeed = 50f;
    private PlayerBehavior playerBehavior;
    [SerializeField] Animator m_Animator;

    private void Start()
    {
        playerBehavior = GetComponent<PlayerBehavior>();
    }

    public void Dash()
    {
        StartCoroutine(DashRoutine());
        m_Animator.SetTrigger("dash");
        StartCoroutine(StopDashAnimation());
    }

    private IEnumerator DashRoutine()
    {
        rb.AddForce(transform.forward * dashSpeed);
        rb.AddForce(transform.up * dashDistance);
        yield return null;
        /*rb.MovePosition(rb.position - transform.forward * 0.5f);

        Vector3 dashDir = rb.transform.forward;
        dashDir.y = 0f;
        dashDir.Normalize();

        float distanceTraveled = 0f;
        Vector3 lastPosition = rb.position;

        while (distanceTraveled < dashDistance)
        {
            float moveStep = dashSpeed * Time.deltaTime;

            if (rb.SweepTest(dashDir, out RaycastHit hit, moveStep))
            {
                rb.MovePosition(hit.point);
                break;
            }

            Vector3 newPos = rb.position + dashDir * moveStep;
            rb.MovePosition(newPos);

            distanceTraveled += Vector3.Distance(rb.position, lastPosition);
            lastPosition = rb.position;

            yield return null;
        }*/
    }

    private IEnumerator StopDashAnimation()
    {
        yield return new WaitForSeconds(playerBehavior.crouchChargeTime);
        m_Animator.ResetTrigger("dash");
    }
}
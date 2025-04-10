using UnityEngine;
using System.Collections;

public class PlayerAbilities : MonoBehaviour
{
    [SerializeField] Rigidbody rb;
    [SerializeField] GameObject player;
    [SerializeField] float dashDistance = 5f;
    [SerializeField] float dashSpeed = 50f;

    public void Dash()
    {
        StartCoroutine(DashRoutine());
    }

    private IEnumerator DashRoutine()
    {
        Vector3 dashDir = player.transform.forward;
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
        }
    }
}
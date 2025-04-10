using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinBehavior : MonoBehaviour
{
    public float rotationSpeed = 100f;
    private CollectableControler cc;
    private void Start()
    {
        cc = GameObject.FindObjectOfType<CollectableControler>();
        print(cc.gameObject);
    }
    void Update()
    {
        transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<PlayerBehavior>()) {
            cc.Collected();
            Destroy(gameObject);
        }
    }
}

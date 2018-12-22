using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeteorScript : MonoBehaviour
{
    TimeManager timeManager;
    Rigidbody rigidbody;
    public Vector3 velocity = new Vector3(-2, -2, 2);
    public Vector3 angularVelocity = new Vector3(1, 1.5f, 2.5f);
    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        timeManager = FindObjectOfType<TimeManager>();

        SetSpeed();
    }

    public void SetSpeed()
    {
        if (timeManager.running)
        {
            rigidbody.velocity = velocity;
            rigidbody.angularVelocity = angularVelocity;
        }
        else
        {
            rigidbody.velocity = new Vector3(0, 0, 0);
            rigidbody.angularVelocity = new Vector3(0, 0, 0);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Tile" || transform.localScale.x < collision.rigidbody.transform.localScale.x)
            Destroy(gameObject);
    }

    private void Update()
    {
        if (timeManager.hasChanged) {
            SetSpeed();
        }

        if (transform.position.y < 0)
        {
            Destroy(gameObject);
        }
    }

}

using Manager;
using UnityEngine;

public class MeteorScript : MonoBehaviour {
    public Vector3 angularVelocity = new Vector3(1, 1.5f, 2.5f);
    private new Rigidbody rigidbody;
    private TimeManager timeManager;

    public Vector3 velocity = new Vector3(-2, -2, 2);

    // Start is called before the first frame update
    private void Start() {
        rigidbody = GetComponent<Rigidbody>();
        timeManager = FindObjectOfType<TimeManager>();

        SetSpeed();
    }

    public void SetSpeed() {
        if (timeManager.running) {
            rigidbody.velocity = velocity;
            rigidbody.angularVelocity = angularVelocity;
        } else {
            rigidbody.velocity = new Vector3(0, 0, 0);
            rigidbody.angularVelocity = new Vector3(0, 0, 0);
        }
    }

    private void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.tag == "Tile" || transform.localScale.x < collision.rigidbody.transform.localScale.x) {
            Destroy(gameObject);
        }
    }

    private void Update() {
        if (timeManager.hasChanged) {
            SetSpeed();
        }

        if (transform.position.y < 0) {
            Destroy(gameObject);
        }
    }
}

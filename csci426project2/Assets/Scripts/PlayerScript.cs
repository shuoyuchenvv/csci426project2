using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    // movement
    public float acceleration = 10f; // how fast player speeds up
    public float maxSpeed = 5f; // top speed
    public float drag = 2f; // water resistance effect

    // eating mechanic
    public float zoomOutFactor = 0.2f; // camera zooms out slightly
    public float growthFactor = 0.1f; // how much player grows per enemy eaten
    public float slowDownFactor = 0.2f; // how much speed decreases per growth

    private Rigidbody2D rb;
    private Vector2 movementInput;
    private CameraFollowSmooth cameraScript; // reference to camera script

    // Start is called once b4 first execution of Update, after MonoBehaviour created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>(); // finds RigidBody2D component attached to same GameObject as script
        rb.gravityScale = 0; // no gravity, since it's top-down game
        rb.linearDamping = drag; // apply drag directly

        cameraScript = FindAnyObjectByType<CameraFollowSmooth>(); // auto-find camera script
    }

    // Update is called once per frame
    void Update()
    {
        // get movement input
        movementInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        movementInput = movementInput.normalized; // prevent faster diagonal movement
    }

    void FixedUpdate() {
        // apply force in direction of movement
        rb.AddForce(movementInput * acceleration);

        // clamp max speed manually (drag alone won't do it perfectly)
        if (rb.linearVelocity.magnitude > maxSpeed)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
        }
    }

    // detect collision with enemies
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy")) // check if collided object is enemy
        {
            Grow(); // make player bigger and slower
            Destroy(other.gameObject); // destroy the enemy
        }
    }

    // function to decrease speed and increase size
    void Grow() {
        // increase size slightly
        transform.localScale += new Vector3(growthFactor, growthFactor, 0);

        // decrease max speed slightly
        maxSpeed = Mathf.Max(1f, maxSpeed - slowDownFactor); // prevents maxSpeed from going below 1

        // use CameraFollowSmooth to smoothly adjust zoom
        if (cameraScript != null)
        {
            cameraScript.AdjustZoom(zoomOutFactor);
        }
    }
}

using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    // Movement settings
    public float acceleration = 10f;
    public float maxSpeed = 5f;
    public float drag = 2f;

    // Growth mechanics
    public float zoomOutFactor = 0.2f;    // How much camera zooms out when growing
    public float growthFactor = 0.1f;     // Size increase per enemy eaten
    public float slowDownFactor = 0.2f;   // Speed decrease per growth

    // Private references
    private Rigidbody2D rb;
    private Vector2 movementInput;
    private CameraFollowSmooth cameraScript;

    // Start is called before the first frame update
    void Start()
    {
        // Get and setup components
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;              // No gravity in top-down game
        rb.linearDamping = drag;          // Apply water-like resistance

        cameraScript = FindAnyObjectByType<CameraFollowSmooth>();
    }

    // Update is called once per frame
    void Update()
    {
        // Get movement input and normalize it to prevent faster diagonal movement
        movementInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        movementInput = movementInput.normalized;
    }

    void FixedUpdate()
    {
        // Apply movement force
        rb.AddForce(movementInput * acceleration);

        // Clamp maximum speed
        if (rb.linearVelocity.magnitude > maxSpeed)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
        }
    }

    // Called when this object's collider enters another trigger collider
    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Trigger entered with: " + other.gameObject.name);  // Debug log

        if (other.CompareTag("Enemy"))
        {
            EnemyScript enemy = other.GetComponent<EnemyScript>();
            Debug.Log("Enemy found, checking for soul state");  // Debug log

            if (enemy != null)
            {
                if (enemy.isSoul)
                {
                    Debug.Log("Soul fish caught the player! Ending game...");  // Debug log
                    if (GameManager.Instance != null)
                    {
                        GameManager.Instance.EndGame();
                    }
                    else
                    {
                        Debug.LogError("GameManager instance not found!");  // Debug error
                    }
                }
                else if (enemy.IsLargerThan(transform))
                {
                    Debug.Log("Larger enemy encountered, turning into soul");  // Debug log
                    enemy.TurnIntoSoul();
                }
                else
                {
                    Debug.Log("Smaller enemy encountered, consuming");  // Debug log
                    Grow();
                    Destroy(other.gameObject);
                }
            }
        }
    }

    // Handle growth mechanics
    void Grow()
    {
        // Increase size
        transform.localScale += new Vector3(growthFactor, growthFactor, 0);

        // Decrease speed (with minimum limit)
        maxSpeed = Mathf.Max(1f, maxSpeed - slowDownFactor);

        // Adjust camera zoom if camera script exists
        if (cameraScript != null)
        {
            cameraScript.AdjustZoom(zoomOutFactor);
        }
    }
}
using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    // Public variables adjustable in Unity Inspector
    public float moveSpeed = 3f;
    public bool isSoul = false;

    // Soul following behavior settings
    public float followDistance = 2f;     // Distance to maintain when following as a soul

    // Private references and variables
    private Transform player;
    private SpriteRenderer spriteRenderer;
    private float angleOffset;            // Offset for circular movement

    // Start is called before the first frame update
    void Start()
    {
        // Find player and get components
        player = GameObject.FindGameObjectWithTag("Player").transform;
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Random start angle for soul movement
        angleOffset = Random.Range(0f, 360f);

        // If starting as a soul, initialize soul properties
        if (isSoul)
        {
            InitializeSoulState();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (player == null) return;

        if (isSoul)
        {
            FollowAsSpirit();
        }
        else
        {
            // Basic movement towards player
            transform.position = Vector2.MoveTowards(transform.position,
                player.position, moveSpeed * Time.deltaTime);
        }
    }

    // Soul movement pattern
    private void FollowAsSpirit()
    {
        // Calculate base direction to player
        Vector2 directionToPlayer = (player.position - transform.position).normalized;

        // Add circular motion
        float angle = Time.time * moveSpeed + angleOffset;
        Vector2 offset = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * followDistance;

        // Calculate target position
        Vector2 targetPosition = (Vector2)player.position - (directionToPlayer * followDistance) + offset;

        // Move towards target position
        transform.position = Vector2.Lerp(transform.position, targetPosition, Time.deltaTime * moveSpeed);
    }

    // Convert enemy to soul state
    public void TurnIntoSoul()
    {
        isSoul = true;
        InitializeSoulState();
        Debug.Log($"Enemy {gameObject.name} turned into soul"); // Debug log
    }

    // Initialize soul state properties
    private void InitializeSoulState()
    {
        // Make semi-transparent
        spriteRenderer.color = new Color(1f, 1f, 1f, 0.5f);

        // Keep collider enabled for soul state
        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null)
        {
            collider.enabled = true;
            Debug.Log($"Soul collider state: {collider.enabled}"); // Debug log
        }
    }

    // Compare size with another transform
    public bool IsLargerThan(Transform other)
    {
        return transform.localScale.x > other.localScale.x;
    }
}
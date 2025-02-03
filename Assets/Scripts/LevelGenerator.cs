using UnityEngine;
using System.Collections.Generic;

public class LevelGenerator : MonoBehaviour
{
    [Header("References")]
    public GameObject enemyPrefab;
    public Transform player;
    private Rigidbody2D playerRb;         // Reference to player's rigidbody

    [Header("Spawn Settings")]
    public float spawnDistance = 10f;     // Distance ahead of player to spawn enemies
    public float spawnWidth = 15f;        // Width of spawn area
    public int enemiesPerSpawn = 3;       // How many enemies to spawn at once
    public float spawnInterval = 2f;      // Time between spawn attempts
    public float minSpeedForSpawn = 0.1f; // Minimum player speed to trigger spawning

    [Header("Enemy Size Settings")]
    public float minEnemyScale = 0.5f;
    public float maxEnemyScale = 2f;
    public float playerSizeMultiplier = 1.5f;

    [Header("Chunk Management")]
    public float deleteDistance = 30f;    // Distance to delete enemies

    private float nextSpawnTime;
    private Vector2 lastPlayerPos;
    private List<GameObject> spawnedEnemies = new List<GameObject>();
    private Vector2 moveDirection;

    void Start()
    {
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player").transform;

        playerRb = player.GetComponent<Rigidbody2D>();
        lastPlayerPos = player.position;
        nextSpawnTime = Time.time;
    }

    void Update()
    {
        UpdateMoveDirection();
        CleanupEnemies();

        // Only spawn if player is moving fast enough
        if (Time.time >= nextSpawnTime && playerRb.linearVelocity.magnitude >= minSpeedForSpawn)
        {
            SpawnEnemyChunk();
            nextSpawnTime = Time.time + spawnInterval;
        }
    }

    void UpdateMoveDirection()
    {
        // Get movement from rigidbody velocity
        if (playerRb.linearVelocity.magnitude >= minSpeedForSpawn)
        {
            moveDirection = playerRb.linearVelocity.normalized;
        }
        // Fallback to position-based direction if velocity is too low
        else
        {
            Vector2 currentPos = player.position;
            Vector2 movement = currentPos - lastPlayerPos;
            if (movement.magnitude > 0.01f)
            {
                moveDirection = movement.normalized;
            }
            lastPlayerPos = currentPos;
        }
    }

    void SpawnEnemyChunk()
    {
        // Calculate spawn center position in front of player
        Vector2 spawnCenter = (Vector2)player.position + moveDirection * spawnDistance;

        // Calculate perpendicular vector for width distribution
        Vector2 perpendicular = new Vector2(-moveDirection.y, moveDirection.x);

        for (int i = 0; i < enemiesPerSpawn; i++)
        {
            // Distribute enemies along the width, perpendicular to movement direction
            float offset = Random.Range(-spawnWidth / 2f, spawnWidth / 2f);
            Vector2 spawnPos = spawnCenter + perpendicular * offset;

            // Create enemy
            GameObject enemy = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);

            // Calculate size based on player size
            float maxSize = Mathf.Min(maxEnemyScale, player.localScale.x * playerSizeMultiplier);
            float randomScale = Random.Range(minEnemyScale, maxSize);
            enemy.transform.localScale = new Vector3(randomScale, randomScale, 1);

            spawnedEnemies.Add(enemy);
        }
    }

    void CleanupEnemies()
    {
        for (int i = spawnedEnemies.Count - 1; i >= 0; i--)
        {
            if (spawnedEnemies[i] == null)
            {
                spawnedEnemies.RemoveAt(i);
                continue;
            }

            // Delete enemies that are too far from the player
            float distance = Vector2.Distance(player.position, spawnedEnemies[i].transform.position);
            if (distance > deleteDistance)
            {
                Destroy(spawnedEnemies[i]);
                spawnedEnemies.RemoveAt(i);
            }
        }
    }

    void OnDrawGizmos()
    {
        if (player != null)
        {
            Gizmos.color = Color.yellow;
            // Draw spawn area in the movement direction
            if (moveDirection != Vector2.zero)
            {
                Vector2 spawnCenter = (Vector2)player.position + moveDirection * spawnDistance;
                Vector2 perpendicular = new Vector2(-moveDirection.y, moveDirection.x);

                Vector2 p1 = spawnCenter + perpendicular * (spawnWidth / 2f);
                Vector2 p2 = spawnCenter - perpendicular * (spawnWidth / 2f);

                Gizmos.DrawLine(p1, p2);
                // Draw direction indicator
                Gizmos.color = Color.blue;
                Gizmos.DrawLine(player.position, spawnCenter);
            }
        }
    }
}
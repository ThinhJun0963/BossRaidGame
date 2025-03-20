using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI_SlimeBoss : MonoBehaviour
{
    [Header("Slime Boss Settings")]
    [SerializeField] private int health = 10;
    [SerializeField] private GameObject slimePrefab;
    [SerializeField] private int spawnOnDeath = 20;
    [SerializeField] private float spawnRadius = 2.0f;

    public void TakeDamage(int damage)
    {
        health -= damage;

        if (health <= 0) Die();
    }

    private void Die()
    {
        for (int i = 0; i < spawnOnDeath; i++)
        {
            SpawnSlime();
        }

        Destroy(gameObject); // Remove boss after spawning slimes
    }

    private void SpawnSlime()
    {
        if (slimePrefab == null) return;

        Vector2 spawnPosition = (Vector2)transform.position + Random.insideUnitCircle * spawnRadius;
        Instantiate(slimePrefab, spawnPosition, Quaternion.identity);
    }
}

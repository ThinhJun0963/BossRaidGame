using UnityEngine;

public class BossFacing : MonoBehaviour
{
    private Transform playerTransform;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (playerTransform != null)
        {
            // Xác định hướng của Player
            float direction = transform.position.x - playerTransform.position.x;

            // Flip sprite dựa vào hướng
            spriteRenderer.flipX = direction < 0;
        }
    }
}

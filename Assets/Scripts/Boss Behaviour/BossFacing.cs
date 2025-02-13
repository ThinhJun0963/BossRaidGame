using UnityEngine;

public class BossFacing : MonoBehaviour
{
    private Transform playerTransform;
    private Vector3 previousPosition;

    void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        previousPosition = transform.position;
    }

    void Update()
    {
        if (playerTransform == null) return;

        // Kiểm tra hướng di chuyển
        Vector3 movement = transform.position - previousPosition;

        if (Mathf.Abs(movement.x) > 0.01f) // Nếu đang di chuyển
        {
            // Đi sang phải thì scale.x = 1, đi sang trái thì scale.x = -1
            transform.localScale = new Vector3(
                movement.x > 0 ? 1 : -1,
                transform.localScale.y,
                transform.localScale.z
            );
        }
        else // Nếu đứng yên thì xoay theo hướng player
        {
            transform.localScale = new Vector3(
                transform.position.x < playerTransform.position.x ? 1 : -1,
                transform.localScale.y,
                transform.localScale.z
            );
        }

        previousPosition = transform.position;
    }
}
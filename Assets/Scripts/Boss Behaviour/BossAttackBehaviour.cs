using UnityEngine;

public class BossAttackBehaviour : StateMachineBehaviour
{
    [SerializeField] private float attackRange = 2f; // Tầm đánh
    [SerializeField] private float attackCooldown = 2f; // Thời gian hồi đòn đánh

    private Transform playerTransform;
    private float nextAttackTime = 0f;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        if (playerTransform == null)
        {
            Debug.LogError("Player not found!");
            return;
        }
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (playerTransform == null) return;

        float distanceToPlayer = Vector2.Distance(animator.transform.position, playerTransform.position);

        // Kiểm tra khoảng cách và cooldown
        if (distanceToPlayer <= attackRange && Time.time >= nextAttackTime)
        {
            // Trigger attack animation
            animator.SetTrigger("attack1");
            nextAttackTime = Time.time + attackCooldown;
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Reset attack trigger
        animator.ResetTrigger("attack1");
    }
}
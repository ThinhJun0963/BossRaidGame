using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowingBehaviour : StateMachineBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private float awareDistance = 10f;
    [SerializeField] private float attackDistance = 2f;

    private Transform playerPos;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        playerPos = GameObject.FindGameObjectWithTag("Player").transform;
        if (playerPos == null)
        {
            Debug.LogError("Player not found!");
            return;
        }
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (playerPos == null) return;

        float distanceToPlayer = Vector2.Distance(animator.transform.position, playerPos.position);

        if (distanceToPlayer > awareDistance || distanceToPlayer < attackDistance)
        {
            animator.SetBool("isFollowing", false);
        }
        else
        {
            animator.transform.position = Vector2.MoveTowards(
                animator.transform.position,
                playerPos.position,
                speed * Time.deltaTime
            );

            // Điều chỉnh hướng mặt
            float direction = animator.transform.position.x - playerPos.position.x;
            animator.transform.localScale = new Vector3(
                direction > 0 ? -1 : 1,
                animator.transform.localScale.y,
                animator.transform.localScale.z
            );
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleBehaviour : StateMachineBehaviour
{
    [SerializeField] private float awareDistance = 10f;
    [SerializeField] private float attackDistance = 2f;
    [SerializeField] private float idleWaitTime = 4f;

    private Transform playerPos;
    private float idleTimer;
    private BossState currentState;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        playerPos = GameObject.FindGameObjectWithTag("Player").transform;
        if (playerPos == null)
        {
            Debug.LogError("Player not found!");
            return;
        }
        idleTimer = 0f;
        currentState = BossState.Idle;
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (playerPos == null) return;

        float distanceToPlayer = Vector2.Distance(animator.transform.position, playerPos.position);

        // Xử lý chuyển state
        if (distanceToPlayer < attackDistance)
        {
            currentState = BossState.Idle;
            ResetAllBools(animator);
            animator.SetBool("isIdle", true);
        }
        else if (distanceToPlayer <= awareDistance)
        {
            currentState = BossState.Following;
            ResetAllBools(animator);
            animator.SetBool("isFollowing", true);
        }
        else
        {
            idleTimer += Time.deltaTime;
            if (idleTimer >= idleWaitTime)
            {
                currentState = BossState.Patrol;
                ResetAllBools(animator);
                animator.SetBool("isPatroling", true);
            }
        }
    }

    private void ResetAllBools(Animator animator)
    {
        animator.SetBool("isIdle", false);
        animator.SetBool("isFollowing", false);
        animator.SetBool("isPatroling", false);
    }
}
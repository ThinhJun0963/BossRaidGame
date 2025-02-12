using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleBehaviour : StateMachineBehaviour
{
    private Transform playerPos;
    public float awareDistance;
    public float attackDistance;

    public float idleWaitTime = 4f; // Thời gian chờ trước khi chuyển sang patrol
    private float idleTimer; // Bộ đếm thời gian

    // OnStateEnter: Reset lại giá trị khi bắt đầu trạng thái Idle
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        playerPos = GameObject.FindGameObjectWithTag("Player").transform;
        idleTimer = 0f; // Đặt lại bộ đếm thời gian
    }

    // OnStateUpdate: Xử lý logic trạng thái Idle
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        float distanceToPlayer = Vector2.Distance(animator.transform.position, playerPos.position);

        if (distanceToPlayer < attackDistance)
        {
            // Player trong tầm tấn công, giữ trạng thái Idle
            animator.SetBool("isIdle", true);
            idleTimer = 0f; // Đặt lại timer
        }
        else if (distanceToPlayer >= attackDistance && distanceToPlayer <= awareDistance)
        {
            // Player trong tầm nhận biết, chuyển sang trạng thái theo dõi
            animator.SetBool("isFollowing", true);
            idleTimer = 0f; // Đặt lại timer
        }
        else if (distanceToPlayer > awareDistance)
        {
            // Player ngoài tầm nhận biết, bắt đầu tăng thời gian chờ
            idleTimer += Time.deltaTime;

            if (idleTimer >= idleWaitTime)
            {
                animator.SetBool("isIdle", false); // Chuyển sang trạng thái Patrol
                idleTimer = 0f; // Đặt lại timer
            }
        }
    }

    // OnStateExit: Thực hiện khi thoát khỏi trạng thái Idle
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Reset các trạng thái nếu cần
        animator.SetBool("isIdle", false); // Đảm bảo trạng thái Idle được tắt
    }
}

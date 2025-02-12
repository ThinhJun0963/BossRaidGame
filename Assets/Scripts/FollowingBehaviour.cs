using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowingBehaviour : StateMachineBehaviour
{
    public float speed;
    private Transform playerPos;
    public float awareDistance;
    public float attackDistance;
  

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
       // lấy ra vị trí của Player
        playerPos = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Nếu khoảng cách tới vừa đủ thì dừng đuổi theo -> điều kiện để chuyển về Idle
        if (Vector2.Distance(animator.transform.position, playerPos.position) > awareDistance ||
             Vector2.Distance(animator.transform.position, playerPos.position) < attackDistance)
        {
            animator.SetBool("isFollowing", false);
        } else
        {
            // Boss đi thẳng tới vị trí của Player
            animator.transform.position = Vector2.MoveTowards(animator.transform.position, playerPos.position, speed * Time.deltaTime);

            // Điều chỉnh hướng mặt của boss
            if (animator.transform.position.x < playerPos.position.x)
            {
                // Di chuyển sang phải
                animator.transform.localScale = new Vector3(1, animator.transform.localScale.y, animator.transform.localScale.z);
            }
            else if (animator.transform.position.x > playerPos.position.x)
            {
                // Di chuyển sang trái
                animator.transform.localScale = new Vector3(-1, animator.transform.localScale.y, animator.transform.localScale.z);
            }
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Implement code that processes and affects root motion
    }
}

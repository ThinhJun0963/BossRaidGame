using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolBehaviour : StateMachineBehaviour
{
    [SerializeField] private float speed = 3f;
    [SerializeField] private float minX = -10f;
    [SerializeField] private float maxX = 10f;
    [SerializeField] private float minY = -10f;
    [SerializeField] private float maxY = 10f;
    [SerializeField] private float awareDistance = 10f;
    [SerializeField] private float startWaitTime = 3f;

    private Transform playerPos;
    private Transform moveSpots;
    private float waitTime;
    private Vector2 targetPosition;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        playerPos = GameObject.FindGameObjectWithTag("Player").transform;
        if (playerPos == null)
        {
            Debug.LogError("Player not found!");
            return;
        }

        // Tạo điểm đến ngẫu nhiên
        targetPosition = new Vector2(
            Random.Range(minX, maxX),
            Random.Range(minY, maxY)
        );

        waitTime = startWaitTime;
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (playerPos == null) return;

        // Kiểm tra người chơi trước
        if (Vector2.Distance(animator.transform.position, playerPos.position) <= awareDistance)
        {
            animator.SetBool("isPatroling", false);
            animator.SetBool("isFollowing", true);
            return;
        }

        // Xử lý di chuyển tuần tra
        if (Vector2.Distance(animator.transform.position, targetPosition) > 0.1f)
        {
            animator.transform.position = Vector2.MoveTowards(
                animator.transform.position,
                targetPosition,
                speed * Time.deltaTime
            );
        }
        else
        {
            if (waitTime <= 0)
            {
                animator.SetBool("isIdle", true);
                animator.SetBool("isPatroling", false);
            }
            else
            {
                waitTime -= Time.deltaTime;
            }
        }
    }
}
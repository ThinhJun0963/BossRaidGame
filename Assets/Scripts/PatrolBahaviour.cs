using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolBehaviour : StateMachineBehaviour
{
    public float speed;

    public Transform moveSpots;
    public float minX;
    public float maxX;
    public float minY;
    public float maxY;

    private Transform playerPos;
    public float awareDistance;

    private float waitTime; // Bộ đếm thời gian chờ
    public float startWaitTime = 3f; // Thời gian chờ mặc định

    // OnStateEnter: Xác định các giá trị khởi tạo
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Đặt vị trí random để boss đi đến
        moveSpots.position = new Vector2(Random.Range(minX, maxX), Random.Range(minY, maxY));
        playerPos = GameObject.FindGameObjectWithTag("Player").transform;

        // Khởi tạo thời gian chờ
        waitTime = startWaitTime;
    }

    // OnStateUpdate: Xử lý hành vi di chuyển
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Boss di chuyển tới vị trí random
        if (Vector2.Distance(animator.transform.position, moveSpots.position) > 0.1f)
        {
            animator.transform.position = Vector2.MoveTowards(animator.transform.position, moveSpots.position, speed * Time.deltaTime);
        }
        else
        {
            // Giảm thời gian chờ khi boss đã đến vị trí mục tiêu
            if (waitTime <= 0)
            {
                animator.SetBool("isIdle", true); // Kích hoạt trạng thái Idle sau khi hết thời gian chờ
            }
            else
            {
                waitTime -= Time.deltaTime; // Giảm thời gian chờ mỗi khung hình
            }
        }

        // Nếu player trong phạm vi awareDistance, chuyển sang trạng thái theo dõi
        if (Vector2.Distance(animator.transform.position, playerPos.position) <= awareDistance)
        {
            animator.SetBool("isPatroling", false);
        }
    }

    // OnStateExit: Đặt lại các giá trị khi rời trạng thái Patrol
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
    }
}

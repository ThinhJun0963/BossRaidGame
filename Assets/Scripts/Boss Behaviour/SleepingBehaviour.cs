using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SleepingBehaviour : StateMachineBehaviour
{
    [SerializeField] private float awareDistance = 10f;
    private Transform playerPos;
    private bool hasTriggeredIntro = false;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        playerPos = GameObject.FindGameObjectWithTag("Player").transform;
        if (playerPos == null)
        {
            Debug.LogError("Player not found!");
            return;
        }
        hasTriggeredIntro = false;
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (playerPos == null) return;

        if (!hasTriggeredIntro &&
            Vector2.Distance(animator.transform.position, playerPos.position) <= awareDistance)
        {
            animator.SetBool("isIntro", true);
            hasTriggeredIntro = true;
        }
    }
}
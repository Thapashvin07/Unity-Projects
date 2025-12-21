using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CancelDamage : StateMachineBehaviour
{
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    //override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
       animator.gameObject.GetComponent<PlayerMovement>().isDamageable = false;
       float upordownBorder = (animator.transform.position.y>0f)?1:-1;
      //  Vector3 targetPosition = new Vector3(0,1);
       animator.gameObject.GetComponent<PlayerMovement>().direction = Vector2.up*upordownBorder;
      Vector3 worldPos = Camera.main.ViewportToWorldPoint(new Vector3(0.05f, 0.5f));
      worldPos.y = 4*upordownBorder+(-1*upordownBorder);
      // animator.transform.position = worldPos;
      animator.gameObject.GetComponent<PlayerMovement>().targetPos = worldPos;
    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}

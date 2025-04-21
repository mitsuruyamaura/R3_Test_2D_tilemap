using System.Collections;
using UnityEngine;

public class BattleAnimatorCoroutineBase : MonoBehaviour
{
    [SerializeField] private Animator animator;

    public IEnumerator PlayAttackAnimationCoroutine() {
        animator.SetTrigger("Attack");
        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"));
    }

    public IEnumerator PlayDamageAnimationCoroutine() {
        animator.SetTrigger("Hit");
        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"));
    }
}
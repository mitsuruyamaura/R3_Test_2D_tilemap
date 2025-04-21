using Cysharp.Threading.Tasks;
using System.Collections;
using UnityEngine;

public class BattleAnimator : MonoBehaviour {
    [SerializeField] private Animator animator;

    public async UniTask PlayAttackAnimationAsync() {
        animator.SetTrigger("Attack");
        await UniTask.WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"));
    }

    public async UniTask PlayDamageAnimationAsync() {
        animator.SetTrigger("Hit");
        await UniTask.WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"));
    }


    public IEnumerator PlayAttackAnimationCoroutine() {
        animator.SetTrigger("Attack");
        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"));
    }

    public IEnumerator PlayDamageAnimationCoroutine() {
        animator.SetTrigger("Hit");
        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"));
    }
}
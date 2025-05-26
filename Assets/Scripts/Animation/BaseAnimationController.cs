using UnityEngine;
using System.Collections;
using Unity.VisualScripting;

public class BaseAnimationController : MonoBehaviour
{
    [SerializeField] protected Animator animator;

    [Header("Idle Settings")]
    [SerializeField] protected AnimationClip[] idleVariants;
    protected int currentIdleIndex = -1;
    [SerializeField] protected float timeToRollForNewIdleAnim;
    [SerializeField] protected int chanceToSwapIdleAnim;

    [Header("Death Settings")]
    [SerializeField] protected AnimationClip[] deathVariants;

    [Header("Attack Settings")]
    [SerializeField] protected AnimationClip[] meleeAttackVariants;

    protected bool isAiming = false;
    protected bool isCasting = false;

    public virtual void Awake()
    {
        if (!animator) { animator = GetComponent<Animator>(); }
    }

    public virtual void UpdateMovement(float speed)
    {
        animator.SetFloat("Speed", speed);
    }

    // public virtual IEnumerator CycleIdleAnimations()
    // {
    //     while (true)
    //     {
    //         // If the player is standing still, (chanceToSwapIdleAnim) every (timeToRollForNewIdleAnim) seconds.
    //         yield return new WaitForSeconds(timeToRollForNewIdleAnim);
    //         if (!isAiming && animator.GetFloat("Speed") == 0)
    //         {
    //             int rand = Random.Range(0, 10);
    //             if (rand < chanceToSwapIdleAnim)
    //             {
    //                 TriggerRandomIdle();
    //             }
    //         }
    //     }
    // }

    // public virtual void TriggerRandomIdle()
    // {
    //     if (idleVariants.Length == 0) { return; }

    //     int index = currentIdleIndex;
    //     do { index = Random.Range(0, idleVariants.Length); }
    //     while (index == currentIdleIndex && idleVariants.Length > 1);

    //     currentIdleIndex = index;
    //     //animator.SetFloat("IdleIndex", index);
    //     PlayIdleByIndex(index);
    // }

    // private void PlayIdleByIndex(int index)
    // {
    //     string stateName = $"Idle {index + 1}";
    //     animator.CrossFade(stateName, 0.25f);
    // }

    public virtual void TriggerRandomDeath()
    {
        if (deathVariants.Length == 0) { return; }
        int index = Random.Range(0, deathVariants.Length);
        animator.SetInteger("DeathIndex", index);
        animator.SetTrigger("Die");
    }

    public virtual void TriggerRandomAttack()
    {
        if (meleeAttackVariants.Length == 0) { return; }
        if (AnimatorHasParameter("Attack"))
        {
            int index = Random.Range(0, meleeAttackVariants.Length);
            animator.SetInteger("AttackIndex", index);
            animator.SetTrigger("Attack");
        }
    }

    public virtual void TriggerSpellcast()
    {
        if (AnimatorHasParameter("CastSpell"))
        {
            animator.SetTrigger("CastSpell");
        }
    }

    public virtual void SetAiming(bool isAiming)
    {
        if (AnimatorHasParameter("IsAiming"))
        {
            animator.SetBool("IsAiming", isAiming);
        }
    }

    public virtual void SetCasting(bool isCasting)
    {
        if (AnimatorHasParameter("IsCasting"))
        {
            animator.SetBool("IsCasting", isCasting);
        }
    }

    // Helper method for checking if a parameter exists
    protected bool AnimatorHasParameter(string paramName)
    {
        foreach (AnimatorControllerParameter param in animator.parameters)
        {
            if (param.name == paramName)
            {
                return true;
            }
        }
        return false;
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Character
{
    public static Player current;
    private Animator anim;
    private float lastAttackTime;

    protected override void Awake()
    {
        base.Awake();
        current = this;
        anim = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        if (target != null && !target.isDead)
        {
            float targetDistance = Vector3.Distance(transform.position, target.transform.position);
            if (targetDistance <= attackRange)
            {
                controller.StopMovement();
                controller.LookTowards(target.transform.position - transform.position);

                // Check attack speed
                if (Time.time - lastAttackTime > attackRate)
                {
                    lastAttackTime = Time.time;
                    anim.SetTrigger("isAttack");
                }
            }
            else
            {
                controller.MoveToTarget(target.transform);
            }
        }
        else
        {
            SetTarget(null);
        }
        anim.SetBool("isMoving", controller.isMoving);
    }

    public override IEnumerator Die()
    {
        anim.SetBool("isDead", true);
        yield return base.Die();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CController = UnityEngine.CharacterController;

public class Player : Character
{
    public static Player current;
    public GameObject nav;
    public float targetRange;
    private bool allowAttack = true;
    private Animator anim;
    private float lastAttackTime;
    private CController m_controller;

    protected override void Awake()
    {
        base.Awake();
        current = this;
        anim = GetComponentInChildren<Animator>();
        m_controller = GetComponent<CController>();
    }

    private void Update()
    {
        if (Time.time >= lastAttackTime + attackRate)
        {
            allowAttack = true;
        }
        if (target == null)
        {
            nav.SetActive(false);
            target = EnemyManager.Instance.FindNearestEnemy(transform.position, targetRange);
        }
        else
        {
            nav.SetActive(true);
            nav.transform.position = target.transform.position;
            if (target.isDead || Vector3.Distance(transform.position, target.transform.position) > targetRange)
            {
                target = null;
            }
        }
        anim.SetBool("isMoving", m_controller.velocity.magnitude > 0.2f);
    }

    public void Attack()
    {
        if (!allowAttack) return;
        
        if (target != null)
        {
            float distance = Vector3.Distance(transform.position, target.transform.position);
            if (distance <= targetRange)
            {
                transform.LookAt(target.transform.position);
                anim.SetTrigger("isAttack");
                allowAttack = false;
            }
        }
    }

    public override IEnumerator Die()
    {
        anim.SetBool("isDead", true);
        yield return base.Die();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Character
{
    public enum State
    {
        Idle, Chase, Attack
    }

    public State currentState = State.Idle;
    private Animator anim;
    private float lastAttackTime;
    private float targetDistance;

    [SerializeField] private float chaseRange;

    private void Start()
    {
        target = Player.current;
        anim = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        if (target == null || isDead) return;

        if (target.isDead)
        {
            anim.SetBool("isMoving", false);
            SetState(State.Idle);
            return;
        }

        targetDistance = Vector3.Distance(transform.position, target.transform.position);
        switch (currentState)
        {
            case State.Idle:
                IdleUpdate();
                break;
            case State.Chase:
                ChaseUpdate();
                break;
            case State.Attack:
                AttackUpdate();
                break;
        }
        anim.SetBool("isMoving", currentState == State.Chase);
    }

    private void SetState(State state)
    {
        currentState = state;

        switch (currentState)
        {
            case State.Idle:
                controller.StopMovement();
                break;
            case State.Attack:
                controller.StopMovement();
                break;
            case State.Chase:
                controller.MoveToTarget(target.transform);
                break;
        }
    }

    private void IdleUpdate()
    {
        if (targetDistance < chaseRange && targetDistance > attackRange)
        {
            SetState(State.Chase);
        }
        else if (targetDistance <= attackRange)
        {
            SetState(State.Attack);
        }
    }

    private void ChaseUpdate()
    {
        if (targetDistance > chaseRange || isDead)
        {
            SetState(State.Idle);
        }
        else if (targetDistance <= attackRange)
        {
            SetState(State.Attack);
        }
    }

    private void AttackUpdate()
    {
        if (targetDistance > attackRange)
        {
            SetState(State.Chase);
        }
        else if (targetDistance > chaseRange || isDead)
        {
            SetState(State.Idle);
        }

        controller.LookTowards(target.transform.position - transform.position);

        if (Time.time - lastAttackTime > attackRate)
        {
            lastAttackTime = Time.time;
            anim.SetTrigger("isAttack");
        }
    }

    public override void Die()
    {
        base.Die();
        anim.SetBool("isDead", true);
    }
}

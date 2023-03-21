using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Character : MonoBehaviour
{
    [Header("Stats")]
    public int currentHp;
    public int maxHp;
    public int damage;
    [SerializeField] protected float attackRange, attackRate;

    [Header("Components")]
    public GameObject healthBarPrefab;
    public Character target;
    public bool isDead;
    [HideInInspector] public CharacterController controller;

    public event UnityAction onTakeDamage;
    public event UnityAction onDie;

    protected virtual void Awake()
    {
        controller = GetComponent<CharacterController>();
        healthBarPrefab = Instantiate(healthBarPrefab, transform.position + healthBarPrefab.transform.position, Quaternion.identity, transform);
    }

    protected virtual void OnEnable()
    {
        currentHp = maxHp;
        healthBarPrefab.SetActive(true);
        isDead = false;
    }

    public virtual void SetTarget(Character _target) 
    {
        target = _target;
    }
    public virtual void TakeDamage(int value) 
    { 
        currentHp -= value;
        onTakeDamage?.Invoke();

        if (currentHp <= 0)
        {
            StartCoroutine(Die());
        }
    }
    public virtual void AttackTarget() 
    { 
        if (target != null)
        {
            target.TakeDamage(damage);
        }
    }
    public virtual IEnumerator Die()
    {
        isDead = true;
        onDie?.Invoke();
        controller.StopMovement();

        yield return new WaitForSeconds(3f);
        ObjectPool.Instance.DestroyGameObject(this.gameObject, true);
    }
}

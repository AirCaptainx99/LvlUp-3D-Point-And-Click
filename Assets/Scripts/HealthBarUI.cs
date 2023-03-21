using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    [SerializeField] private Image healthFill;
    [SerializeField] private Color playerHealthColor, enemyHealthColor;
    private Character character;

    private void OnEnable()
    {
        UpdateHealthBar();
        character.onTakeDamage += UpdateHealthBar;
        character.onDie += HideHealthBar;
    }

    private void OnDisable()
    {
        character.onTakeDamage -= UpdateHealthBar;
        character.onDie -= HideHealthBar;
    }

    private void Awake()
    {
        character = GetComponentInParent<Character>();
        healthFill.color = (transform.root.tag == "Player") ? playerHealthColor : enemyHealthColor;
    }

    private void Start()
    {
        UpdateHealthBar();
    }

    private void Update()
    {
        transform.rotation = Quaternion.LookRotation(transform.position - Camera.main.transform.position);
    }

    private void UpdateHealthBar()
    {
        healthFill.fillAmount = (float)character.currentHp / character.maxHp;
    }

    private void HideHealthBar()
    {
        gameObject.SetActive(false);
    }
}

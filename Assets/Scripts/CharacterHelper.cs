using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterHelper : MonoBehaviour
{
    Character character;

    private void Awake()
    {
        character = GetComponentInParent<Character>();
    }

    public void OnAttack()
    {
        character.AttackTarget();
    }
}

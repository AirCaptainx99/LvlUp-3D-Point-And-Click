using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using CController = UnityEngine.CharacterController;

public class JoystickController : MonoBehaviour
{
    public float speed = 5f;
    PlayerInput playerInput;
    Transform cameraTransform;
    CController controller;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        controller = GetComponent<CController>();
        cameraTransform = Camera.main.transform;
    }

    private void Update()
    {
        Vector2 input = playerInput.actions["Move"].ReadValue<Vector2>();
        Vector3 move = new Vector3(input.x, 0f, input.y);
        
        move = move.x * cameraTransform.right + move.z * cameraTransform.forward;
        move.y = 0f;

        controller.Move(move * Time.deltaTime * speed);

        if (input != Vector2.zero)
        {
            transform.forward = move;
        }
    }
}

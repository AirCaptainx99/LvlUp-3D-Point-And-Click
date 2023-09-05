using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    public Transform cameraPoint;
    private PlayerInput playerInput;

    private void Start()
    {
        playerInput = Player.current.GetComponent<PlayerInput>();
    }

    private void Update()
    {
        Vector2 input = playerInput.actions["Look"].ReadValue<Vector2>();
        cameraPoint.position = Player.current.transform.position;

        if (input != Vector2.zero)
        {
            cameraPoint.rotation *= Quaternion.Euler(0f, input.x, 0f);
        }
    }
}

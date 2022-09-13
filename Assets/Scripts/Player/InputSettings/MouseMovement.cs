using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MouseMovement : MonoBehaviour
{

    public Camera playerCamera;
    public Gun gun;
    private bool ray_hit_something = false;
    private void FixedUpdate()
    {
        RaycastHit hit;
        Ray ray = playerCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

        ray_hit_something = Physics.Raycast(ray, out hit);

        if (ray_hit_something)
        {
            transform.LookAt(hit.point);
        }
    }
}

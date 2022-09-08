using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerMovement : MonoBehaviour
{
    //Input Fields
    private PlayerControl playerActionsAsset;
    private InputAction move;

    //movement fields
    private Rigidbody rb;
    [SerializeField]
    private float movementForce = 1f;
    [SerializeField]
    private float maxspeed = 5f;
    private Vector3 forceDirection = Vector3.zero;

    [SerializeField]
    private Camera playerCamera;

    public Gun gun;


    private void Awake()
    {
        rb = this.GetComponent<Rigidbody>();
        playerActionsAsset = new PlayerControl();
        playerActionsAsset.Player.Crouch.performed += context => crouch();
        playerActionsAsset.Player.UnCrouch.performed += context => UnCrouch();
        playerActionsAsset.Player.Attack.performed += context => Shoot();

    }


    private void OnEnable()
    {
        move = playerActionsAsset.Player.Move;
        playerActionsAsset.Player.Enable();
    }
    private void OnDisable()
    {
        playerActionsAsset.Player.Disable();
    }
   
    private void FixedUpdate()
    {
        forceDirection += move.ReadValue<Vector2>().x * GetCameraRight(playerCamera) * movementForce;
        forceDirection += move.ReadValue<Vector2>().y * GetCameraForward(playerCamera) * movementForce;

        rb.AddForce(forceDirection, ForceMode.Impulse);
        forceDirection = Vector3.zero;

        if (rb.velocity.y < 0f)
            rb.velocity += Vector3.down * Physics.gravity.y * Time.fixedDeltaTime;

        Vector3 horizontalVelocity = rb.velocity;
        horizontalVelocity.y = 0;
        if (horizontalVelocity.sqrMagnitude > maxspeed * maxspeed)
            rb.velocity = horizontalVelocity.normalized * maxspeed + Vector3.up * rb.velocity.y;


        LookAt();

    }

    private void Shoot()
    {
        gun.Shoot();
    }

    private void crouch()
    {
        Vector3 scale = GetComponent<Transform>().localScale;
        scale.y = 0.5f;
        GetComponent<Transform>().localScale = scale;
        Debug.Log("crouch");
        maxspeed = 1;
        
    }

    private void UnCrouch()
    {
        Vector3 scale = GetComponent<Transform>().localScale;
        scale.y = 1f;
        GetComponent<Transform>().localScale = scale;
        Debug.Log("uncrouch");
        maxspeed = 5;
    }


    private void LookAt()
    {
        Vector3 direction = rb.velocity;
        direction.y = 0f;

        if (move.ReadValue<Vector2>().sqrMagnitude > 0.1f && direction.sqrMagnitude > 0.1f)
            this.rb.rotation = Quaternion.LookRotation(direction, Vector3.up);
        else
            rb.angularVelocity = Vector3.zero;
    }

    private Vector3 GetCameraForward(Camera playerCamera)
    {
        Vector3 forward = playerCamera.transform.forward;
        forward.y = 0;
        return forward.normalized;
    }

    private Vector3 GetCameraRight(Camera playerCamera)
    {
        Vector3 right = playerCamera.transform.right;
        right.y = 0;
        return right.normalized;
    }

    private bool IsGrounded()
    {
        Ray ray = new Ray(this.transform.position + Vector3.up * 0.25f, Vector3.down);
        if (Physics.Raycast(ray, out RaycastHit hit, 0.3f))
            return true;
        else
            return false;
    }

}

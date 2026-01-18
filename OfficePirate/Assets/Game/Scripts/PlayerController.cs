using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    //ref
    private Rigidbody _rb;
    private Animator _animator;
    
    //movement
    private Vector3 _moveInput;
    private Vector3 _deltaPos;
    private Vector3 _deltaRot;
    
    
  
    void Start()
    {
        _rb =  GetComponent<Rigidbody>();
        _animator = GetComponent<Animator>();
    }


    void Update()
    {

        
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    public void GetMoveInput(InputAction.CallbackContext context)
    {
        _moveInput = context.ReadValue<Vector2>();
    }

    
    private void MovePlayer()
    {
        Vector3 targetPosition = _rb.position +  _deltaPos;
        targetPosition.y = _rb.position.y;
        
        _rb.MovePosition(targetPosition);

        _deltaPos = Vector3.zero;
    }


    void OnAnimatorMove()
    {
        _animator.rootPosition = _rb.position;
        _animator.rootRotation = _rb.rotation;
        
        _deltaPos +=  _animator.deltaPosition;
        
    }
}

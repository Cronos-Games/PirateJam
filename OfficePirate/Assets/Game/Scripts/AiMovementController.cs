using System;
using UnityEngine;
using UnityEngine.AI;

public class AiMovementController : MonoBehaviour
{
    private Rigidbody _rb;
    private Animator _animator;
    private NavMeshAgent _agent;
    
    // Input (world-based): x = world X, y = world Z
    private Vector2 _moveInput;

    // Root motion accumulation between physics ticks
    private Vector3 _deltaPos;
    private Quaternion _deltaRot = Quaternion.identity;

    [Header("Settings")]
    [SerializeField] private float inputDeadzone = 0.15f;     // stick deadzone
    [SerializeField] private float rotationDeadzone = 2f;      // degrees


    [Header("References")]
    [SerializeField] private GameObject target;

    private bool _hasTargetYaw;
    private float _targetYaw; // degrees in world Y

    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _animator = GetComponent<Animator>();
        _agent = GetComponent<NavMeshAgent>();

        _deltaRot = Quaternion.identity;
        _deltaPos = Vector3.zero;
        
        _agent.updatePosition = false;
        _agent.updateRotation = false;
        
        _agent.destination = target.transform.position;
    }

    private void Update()
    {

    }

    void FixedUpdate()
    {
        
        Navigate();
        ApplyRootMotionToRigidbody();
        UpdateAnimatorBools();
    }
    
    
    void Navigate()
    {
        Vector2 direction = new Vector2(_agent.velocity.x, _agent.velocity.z); 
        _moveInput = direction;
        
        Debug.Log(_moveInput);
    }
    

    void OnAnimatorMove()
    {
        // Accumulate root motion deltas until next FixedUpdate
        _deltaPos += _animator.deltaPosition;

        // IMPORTANT: order matches how we apply it: rbRot * deltaRot
        _deltaRot = _deltaRot * _animator.deltaRotation;

        UpdateTargetYawAndTurnBools(); // purely decides turn direction + target yaw
        _agent.nextPosition = transform.position;
    }

    private void ApplyRootMotionToRigidbody()
    {
        // Position: keep gravity on Y (don’t inject root Y)
        Vector3 targetPos = _rb.position + _deltaPos;
        targetPos.y = _rb.position.y;
        _rb.MovePosition(targetPos);

        // Rotation: apply root rotation, but clamp yaw so we don't overshoot target
        Quaternion desiredRotFromAnim = _rb.rotation * _deltaRot;

        if (_hasTargetYaw)
        {
            float currentYaw = _rb.rotation.eulerAngles.y;
            float animYaw = desiredRotFromAnim.eulerAngles.y;

            // Step the animation wants to do this tick (signed, -180..180)
            float stepYaw = Mathf.DeltaAngle(currentYaw, animYaw);

            // How far we still need to go to target (signed)
            float remainingYaw = Mathf.DeltaAngle(currentYaw, _targetYaw);

            // If we're basically aligned, snap to exact yaw and stop turning
            if (Mathf.Abs(remainingYaw) <= rotationDeadzone)
            {
                desiredRotFromAnim = Quaternion.Euler(0f, _targetYaw, 0f);
            }
            else
            {
                // Clamp step so it can't exceed remaining (prevents overshoot/hunting)
                float maxStep = Mathf.Abs(remainingYaw);
                float clampedStep = Mathf.Clamp(stepYaw, -maxStep, +maxStep);

                // Make sure clamped step goes in the same direction as remaining
                clampedStep = Mathf.Sign(remainingYaw) * Mathf.Abs(clampedStep);

                desiredRotFromAnim = Quaternion.Euler(0f, currentYaw + clampedStep, 0f);
            }
        }

        _rb.MoveRotation(desiredRotFromAnim);

        // Reset accumulators
        _deltaPos = Vector3.zero;
        _deltaRot = Quaternion.identity;
    }

    private void UpdateAnimatorBools()
    {
        bool moving = _moveInput.sqrMagnitude >= inputDeadzone * inputDeadzone;
        _animator.SetBool("IsMoving", moving);

        if (!moving)
        {
            // if you want, also clear turn bools here
            _animator.SetBool("TurnLeft", false);
            _animator.SetBool("TurnRight", false);
            _hasTargetYaw = false;
        }
    }

    private void UpdateTargetYawAndTurnBools()
    {
        // World-based direction: left = (-1, 0), up = (0, +1)
        Vector3 targetDir = new Vector3(_moveInput.x, 0f, _moveInput.y);

        if (targetDir.sqrMagnitude < inputDeadzone * inputDeadzone)
        {
            _hasTargetYaw = false;
            _animator.SetBool("TurnLeft", false);
            _animator.SetBool("TurnRight", false);
            return;
        }

        targetDir.Normalize();

        // Convert direction -> yaw degrees (world)
        float targetYaw = Mathf.Atan2(targetDir.x, targetDir.z) * Mathf.Rad2Deg;
        _targetYaw = targetYaw;
        _hasTargetYaw = true;

        float currentYaw = _rb.rotation.eulerAngles.y;
        float deltaYaw = Mathf.DeltaAngle(currentYaw, _targetYaw);
        float abs = Mathf.Abs(deltaYaw);

        // Stop turning inside deadzone
        if (abs <= rotationDeadzone)
        {
            _animator.SetBool("TurnLeft", false);
            _animator.SetBool("TurnRight", false);
            return;
        }

        // DeltaAngle > 0 => target is to the right (turn right)
        bool turnRight = deltaYaw > 0f;
        _animator.SetBool("TurnRight", turnRight);
        _animator.SetBool("TurnLeft", !turnRight);
    }
}

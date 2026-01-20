using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class AiSmoothMovementController : MonoBehaviour
{
   private Rigidbody _rb;
    private Animator _animator;
    private NavMeshAgent _agent;

    // Input (world-based): x = world X, y = world Z
    private Vector2 _moveInput;

    // Root motion accumulation between physics ticks
    private Vector3 _deltaPos;

    public UnityEvent reachedWayPoint;
    private bool _isWalking;
    
    [Header("Settings")]
    [SerializeField] private float rotationDeadzone = 2f;      // degrees
    [SerializeField] private float stoppingDistance = 1f;
    [SerializeField] private float turnTime = 0.1f;

    private float _moveDeadzone = 0.15f;
    
    
    private float _yawVelocity;
    private float _currentYaw;
    private bool _hasYaw;
    

    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _animator = GetComponent<Animator>();
        _agent = GetComponent<NavMeshAgent>();
        
        _deltaPos = Vector3.zero;
        
        _agent.updatePosition = false;
        _agent.updateRotation = false;

        _agent.stoppingDistance = stoppingDistance;
    }

    private void Start()
    {
        if (reachedWayPoint == null)
        {
            reachedWayPoint = new UnityEvent();
        }
    }

    private void Update()
    {
        float distance = Vector3.Distance(_rb.position, _agent.destination);
        if (distance <= stoppingDistance && _isWalking)
        {
            reachedWayPoint.Invoke(); //reached waypoint
            _isWalking = false;
        }
    }
    
    
    void FixedUpdate()
    {
        Navigate();
        RotateCharacter(_moveInput);
        ApplyRootMotionToRigidbody();
        UpdateAnimatorBools();
    }

    void Navigate()
    {
        Vector2 direction = new Vector2(_agent.velocity.x, _agent.velocity.z); 
        _moveInput = direction;
    }
    
    void OnAnimatorMove()
    {
        // Accumulate root motion deltas until next FixedUpdate
        _deltaPos += _animator.deltaPosition;
        
        _agent.nextPosition = transform.position;
    }

    private void ApplyRootMotionToRigidbody()
    {
        // Position: keep gravity on Y (don’t inject root Y)
        Vector3 targetPos = _rb.position + _deltaPos;
        targetPos.y = _rb.position.y;
        _rb.MovePosition(targetPos);

        // Reset accumulators
        _deltaPos = Vector3.zero;
    }

    private void UpdateAnimatorBools()
    {
        bool moving = _moveInput.sqrMagnitude >= _moveDeadzone * _moveDeadzone;     
        _animator.SetBool("IsMoving", moving);
    }

    private void RotateCharacter(Vector2 cleanInput)
    {
        if (cleanInput == Vector2.zero)
            return;
        
        
        float targetYaw = Mathf.Atan2(cleanInput.x, cleanInput.y) * Mathf.Rad2Deg;

        if (!_hasYaw)
        {
            _currentYaw = _rb.rotation.eulerAngles.y;
            _hasYaw = true;
        }

        _currentYaw = Mathf.SmoothDampAngle(
            _currentYaw,
            targetYaw,
            ref _yawVelocity,
            turnTime);
        
        _rb.MoveRotation(Quaternion.Euler(0, _currentYaw, 0));
    }

    public void SetDestination(Transform destination)
    {
        _agent.SetDestination(destination.position);
        _isWalking = true;
    }
    
}

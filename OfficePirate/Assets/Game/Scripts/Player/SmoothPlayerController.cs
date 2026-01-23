using UnityEngine;
using UnityEngine.InputSystem;

public class SmoothPlayerController : MonoBehaviour
{
    private Rigidbody _rb;
    private Animator _animator;

    // Input (world-based): x = world X, y = world Z
    private Vector2 _moveInput;
    private bool isRunning;
    
    // Root motion accumulation between physics ticks
    private Vector3 _deltaPos;

    [Header("Settings")]
    [SerializeField] private float inputDeadzone = 0.15f;     // stick deadzone
    [SerializeField] private float turnTime = 0.1f;

    private float _yawVelocity;
    private float _currentYaw;
    private bool _hasYaw;
    

    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _animator = GetComponent<Animator>();
        
        _deltaPos = Vector3.zero;
    }

    public void GetMoveInput(InputAction.CallbackContext context)
    { 
            _moveInput = context.ReadValue<Vector2>();
    }

    public void GetRunInput(InputAction.CallbackContext context)
    {
        if (context.performed)
            isRunning = true;
        if (context.canceled)
            isRunning = false;
    }

    private Vector2 GetCleanMoveInput()
    {
        Vector2 v = _moveInput;

        // axis deadzone to prevent tiny sideways drift
        if (Mathf.Abs(v.x) < inputDeadzone) v.x = 0f;
        if (Mathf.Abs(v.y) < inputDeadzone) v.y = 0f;

        // optional: normalize so diagonals aren't "stronger"
        if (v.sqrMagnitude > 1f) v.Normalize();

        return v;
    }
    
    
    
    void FixedUpdate()
    {
        RotateCharacter(GetCleanMoveInput());
        ApplyRootMotionToRigidbody();
        UpdateAnimatorBools();
    }

    void OnAnimatorMove()
    {
        // Accumulate root motion deltas until next FixedUpdate
        _deltaPos += _animator.deltaPosition;
        
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
        bool moving = _moveInput.sqrMagnitude >= inputDeadzone * inputDeadzone;
        _animator.SetBool("IsMoving", moving);
        _animator.SetBool("IsRunning", isRunning);
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

    
    

}

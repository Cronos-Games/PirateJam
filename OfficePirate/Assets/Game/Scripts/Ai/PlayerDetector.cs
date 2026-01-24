using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class PlayerDetector : MonoBehaviour
{
    [Header("Settings")] 
    [SerializeField] private float detectionDistance;
    [SerializeField] private float detectionAngle;

    [SerializeField] private float eyeHeight = 1.7f;
    [SerializeField] private float targetHeight = 1.2f;
    [SerializeField] private LayerMask losMask;
    [SerializeField] private float sphereCastRadius = 0.1f;
    
    
    [Header("References")]
    [SerializeField] private SphereCollider detectionCollider;
    
    public UnityEvent onPlayerDetected;

    private void Start()
    {
        detectionCollider.radius = detectionDistance;
    }


    void OnTriggerStay(Collider other)
    {
        
        if (other.gameObject.CompareTag("Player"))
        {
            SmoothPlayerController playerController = other.gameObject.GetComponent<SmoothPlayerController>();
            
            Vector3 direction = other.transform.position - transform.position;
            Vector2 direction2D = new Vector2(direction.x, direction.z).normalized;

            Vector2 forward = new Vector2(transform.forward.x, transform.forward.z).normalized;
            
            Vector2 vectorLeft = RotateVector(forward, detectionAngle/2);
            Vector2 vectorRight = RotateVector(forward, -detectionAngle/2);
            
            if (IsBetween(vectorLeft, vectorRight, direction2D))
            {
                if (HasLineOfSight(other) && playerController.isRunning)
                {
                    onPlayerDetected.Invoke();
                    AiManager.Instance.managerAi.CallManager();
                    Debug.Log("Player detected");
                }
            }
        }
    }
    
    
    private bool IsBetween(Vector2 left, Vector2 right, Vector2 point)
    {
        float Cross(Vector2 u, Vector2 v) => u.x * v.y - u.y * v.x;

        float lr = Cross(left, right);
        float lp = Cross(left, point);
        float pr = Cross(point, right);

        if (lr >= 0)
            return lp >= 0 && pr >= 0;
        
        return !(lp >= 0 && pr >= 0);
    }

    Vector2 RotateVector(Vector2 v, float angle)
    {
        float radians = Mathf.Deg2Rad * angle;
        float x = v.x * Mathf.Cos(radians) - v.y * Mathf.Sin(radians);
        float y = v.x * Mathf.Sin(radians) + v.y * Mathf.Cos(radians);
        return new Vector2(x, y);
    }


    private bool HasLineOfSight(Collider targetCollider)
    {
        Vector3 eye = transform.position + Vector3.up * eyeHeight;

        Vector3 target = targetCollider.bounds.center;
        target.y = targetCollider.transform.position.y + targetHeight;

        Vector3 toTarget = target - eye;
        float distance = toTarget.magnitude;
        if (distance < 0.001f)
            return true;
        
        Vector3 direction = toTarget / distance;

        RaycastHit hit;
        bool hasHit;

        if (sphereCastRadius > 0f)
        {
            hasHit = Physics.SphereCast(
                eye,
                sphereCastRadius,
                direction,
                out hit,
                distance,
                losMask,
                QueryTriggerInteraction.Ignore
            );
        }
        else
        {
            hasHit = Physics.Raycast(
                eye,
                direction,
                out hit,
                distance,
                losMask,
                QueryTriggerInteraction.Ignore
            );
        }

        if (!hasHit)
            return true;
        
        return hit.collider.transform.root == targetCollider.transform.root;
    }
    
}

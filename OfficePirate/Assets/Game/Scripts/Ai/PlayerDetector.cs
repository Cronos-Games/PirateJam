using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class PlayerDetector : MonoBehaviour
{
    [Header("Settings")] 
    [SerializeField] private float detectionDistance;
    [SerializeField] private float detectionAngle;
    
    [Header("References")]
    [SerializeField] private Collider detectionCollider;
    
    public UnityEvent onPlayerDetected;


    void OnTriggerStay(Collider other)
    {
        
        if (other.gameObject.CompareTag("Player"))
        {
            Vector3 direction = other.transform.position - transform.position;
            Vector2 direction2D = new Vector2(direction.x, direction.z);

            Vector2 forward = new Vector2(transform.forward.x, transform.forward.z).normalized;
            
            Vector2 vectorLeft = RotateVector(forward, detectionAngle/2);
            Vector2 vectorRight = RotateVector(forward, -detectionAngle/2);
            
            if (IsBetween(vectorLeft, vectorRight, direction2D))
            {
                onPlayerDetected.Invoke();
                AiManager.Instance.managerAi.CallManager();
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
        float x = v.x * Mathf.Cos(angle) - v.y * Mathf.Sin(angle);
        float y = v.x * Mathf.Sin(angle) + v.y * Mathf.Cos(angle);
        return new Vector2(x, y);
    }

    
}

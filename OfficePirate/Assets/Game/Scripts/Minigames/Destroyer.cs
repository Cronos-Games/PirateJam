using System;
using UnityEngine;
using UnityEngine.Events;

public class Destroyer : MonoBehaviour
{
    [SerializeField] private UnityEvent onDestroy;
    public void Destroy()
    {
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        onDestroy.Invoke();
    }
}
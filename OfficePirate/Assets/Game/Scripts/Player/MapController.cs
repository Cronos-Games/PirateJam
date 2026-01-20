using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class MapController : MonoBehaviour
{
    private Camera _mainCamera;
    [SerializeField] private Camera mapCamera;

    private void Start()
    {
        _mainCamera = Camera.main;
    }

    public void ToggleMap(InputAction.CallbackContext context)
    {
        _mainCamera.enabled = !_mainCamera.enabled;
        mapCamera.enabled = !mapCamera.enabled;
    }
    
    
    
}

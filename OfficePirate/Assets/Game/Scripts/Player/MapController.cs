using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class MapController : MonoBehaviour
{
    private Camera _mainCamera;
    [SerializeField] private Camera mapCamera;
    [SerializeField] private InputActionReference playerMoveAction;
    public bool Enabled = true;

    private void Start()
    {
        _mainCamera = Camera.main;
    }

    public void ToggleMap(InputAction.CallbackContext context)
    {
        if (!Enabled) return;
        _mainCamera.enabled = !_mainCamera.enabled;
        mapCamera.enabled = !mapCamera.enabled;
        
        TogglePlayerMove();
    }

    private void TogglePlayerMove()
    {
        if (_mainCamera.enabled)
        {
            playerMoveAction.action.Enable();
        }
        else
        {
            playerMoveAction.action.Disable();
        }
    }
    
    
    
}

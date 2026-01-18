using UnityEngine;

public interface IInteractable
{
    Transform Transform { get; }
    bool CanInteract { get; }
    int Priority { get; } 
    void Interact();
}
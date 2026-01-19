using UnityEngine;

public interface IInteractable
{
    Transform Transform { get; }
    bool CanInteract { get; }
    int Priority { get; } 
    float UpperTimeLimit { get; }
    float LowerTimeLimit { get; }
    void Interact();
}
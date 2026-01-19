using System;
using UnityEngine;

public class MiniGameTester : MonoBehaviour
{
    [SerializeField] private MiniGameController miniGameController;

    private void Start()
    {
        Instantiate(miniGameController.gameObject);
    }
}

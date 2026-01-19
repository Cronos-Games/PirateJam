using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NPCAi : MonoBehaviour
{
    AiMovementController _movementController;

    [SerializeField] private Transform basePosition;
    [SerializeField] private Transform[] targets;
    [SerializeField] private float timeoutInSeconds;

    
    private List<Transform> Path = new List<Transform>();

    void Start()
    {
        
    }


    void Update()
    {
        
    }

    //create path to check
    private void createPath(bool allTargets, bool reverse = false)
    {
        Path.Clear();
        if (allTargets)
        {
            if (!reverse)
            {
                for (int i = 0; i < targets.Length; i++)
                {
                    Path.Add(targets[i]);
                }
            }
            else
            {
                for (int i = targets.Length; i > 0; i--)
                {
                    Path.Add(targets[i - 1]);
                }
            }

        }
        else
        {
            Transform randomTarget =  targets[Random.Range(0, targets.Length)];
            Path.Add(randomTarget);
        }
        Path.Append(basePosition);
    }

    public void OnReachedWaypoint()
    {
        //random timeout (time range for specific waypoint?
        //move to next waypoint
    }
}

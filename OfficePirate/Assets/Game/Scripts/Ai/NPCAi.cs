using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NPCAi : MonoBehaviour
{
    AiMovementController _movementController;
    AiSmoothMovementController _smoothMovementController;

    [SerializeField] private GameObject basePosition;
    [SerializeField] private GameObject[] targets;
    [SerializeField] private float baseUpperTimeLimit;
    [SerializeField] private float baseLowerTimeLimit;

    
    private List<GameObject> _path = new List<GameObject>();
    private GameObject _currentTarget;

    private float _startTime;
    private float _endTime;
    private bool _timerRunning = false;
    

    void Start()
    {
        _movementController = GetComponent<AiMovementController>();
        _smoothMovementController = GetComponent<AiSmoothMovementController>();
        
        CreatePath(true, false);
        _currentTarget = _path.First();
        //_movementController.SetDestination(_currentTarget.transform);
        _smoothMovementController.SetDestination(_currentTarget.transform);
    }


    void Update()
    {
        Tick();
    }
    
    
    private void StartTimer(float timeInSeconds)
    {
        _startTime = Time.timeSinceLevelLoad;
        _endTime =  _startTime + timeInSeconds;
        _timerRunning = true;
    }

    private void Tick()
    {
        if (_timerRunning &&  Time.timeSinceLevelLoad > _endTime)
        {
            TimerEnded();
            _timerRunning = false;
        }
    }
    

    //create path to check
    private void CreatePath(bool allTargets, bool reverse = false)
    {
        _path.Clear();
        if (allTargets)
        {
            if (!reverse)
            {
                for (int i = 0; i < targets.Length; i++)
                {
                    _path.Add(targets[i]);
                }
            }
            else
            {
                for (int i = targets.Length; i > 0; i--)
                {
                    _path.Add(targets[i - 1]);
                }
            }

        }
        else
        {
            GameObject randomTarget =  targets[Random.Range(0, targets.Length)];
            _path.Add(randomTarget);
        }
        _path.Add(basePosition);
    }

    public void OnReachedWaypoint()
    {
        float timeOut = 0;
        if (_path.Count > 1)
        {
            ProcessController pc = _path.First().GetComponent<ProcessController>();
        
            //timeout with random time within range for waypoint
            timeOut = Random.Range(pc.LowerTimeLimit, pc.UpperTimeLimit);
        }
        else
        {
            timeOut = Random.Range(baseLowerTimeLimit, baseUpperTimeLimit);
        }

        StartTimer(timeOut); //TODO: interact with target
    }

 
    
    private void TimerEnded()
    {
        if (_path.Count > 1)
        {
            _path.Remove(_currentTarget);

        }

        _currentTarget = _path.First();
        //_movementController.SetDestination(_currentTarget.transform);
        _smoothMovementController.SetDestination(_currentTarget.transform);

        if (_path.Count == 1)
        {
            CreatePath(true, false);
        }
    }
}

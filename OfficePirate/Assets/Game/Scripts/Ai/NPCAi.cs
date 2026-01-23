using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NPCAi : MonoBehaviour
{
    AiSmoothMovementController _smoothMovementController;

    [SerializeField] private GameObject basePosition;
    [SerializeField] private float baseUpperTimeLimit;
    [SerializeField] private float baseLowerTimeLimit;
    [SerializeField] private int minTargets;
    [SerializeField] private int maxTargets;

    
    private List<GameObject> _path = new List<GameObject>();
    private GameObject _currentTarget;

    private float _startTime;
    private float _endTime;
    private bool _timerRunning = false;
    

    void Start()
    {
        _smoothMovementController = GetComponent<AiSmoothMovementController>();
        
        GetPath(minTargets, maxTargets);
        
        _currentTarget = _path.First();
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
    


    private void GetPath(int minimumTargets, int maximumTargets)
    {
        _path.Clear();
        
        int amountOfTargets =  Random.Range(minimumTargets, maximumTargets + 1);
        List<GameObject> targets = AiManager.Instance.GetRandomTargets(amountOfTargets);
        
        foreach (GameObject target in targets)
        {
            _path.Add(target);
        }
        
        _path.Add(basePosition);
    }
    
    public void OnReachedWaypoint()
    {
        float timeOut = 0;
        if (_path.Count > 1)
        {
            ProcessController pc = _currentTarget.GetComponent<ProcessController>();
            
            //interact with waypoint, either fixing it or adding progress and get time to wait before next checkpoint
            timeOut = pc.AiInteract();
        }
        else
        {   
            timeOut = Random.Range(baseLowerTimeLimit, baseUpperTimeLimit);
        }

        StartTimer(timeOut); //TODO: interact with target
    }

 
    
    private void TimerEnded()
    {
        if (_path.Count == 0)
        {
            GetPath(minTargets, maxTargets);
        }
        
        _currentTarget = _path[0];
        
        if (_currentTarget != basePosition && !AiManager.Instance.availableTargets.Contains(_currentTarget))
            AiManager.Instance.availableTargets.Add(_currentTarget);
        
        _path.RemoveAt(0);

        if (_path.Count == 0)
            GetPath(minTargets, maxTargets);

        _currentTarget = _path[0];
        _smoothMovementController.SetDestination(_currentTarget.transform);
    }
}

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ManagerAi : MonoBehaviour
{
    AiSmoothMovementController _smoothMovementController;

    [SerializeField] private float interactionMultiplier = 0.7f;
    [SerializeField] private GameObject basePosition;

    
    private List<GameObject> _path = new List<GameObject>();
    private GameObject _currentTarget;

    private float _startTime;
    private float _endTime;
    private bool _timerRunning = false;
    
    void Start()
    {
        _smoothMovementController = GetComponent<AiSmoothMovementController>();
        
        CallManager();
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

    
    private void GetPath()
    {
        List<GameObject> targets = AiManager.Instance.GetAllInteractables();

        _path.Clear();
        
        foreach (GameObject target in targets)
            _path.Add(target);
        
        _path.Add(basePosition);
    }
    
    public void OnReachedWaypoint()
    {
        float timeOut = 0;
        if (_path.Count > 1)
        {
            ProcessController pc = _currentTarget.GetComponent<ProcessController>();
            
            //interact with waypoint, either fixing it or adding progress and get time to wait before next checkpoint
            timeOut = pc.AiInteract(interactionMultiplier);
            StartTimer(timeOut);
        }
    }

    private void TimerEnded()
    {
        _currentTarget = _path[0];
        
        _path.RemoveAt(0);
        
        _currentTarget = _path[0];
        _smoothMovementController.SetDestination(_currentTarget.transform);
    }

    public void CallManager()
    {
        GetPath();
        
        _currentTarget = _path.First();
        _smoothMovementController.SetDestination(_currentTarget.transform);
    }
    
}

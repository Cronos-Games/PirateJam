using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class TimeController : MonoBehaviour
{

    [Header("Settings")] 
    [SerializeField] private float dayDurationInSeconds = 300f;
    
    [SerializeField] private int dayStartHour = 9;
    [SerializeField] private int dayEndHour = 17;
    
    [Header("References")]
    [SerializeField] TextMeshProUGUI timeText;


    [Header("Events")] 
    public UnityEvent onDayStart;
    public UnityEvent onDayEnd;
    
    //timer properties
    private float _startTime = 0f;
    private bool _timerRunning = false;
    


    private void Start()
    {
        onDayStart = new UnityEvent();
        onDayEnd = new UnityEvent();
        
        StartTimer();
    }

    private void Update()
    {
        Tick();
    }

    private void Tick()
    {
        if (_timerRunning)
        {
            if (Time.timeSinceLevelLoad - _startTime > dayDurationInSeconds)
            {
                onDayEnd.Invoke();
                StartTimer();
            }
            UpdateTimeText(Time.timeSinceLevelLoad - _startTime);
        }
    }

    //start day timer
    public void StartTimer()
    {
        _startTime = Time.timeSinceLevelLoad;
        _timerRunning = true;
        onDayStart.Invoke();
    }

    private void UpdateTimeText(float currentTimeInSeconds)
    {
        string ampmText = "";
        string displayHours = "";
        string displayMinutes = "";
        
        float totalDayHours = dayEndHour - dayStartHour;
        float totalDayMinutes = totalDayHours * 60;
        
        float hourDivide = dayDurationInSeconds / totalDayHours;
        float minuteDivide = dayDurationInSeconds / totalDayMinutes;
        
        
        int currentHourAmount = Mathf.FloorToInt(currentTimeInSeconds / hourDivide);

        int currentHour = dayStartHour + currentHourAmount;
        int currentMinute = Mathf.FloorToInt((currentTimeInSeconds / minuteDivide) - (currentHourAmount * 60));

        if (currentHour < 12)
        {
            ampmText = " AM";
        }
        else
        {
            ampmText = " PM";
        }

        if (currentHour == 0)
        {
            displayHours = "12";
        } else if (currentHour > 12)
        {
            displayHours = (currentHour - 12).ToString();
        }
        else
        {
            displayHours = currentHour.ToString();
        }

        if (currentMinute < 10)
        {
            displayMinutes = "0" + currentMinute.ToString();
        }
        else
        {
            displayMinutes = currentMinute.ToString();
        }
        

        timeText.text = displayHours + ":" + displayMinutes + ampmText;
    }
}

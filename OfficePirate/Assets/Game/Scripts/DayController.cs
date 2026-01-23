using System.Collections;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class DayController : MonoBehaviour
    {
        
        [SerializeField] private UnityEvent<int> onDayStarted;
        [SerializeField] private GameObject dayCutscene;
        [SerializeField] private GameObject infoTextBar;
        [SerializeField] private TMP_Text dayText;
        [SerializeField] private TimeController timeController;
        
        private int _day = 0;
        private void Start()
        {
            NewDay();
        }
        
        [Button]
        public void NewDay()
        {
            
            StartCoroutine(StartNewDay());
        }  
        
        private IEnumerator StartNewDay()
        {
            _day++;
            onDayStarted.Invoke(_day);
            yield return new WaitForFixedUpdate();
            dayText.text = $"Day {_day}";
            dayCutscene.SetActive(true);
            
            yield return new WaitForSeconds(3f);
            
            dayCutscene.SetActive(false);
            
            timeController.StartTimer();
        }
    }

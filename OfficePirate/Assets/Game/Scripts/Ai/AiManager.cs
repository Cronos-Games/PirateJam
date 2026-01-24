using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class AiManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject[] Interactables;
    
    public ManagerAi managerAi;
    
    public static AiManager Instance;
    public List<GameObject> availableTargets;
    
    
    

    private void Awake()
    {
        availableTargets = new List<GameObject>();
        
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        
        foreach (GameObject target in Interactables)
        {
            availableTargets.Add(target);
        }
        
        managerAi = GameObject.FindGameObjectWithTag("Manager").GetComponent<ManagerAi>();
    }


    public List<GameObject> GetRandomTargets(int amount)
    {
        List<GameObject> returnList = new List<GameObject>();
        int randomIndex = 0;
        
        for (int i = 0; i < amount; i++)
        {
            randomIndex = Random.Range(0, availableTargets.Count);
            returnList.Add(availableTargets[randomIndex]);
            availableTargets.Remove(availableTargets[randomIndex]);
        }
            
        return returnList;
    }

    public List<GameObject> GetAllInteractables()
    {
        List<GameObject> returnList = new List<GameObject>();
        foreach (GameObject interactable in Interactables)
        {
            returnList.Add(interactable);
        }
        return returnList;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class BusStorageUnit : StorageUnit
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void StartWaiting(int secToWait)
    {
        waitSlider.gameObject.SetActive(true);
        waitPoints = secToWait;
        StartCoroutine("ObjectWaiting");

    }
    IEnumerator ObjectWaiting()
    {

        while (waitPoints >= 0)
        {
            yield return new WaitForSeconds(1f);
            waitPoints -= 1;
            int waitColorCoef;
            if (nextWaitPoints != 0)
            {
                waitColorCoef = 100 * waitPoints / nextWaitPoints;
            }
            else
            {
                waitColorCoef = 1;

            }
            if (waitSlider != null)
            {
                waitSlider.value = waitPoints;
                waitColor = new Color((1f - waitColorCoef / 100f), 0.3f, waitColorCoef / 100f, 0.7f);
                fillColor.GetComponent<Image>().color = waitColor;
            }

        }
        waitSlider.gameObject.SetActive(false);
        TruckExit(-100);

    }
    private void OnTriggerEnter(Collider other)
    {
        

        if (other.gameObject.name == "StopPositionTrucks")
        {
            
            waitSlider.value = waitSlider.maxValue;
            StartWaiting(nextWaitPoints);

        }
    }
    private void OnTriggerExit(Collider other)
    {
        Debug.Log("Exit collision with " + other.name);
        if (other.gameObject.name == "StopPositionTrucks")
        {
            waitSlider.gameObject.SetActive(false);
            StopCoroutine("ObjectWaiting");
            loadNeed = 0;
            currentLoad = 0;
        }
    }
    public void AddBoxToLoad()
    {
        currentLoad++;
    }

    public bool CheckLoad()
    {
        if (loadNeed != 0 && currentLoad == loadNeed)
        {

            TruckExit(150); // add 150 piots if car was loaded fully in time
            return true;
        }
        else
        {
            return false;
        }
    }
    public void TruckEnter()
    {
        if (gameObject.CompareTag("Track") && gameObject.GetComponent<NavMeshAgent>() != null)
        {
            Vector3 currentPos = transform.position;
            GetComponent<NavMeshAgent>().SetDestination(currentPos + new Vector3(50, 0, 0));
        }
    }
    public void TruckExit(int exitPoints)
    {
        if (gameObject.CompareTag("Track") && gameObject.GetComponent<NavMeshAgent>() != null)
        {
            Vector3 currentPos = transform.position;
            GetComponent<NavMeshAgent>().SetDestination(currentPos - new Vector3(50, 0, 0));
            GameManager.instance.truks.Add(gameObject);
            PlayerController.instance.AddPoints(exitPoints);

        }
    }
    public void NewTruckTask(int taskBoxNumb)
    {
        loadNeed = taskBoxNumb;
        nextWaitPoints = loadNeed * 75;
        waitSlider.maxValue = nextWaitPoints;
        TruckEnter();
    }
}

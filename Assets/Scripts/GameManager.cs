using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using TMPro;

public class GameManager : MonoBehaviour
{
    private int levelTargetPoint;
    public int currentPoint;

    private float timeRemaining = 200;
    public Text timeText;
    [SerializeField] GameObject timer;
    [SerializeField] TextMeshProUGUI missionText;



    private int boxSpawnTime = 5;
    private IEnumerator coroutine;

    [SerializeField] public List<GameObject> emptyRacks;
    [SerializeField] public List<GameObject> targetRacks;
    [SerializeField] public List<GameObject> storedRacks;
    [SerializeField] public List<GameObject> storedTargetRacks;

    [SerializeField] public List<GameObject> emptyResources;
    [SerializeField] public List<GameObject> storedResources;


   
    [SerializeField] private int waitTime = 1;

    public static GameManager instance;

    public GameObject taskBar;
    public Material gameTaskMat;
    public Material secondMat;
    public int missionNumber;
    

    // Start is called before the first frame update
    private string[] cheatCode;
    private int index2;

    void Start()
    {
       instance = this;
        NextMissionText(missionNumber);

        GameObject[] allRacks = GameObject.FindGameObjectsWithTag("Rack"); // should be
        //Racks = GameObject.FindGameObjectsWithTag("Rack"); // should be
        emptyRacks.AddRange(allRacks); // should be
        //for (int i = 0; i < emptyRacks.Count; i++)
        GameObject[] allResources = GameObject.FindGameObjectsWithTag("Resources");
        // gameobject allResources = GameObject.FindGameObjectsWithTag("Resources");
        emptyResources.AddRange(allResources);


        //Cheat code implementation
        // Code is "jebacpsy", user needs to input this in the right order
        cheatCode = new string[] { "j", "e", "b", "a", "c", "p", "s", "y" };
        index2 = 0;
    }
    private void Awake()
    {
        Debug.Log("Mission "+ missionNumber + " begins");
        GameObject Task1Panel = GameObject.Find("MissionTask1");
        Task1Panel.GetComponent<Animator>().SetTrigger("Open");
        //StartNextMission();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.anyKeyDown)
        {
            // Check if the next key in the code is pressed
            if (Input.GetKeyDown(cheatCode[index2]))
            {
                // Add 1 to index to check the next key in the code
                index2++;
            }
            // Wrong key entered, we reset code typing
            else
            {
                index2 = 0;
            }
        }

        // If index reaches the length of the cheatCode string, 
        // the entire code was correctly entered
        if (index2 == cheatCode.Length) // Change later in case of repeateng function every frame
        {
            GameObject.Find("Player").GetComponent<NavMeshAgent>().speed = 70;
            GameObject.Find("Player").GetComponent<NavMeshAgent>().acceleration = 600;
            
            //playerAgent.transform.
            // Cheat code successfully inputted!
            // Unlock crazy cheat code stuff
        }

        if (timeRemaining > 0) // Timer work
        {
            timeRemaining -= Time.deltaTime;
            Displaytime(timeRemaining);
        }
        else
        {
            Displaytime(0);
        }
    }
   
    public void Mission1()
    {

        //missionText.text = "Welcome to the your new job! \nYour first task: Collect box from pallets on the left. \nPut it on the specified Rack.";
        taskBar.GetComponent<Animator>().SetTrigger("OpenTaskBar");
        int randomResource = Random.Range(0, emptyResources.Count);
        emptyResources[randomResource].gameObject.transform.GetChild(0).gameObject.SetActive(true);
        emptyResources[randomResource].gameObject.GetComponent<StorageUnit>().isStored = true;
        emptyResources[randomResource].gameObject.GetComponent<StorageUnit>().objectType = 0;
        storedResources.Add(emptyResources[randomResource]);
        emptyResources.Remove(emptyResources[randomResource]);


        int randomRack = Random.Range(0, emptyRacks.Count);
        //Racks[randomRack].transform.Rotate(0f, 90f, 0f);
        emptyRacks[randomRack].transform.GetComponentInChildren<MeshRenderer>().material = gameTaskMat;
        emptyRacks[randomRack].tag = "Target";
        targetRacks.Add(emptyRacks[randomRack]);
        emptyRacks.Remove(emptyRacks[randomRack]);
        PlayerController.instance.inMenu = false;

    }
    public void Mission2()
    {
        taskBar.GetComponent<Animator>().SetTrigger("OpenTaskBar");
        PlayerController.instance.inMenu = false;

        foreach (GameObject resourePallete in emptyResources)
        {
            resourePallete.gameObject.transform.GetChild(0).gameObject.SetActive(true);
            resourePallete.gameObject.GetComponent<StorageUnit>().isStored = true;
            resourePallete.gameObject.GetComponent<StorageUnit>().objectType = 1;
            storedResources.Add(resourePallete);
        }
        emptyResources.Clear();
        for (int i = 0; i < storedResources.Count; i++)
        {
            int randomRack = Random.Range(0, emptyRacks.Count);
            emptyRacks[randomRack].transform.GetComponentInChildren<MeshRenderer>().material = gameTaskMat;
            emptyRacks[randomRack].tag = "Target";
            
            targetRacks.Add(emptyRacks[randomRack]);
            emptyRacks.Remove(emptyRacks[randomRack]);
        }
        //taskBar.transform.GetChild(0).gameObject.transform.Find("Label").GetComponent<Text>().text = "0 of 4 box has set";
        taskBar.transform.GetChild(0).GetComponent<Toggle>().isOn = false;
        taskBar.transform.GetChild(1).gameObject.SetActive(false);
        taskBar.transform.GetChild(2).GetComponent<Text>().text = "Mission 2 tasks:";


    }
    public void Mission3()
    {
        GameObject.Find("Boss").GetComponent<BossController>().BossEnter();

        // Tast Text Edit
        taskBar.GetComponent<Animator>().SetTrigger("OpenTaskBar");
        //taskBar.transform.GetChild(0).gameObject.transform.Find("Label").GetComponent<Text>().text = "Talk to your boss";
        taskBar.transform.GetChild(0).GetComponent<Toggle>().isOn = false;
        taskBar.transform.GetChild(1).gameObject.SetActive(true);
        taskBar.transform.GetChild(1).gameObject.transform.Find("Label").GetComponent<Text>().text = "......?";
        taskBar.transform.GetChild(1).GetComponent<Toggle>().isOn = false;
        taskBar.transform.GetChild(2).GetComponent<Text>().text = "Mission 3 tasks:";
    }
    public void Mission4()
    {
        currentPoint = 0;
        levelTargetPoint = 1000;
        timeRemaining = 90;

        GameObject[] Tracks = GameObject.FindGameObjectsWithTag("Track"); 
        foreach (GameObject track in Tracks)
        {
            track.GetComponent<StorageUnit>().StartWaiting(100);
        }
        


        taskBar.transform.GetChild(0).gameObject.SetActive(true);
        taskBar.GetComponent<Animator>().SetTrigger("OpenTaskBar");
        ChangeLevelPoints();
        timer.SetActive(true);
        coroutine = IncomeBoxSpawner(boxSpawnTime);
        StartCoroutine(coroutine);
        taskBar.transform.GetChild(2).GetComponent<Text>().text = "ARCADE:";
        
        taskBar.transform.GetChild(0).GetComponent<Toggle>().isOn = false;
        taskBar.transform.GetChild(1).gameObject.SetActive(true);
        taskBar.transform.GetChild(1).gameObject.SetActive(false);
        
    }
    void NextArcadeMission()
    {
        Debug.Log("Next Arcade Mission");
        currentPoint = 0;
        levelTargetPoint = levelTargetPoint+ levelTargetPoint/10;
        timeRemaining = 90;

        GameObject[] Tracks = GameObject.FindGameObjectsWithTag("Track");
        foreach (GameObject track in Tracks)
        {
            track.GetComponent<StorageUnit>().StartWaiting(100);
        }

        ChangeLevelPoints();
        timer.SetActive(true);
        
    }
    



    public void StartNextMission()
    {
        ClearTargetRack();
        if (missionNumber == 1)  // Firts task - 1 box
        {
            Mission1(); 
        }
        if (missionNumber == 2) //4 boxes
        {
            Mission2(); 
        }
        if (missionNumber == 3)// Boss income
        {
            Mission3(); 
        }
        if (missionNumber == 4)
        {
            Mission4(); // Arcade mode
        }
        if (missionNumber > 4)
        {
            NextArcadeMission(); // NextArcadeMission
        }

        PlayerController.instance.inMenu = false;
    }

    void ClearTargetRack()
    {
        /* foreach (GameObject targetRack in storedTargetRacks)
         {

            storedRacks.Add(targetRack.gameObject);
            
         } 
        */
        foreach (GameObject rak in storedTargetRacks)
        {
            rak.tag = "Rack";
        }
        storedTargetRacks.Clear();
    }

    IEnumerator IncomeBoxSpawner(float loadTime)
    {
       
        while (true)
        {
            yield return new WaitForSeconds(loadTime);
            boxSpawnTime = Random.Range(15, 35); // Spawn Rate min-max
            Debug.Log("Next box spawn in  " + boxSpawnTime + " seconds");
            if (emptyResources.Count != 0)
            {
                int randomResource = Random.Range(0, emptyResources.Count);
                int randomBoxType = Random.Range(0, 3);

                emptyResources[randomResource].gameObject.transform.GetChild(0).gameObject.SetActive(true);
                emptyResources[randomResource].gameObject.GetComponent<StorageUnit>().isStored = true;
                emptyResources[randomResource].gameObject.GetComponent<StorageUnit>().objectType = randomBoxType;
                storedResources.Add(emptyResources[randomResource]);
                emptyResources.Remove(emptyResources[randomResource]);
                Debug.Log(emptyResources.Count + "empty palets left");
            }
            else
            {
                Debug.Log("No more place for storing income boxes");
            }
        }
        
    }
    void Displaytime(float timeToDisplay)
    {
        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);

        timeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        if (timeToDisplay < 0)
        {
            TimeIsOver();
        }
    }
    void NextMissionText(int x)
    {
        if (x == 1)
        {
            missionText.text = "Welcome to the your new job! \nYour first task: Collect box from pallets on the left. \nPut it on the specified Rack.";
        }
        else if (x == 2)
        {
            missionText.text = "0 of 4 box has set";
        }
        else if (x == 3)
        {
            missionText.text = "Talk to your boss";
        }
        else if (x == 4)
        {
            missionText.text = "ARCADE MODE! \n Bring box to cars. Check colour first";
        }
        else if (x == 5)
        {

        }
    }
    public void ChangeLevelPoints()
    {
        taskBar.transform.GetChild(0).gameObject.transform.Find("Label").GetComponent<Text>().text ="POINTS:   " + currentPoint + " / " + levelTargetPoint;
    }


    void CheckArcadeCondition()
    {

    }
    void TimeIsOver()
    {
        if (currentPoint <= levelTargetPoint)
        {
            missionText.text = "TIME IS OVER\nYou've got " + currentPoint + " of " + levelTargetPoint + "points \n Try Again";
            missionNumber = 4;
            Debug.Log("Repeat Arcade Mission");
        }
        else
        {
            missionText.text = "NICE JOB! \nYou've got "+ currentPoint + " of "+levelTargetPoint + "points \n Next Level.";
            int moneyToAdd = currentPoint - levelTargetPoint;
            PlayerController.instance.AddMoney(moneyToAdd);
            missionNumber++;
            
        }       
        GameObject.Find("MissionTask1").gameObject.GetComponent<Animator>().SetTrigger("Open");
        PlayerController.instance.inMenu = true;

    }


}


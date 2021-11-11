using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class GameManager : MonoBehaviour
{
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
    private int index;

    void Start()
    {
       instance = this;
        

        GameObject[] allRacks = GameObject.FindGameObjectsWithTag("Rack"); // should be
        //Racks = GameObject.FindGameObjectsWithTag("Rack"); // should be
        emptyRacks.AddRange(allRacks); // should be
        //for (int i = 0; i < emptyRacks.Count; i++)
        GameObject[] allResources = GameObject.FindGameObjectsWithTag("Resources");
        // gameobject allResources = GameObject.FindGameObjectsWithTag("Resources");
        emptyResources.AddRange(allResources);

        missionNumber = 0;

        //Cheat code implementation
        // Code is "jebacpsy", user needs to input this in the right order
        cheatCode = new string[] { "j", "e", "b", "a", "c", "p", "s", "y" };
        index = 0;
    }
    private void Awake()
    {
        GameObject Task1Panel = GameObject.Find("MissionTask1");
        Task1Panel.GetComponent<Animator>().SetTrigger("Open");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.anyKeyDown)
        {
            // Check if the next key in the code is pressed
            if (Input.GetKeyDown(cheatCode[index]))
            {
                // Add 1 to index to check the next key in the code
                index++;
            }
            // Wrong key entered, we reset code typing
            else
            {
                index = 0;
            }
        }

        // If index reaches the length of the cheatCode string, 
        // the entire code was correctly entered
        if (index == cheatCode.Length)
        {
            GameObject.Find("Player").GetComponent<NavMeshAgent>().speed = 70;
            GameObject.Find("Player").GetComponent<NavMeshAgent>().acceleration = 600;
            Debug.Log("Cheet code" + cheatCode.ToString() + " is activated") ;
            //playerAgent.transform.
            // Cheat code successfully inputted!
            // Unlock crazy cheat code stuff
        }
    }
   
    public void Mission1()
    {
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
        taskBar.transform.GetChild(0).gameObject.transform.Find("Label").GetComponent<Text>().text = "0 of 4 box has set";
        taskBar.transform.GetChild(0).GetComponent<Toggle>().isOn = false;
        taskBar.transform.GetChild(1).gameObject.SetActive(false);
        taskBar.transform.GetChild(2).GetComponent<Text>().text = "Mission 2 tasks:";


    }
    public void Mission3()
    {
        GameObject.Find("Boss").GetComponent<BossController>().BossEnter();


        // Tast Text Edit
        taskBar.GetComponent<Animator>().SetTrigger("OpenTaskBar");
        taskBar.transform.GetChild(0).gameObject.transform.Find("Label").GetComponent<Text>().text = "Talk to your boss";
        taskBar.transform.GetChild(0).GetComponent<Toggle>().isOn = false;
        taskBar.transform.GetChild(1).gameObject.SetActive(true);
        taskBar.transform.GetChild(1).gameObject.transform.Find("Label").GetComponent<Text>().text = "......?";
        taskBar.transform.GetChild(1).GetComponent<Toggle>().isOn = false;
        taskBar.transform.GetChild(2).GetComponent<Text>().text = "Mission 3 tasks:";
    }
    public void Mission4()
    {
        coroutine = IncomeBoxSpawner(boxSpawnTime);
        StartCoroutine(coroutine);
        taskBar.transform.GetChild(2).GetComponent<Text>().text = "ARCADE:";
        taskBar.transform.GetChild(0).gameObject.transform.Find("Label").GetComponent<Text>().text = "Bring box to cars. Check colour first";
        taskBar.transform.GetChild(0).GetComponent<Toggle>().isOn = false;
        taskBar.transform.GetChild(1).gameObject.SetActive(true);
        taskBar.transform.GetChild(1).gameObject.SetActive(false);
        
    }



    public void StartNextMission()
    {
        ClearTargetRack();
        if (missionNumber == 0)  // Firts task - 1 box
        {
            Mission1(); 
        }
        if (missionNumber == 1) //4 boxes
        {
            Mission2(); 
        }
        if (missionNumber == 2)// Boss income
        {
            Mission3(); 
        }
        if (missionNumber == 3)
        {
            Mission4(); // Arcade mode
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


}


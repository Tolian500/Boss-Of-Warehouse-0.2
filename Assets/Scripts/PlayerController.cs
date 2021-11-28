using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using TMPro;


public class PlayerController : HumanController
{
    public static PlayerController instance;
    [SerializeField] private GameObject box1;

    public bool hasBox;
    [SerializeField] int objectType;
    public Canvas canvas;
    public Material mainMaterial;
    [SerializeField] private Material previousMaterial;
    public GameObject task1Toggle;
    public GameObject task2Toggle;
    [SerializeField] TextMeshProUGUI mainText;
    private GameObject taskbar;

    [SerializeField] float loadTime;
    private IEnumerator coroutine;
    public bool inMenu;
    [SerializeField] AudioSource clickSound;
    [SerializeField] GameObject audioEnvironment;
    [SerializeField] string playerName;
    int boxHasSet;

    [SerializeField] GameObject selectedObject;
    [SerializeField] GameObject targetObject;
    private GameObject previousTarget;

    private Camera topCamera;
    public GameObject selectIndicator;

    

    [SerializeField] private List<Material> previousMaterials;
    [SerializeField] private Material[] typeMaterials;

    private int money;
    [SerializeField] int moneyFlow;







    private void Start()
    {
        instance = this;
        hasBox = false;




    }
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        inMenu = true;
        audioEnvironment = GameObject.Find("AudioEnvironment");
        if (DataSavier.Instance == null)
        {
            playerName = "New Player";
        }
        else
        {
            playerName = DataSavier.Instance.currentPlayerName;
        }
        isSelected = true;
        taskbar = GameObject.Find("MissionPanel");
        //loadSlider = gameObject.transform.GetChild(1).transform.GetChild(0).GetComponent<Slider>();
        money = 0;

    }


    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(1) & !inMenu & isSelected)
        {
            ResetAndClearTarget();
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit)) // Navigatin main player
            {
                
                if (hit.transform.gameObject.GetComponent<StorageUnit>())
                {
                    agent.SetDestination(hit.transform.position);
                    clickSound.Play();

                    hit.transform.gameObject.GetComponentInParent<StorageUnit>().isSelected = true;
                    if (hit.transform.gameObject.CompareTag("Track")) // Slided Truck direction
                    {
                        agent.SetDestination(hit.transform.position + new Vector3(5, 0));
                       
                    }
                    
                    
                    targetObject = hit.transform.gameObject;
                    previousTarget = targetObject;
                    
                    selectIndicator.transform.position = hit.transform.position + new Vector3(0, 3);
                }
                

                else
                {
                    agent.SetDestination(hit.point);
                    selectIndicator.transform.position = hit.point;
                    targetObject = null;
                    clickSound.Play();
                }
                clickSound.Play();
            }
        }
        else if (Input.GetMouseButtonDown(1) & !inMenu & !isSelected) // Navigating NPC
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit)  /*&&  !hit.transform.CompareTag("NPC") */)
            {



                if (hit.transform.gameObject.CompareTag("Rack"))
                {
                    selectedObject.gameObject.GetComponent<NavMeshAgent>().SetDestination(hit.transform.position);
                    clickSound.Play();
                }
                else
                {
                    selectedObject.gameObject.GetComponent<NavMeshAgent>().SetDestination(hit.point);
                    clickSound.Play();
                }
            }

        }
        if ((Input.GetMouseButtonDown(0))) // Selecting by mouse
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit) && hit.transform.CompareTag("NPC"))
            {
                selectedObject = hit.collider.gameObject;
                selectedObject.GetComponent<HumanController>().isSelected = true;
                isSelected = false;
                Debug.Log("NPC selected - " + hit.collider.gameObject.name);
            }
            else if (Physics.Raycast(ray, out hit) && !hit.transform.CompareTag("NPC"))
            {
                isSelected = true;
                selectedObject = null;
                Debug.Log("Main player selected");
            }
        }



    }



    private void OnTriggerExit(Collider other)
    {

        loadSlider.gameObject.SetActive(false);

        loadSlider.value = loadSlider.minValue;
        coroutine = playerLoading(loadTime);
        StopCoroutine(coroutine);
        
        if (other.gameObject.name == "Computer")
        {
            topCamera = GameObject.Find("Top Camera").GetComponent<Camera>();
            topCamera.enabled = false;
            
            foreach (GameObject targetRack in GameManager.instance.storedRacks)
            {
               targetRack.transform.GetComponentInChildren<MeshRenderer>().material = mainMaterial;
            }
           cam = Camera.main;
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.transform.GetComponent<StorageUnit>() && other.gameObject.transform.GetComponent<StorageUnit>().isSelected)
        {
            if (other.gameObject.CompareTag("Resources")) // taking box from Resoure palette
            {
                if (other.gameObject.transform.GetComponent<StorageUnit>().isSelected & !hasBox & loadSlider.value == loadSlider.maxValue)
                {
                    TakingBox(other);
                    
                    GameManager.instance.emptyResources.Add(other.gameObject);
                    GameManager.instance.storedResources.Remove(other.gameObject);

                    if (GameManager.instance.missionNumber == 1)
                    {
                        task1Toggle.GetComponent<Toggle>().isOn = true;
                    }
                }


            }
            else if (other.gameObject.CompareTag("Rack"))
            {
                if (!other.gameObject.transform.GetChild(0).gameObject.activeSelf & hasBox & loadSlider.value == loadSlider.maxValue) // putting box on the rack
                {

                    PuttingBox(other);
                    loadSlider.value = 0;
                    ChangeSliderColour(0);

                    GameManager.instance.storedRacks.Add(other.gameObject);
                    GameManager.instance.emptyRacks.Remove(other.gameObject);

                }
                else if (other.gameObject.transform.GetChild(0).gameObject.activeSelf & !hasBox & loadSlider.value == loadSlider.maxValue) // taking box from Rack
                {
                    TakingBox(other);
                                        
                    if(GameManager.instance.missionNumber == 1)
                    {
                        task1Toggle.GetComponent<Toggle>().isOn = true;
                    }
                    
                    GameManager.instance.storedRacks.Remove(other.gameObject);
                    GameManager.instance.emptyRacks.Add(other.gameObject);

                }

            }
            else if (other.gameObject.CompareTag("Target"))
            {
                if (hasBox & loadSlider.value == loadSlider.maxValue)
                {
                    PuttingBox(other);
                    other.transform.GetChild(1).GetComponent<MeshRenderer>().material = mainMaterial;
                    
                    GameManager.instance.targetRacks.Remove(other.gameObject);
                    GameManager.instance.storedTargetRacks.Add(other.gameObject);
                    GameManager.instance.storedRacks.Add(other.gameObject);

                    if (GameManager.instance.missionNumber == 1)
                    {
                        task2Toggle = GameObject.Find("Task2Toggle");
                        task2Toggle.GetComponent<Toggle>().isOn = true;
                        AltCheckTast1Condition();
                    }
                    else if (GameManager.instance.missionNumber == 2)

                    {
                        boxHasSet++;
                        GameManager.instance.taskBar.transform.GetChild(0).gameObject.transform.Find("Label").GetComponent<Text>().text = boxHasSet.ToString() + " of 4 box has set";
                        CheckTask2Condition();
                    }

                    loadSlider.value = 0;
                    ChangeSliderColour(0);
                }
                else if (other.gameObject.GetComponent<StorageUnit>().isStored & !hasBox & loadSlider.value == loadSlider.maxValue) // taking box from Target Rack
                {
                    TakingBox(other);
                    other.transform.GetComponentInChildren<MeshRenderer>().material = GameManager.instance.gameTaskMat;

                    
                    

                    GameManager.instance.storedTargetRacks.Remove(other.gameObject);
                    GameManager.instance.targetRacks.Add(other.gameObject);
                }

            }
            else if (other.gameObject.CompareTag("Track") && hasBox)
            {
                if (loadSlider.value == loadSlider.maxValue && objectType == other.gameObject.transform.GetComponent<StorageUnit>().objectType) // Right Track
                {
                    PuttingBox(other);

                    int pointsToAdd = 100 / 20 * other.gameObject.GetComponent<StorageUnit>().waitPoints;
                    other.gameObject.GetComponent<StorageUnit>().moneySprite.GetComponent<TextMeshPro>().text = pointsToAdd.ToString() + " POINTS";
                    other.gameObject.GetComponentInChildren<Animator>().SetTrigger("Start");
                    
                    AddPoints(pointsToAdd);
                    other.gameObject.GetComponent<StorageUnit>().waitPoints = other.gameObject.GetComponent<StorageUnit>().waitPoints + 20; //Add more waitpoints 
                    hasBox = false;


                }
                else if(loadSlider.value == loadSlider.maxValue && objectType != other.gameObject.transform.GetComponent<StorageUnit>().objectType) // Wrong Track
                {
                    audioEnvironment.gameObject.transform.Find("MistakeAudio").GetComponent<AudioSource>().Play();
                    AddPoints(-50);
                    hasBox = false;
                    gameObject.transform.GetChild(1).gameObject.SetActive(false);

                }

            }
        }
        else { }
        
    }
    private void OnTriggerEnter(Collider other) // Start loading to put/take box but without taking
    {
        
        loadSlider.value = loadSlider.minValue;
        task1Toggle = GameObject.Find("Task1Toggle");
        task2Toggle = GameObject.Find("Task2Toggle");

        if (other.gameObject.CompareTag("Resources") && other.gameObject == targetObject && other.gameObject.GetComponent<StorageUnit>().isStored & !hasBox)
        {
            coroutine = playerLoading(loadTime);
            StartCoroutine(coroutine);

        }

        else if (other.gameObject.CompareTag("Rack") && other.gameObject == targetObject)
        {

            if (hasBox & !other.gameObject.transform.GetChild(0).gameObject.activeSelf)
            {
                coroutine = playerLoading(loadTime);
                StartCoroutine(coroutine);

            }
            else if (other.gameObject.transform.GetChild(0).gameObject.activeSelf & !hasBox)
            {
                coroutine = playerLoading(loadTime);
                StartCoroutine(coroutine);

            }

        }
        else if (other.gameObject.CompareTag("Target") && other.gameObject == targetObject)
        {
            if (other.gameObject.transform.GetChild(0).gameObject.activeSelf & !hasBox)
            {
                coroutine = playerLoading(loadTime);
                StartCoroutine(coroutine);
            }
            else if (!other.gameObject.transform.GetChild(0).gameObject.activeSelf & hasBox)
            {
                coroutine = playerLoading(loadTime);
                StartCoroutine(coroutine);
            }

        }
        else if (other.gameObject.name == "Computer") // Enter the computer
        {

            topCamera = GameObject.Find("Top Camera").GetComponent<Camera>();
            topCamera.enabled = true;
            cam = topCamera;

            foreach (GameObject racks in GameManager.instance.storedRacks)
            {
                previousMaterials.Add(racks.transform.GetComponentInChildren<MeshRenderer>().material);

                racks.transform.GetComponentInChildren<MeshRenderer>().material = typeMaterials[racks.GetComponent<StorageUnit>().objectType];

            }
            
            /*
            previousMaterial = GameManager.instance.targetRacks[0].transform.GetComponentInChildren<MeshRenderer>().material;
            GameManager.instance.targetRacks[0].transform.GetComponentInChildren<MeshRenderer>().material = GameManager.instance.secondMat;
            if (GameManager.instance.missionNumber == 2)
            {
                GameManager.instance.targetRacks[0].transform.GetComponentInChildren<MeshRenderer>().material = GameManager.instance.secondMat;
            }
            */
        }
        else if (other.gameObject.name == "Boss" & GameManager.instance.missionNumber ==3 &!inMenu)
        {
            CheckTask3Condition();
        }
        else if (other.gameObject.CompareTag("Track") && other.gameObject == targetObject && GameManager.instance.missionNumber >= 3) // Start loading with tracks // Unlock after mission 3
        {
            
            if (hasBox)
            {
                coroutine = playerLoading(loadTime);
                StartCoroutine(coroutine);
            }
        }

    } // Starting loading after colliding 




    IEnumerator playerLoading(float loadTime)
    {
        loadSlider.value = loadSlider.minValue;
        loadSlider.gameObject.SetActive(true);
        
        loadSlider.transform.rotation = new Quaternion(gameObject.transform.rotation.x, -gameObject.transform.rotation.y, gameObject.transform.rotation.z, 0f);

        while (loadSlider.value < loadSlider.maxValue)
        {
            yield return new WaitForSeconds(loadTime);
            loadSlider.value++;

        }
    }
    void ChangeSliderColour(int colorIndex) // 0 - set backgroud to grey, 1 - to yellow.
    {
        Color grey = new Color(0.22f, 0.22f, 0.22f);
        Color yellow = new Color(0.78f, 0.56f, 0.17f);
        if (colorIndex == 0)
        {
            GameObject background = GameObject.Find("Player/Canvas/Slider/Background");
            background.GetComponent<Image>().color = grey;
            GameObject fill = GameObject.Find("Player/Canvas/Slider/Fill Area/Fill");
            fill.GetComponent<Image>().color = yellow;
        }
        if (colorIndex == 1)
        {
            GameObject background = GameObject.Find("Player/Canvas/Slider/Background");
            background.GetComponent<Image>().color = yellow;
            GameObject fill = GameObject.Find("Player/Canvas/Slider/Fill Area/Fill");
            fill.GetComponent<Image>().color = grey;

        }

    }
    void TakingBox(Collider other)
    {
        other.gameObject.transform.GetChild(0).gameObject.SetActive(false);
        other.gameObject.GetComponent<StorageUnit>().isStored = false;
        objectType = other.GetComponent<StorageUnit>().objectType;
        hasBox = true;
        gameObject.transform.GetChild(1).gameObject.SetActive(true);

        loadSlider.value = 0;
        ChangeSliderColour(1);
        audioEnvironment.gameObject.transform.Find("TakeBoxAudio").GetComponent<AudioSource>().Play();
    }
    void PuttingBox(Collider other)
    {
        if(other.gameObject.transform.GetChild(0) != null)
        {
            other.gameObject.transform.GetChild(0).gameObject.SetActive(true);
        }
        audioEnvironment.gameObject.transform.Find("PutBoxAudio").GetComponent<AudioSource>().Play();
        other.gameObject.GetComponent<StorageUnit>().isStored = true;
        other.GetComponent<StorageUnit>().objectType = objectType;

        hasBox = false;
        gameObject.transform.GetChild(1).gameObject.SetActive(false);

        loadSlider.value = 0;
        ChangeSliderColour(0);
    }

    void AltCheckTast1Condition()
    {
        if (GameManager.instance.storedTargetRacks.Count == 1 && GameManager.instance.missionNumber == 1)
        {
            // ClearTargetRack();

            mainText.gameObject.GetComponent<TMP_Text>().text = "NICE JOB, " + playerName + "! CONGRATULATION! \n Your next tastk - 4 boxes.";
            mainText.gameObject.GetComponent<Animator>().SetTrigger("Open");
            //mainText.gameObject.SetActive(true);
            inMenu = true;
            audioEnvironment.gameObject.transform.Find("MissionDoneAudio").GetComponent<AudioSource>().Play();
            AddMoney(moneyFlow);


            taskbar.GetComponent<Animator>().SetTrigger("CloseTaskBar");
            GameManager.instance.missionNumber++;
            
        }
    }
    void CheckTask2Condition()
    {
        if (GameManager.instance.storedTargetRacks.Count == 4 && GameManager.instance.missionNumber == 2)
        {

            inMenu = true;
            audioEnvironment.gameObject.transform.Find("MissionDoneAudio").GetComponent<AudioSource>().Play();
            mainText.gameObject.GetComponent<TMP_Text>().text = "Excellent!, " + playerName + "! \n Looks like your manager has task for you. \n Ask him what he need";
            mainText.gameObject.GetComponent<Animator>().SetTrigger("Open");
            taskbar.transform.GetChild(0).GetComponent<Toggle>().isOn = false;
            taskbar.GetComponent<Animator>().SetTrigger("CloseTaskBar");
            GameManager.instance.missionNumber++;
           
            AddMoney(moneyFlow);
        }
    }
    void CheckTask3Condition()
    {
       

        if (hasBox && objectType == 0 & inMenu == false)
        {

            GenerateTextInMainTextBox("You brought it! Thanks! And now..... COME BACK TO WORK!!!! \n There are 3 cars waiting for load.\n Load with right Color! NOW!!!!");
            
            GameManager.instance.missionNumber++;
            inMenu = true;
            audioEnvironment.gameObject.transform.Find("MissionDoneAudio").GetComponent<AudioSource>().Play();
            
            GameObject.Find("Boss").GetComponent<BossController>().BossExit();
            hasBox = false;
            taskbar.GetComponent<Animator>().SetTrigger("CloseTaskBar");
            AddMoney(moneyFlow);
        }
        if (hasBox && objectType > 0)
        {
            GenerateTextInMainTextBox("Wrong one! I need blue one. Put this one back");
            
        }
        if (!hasBox & GameManager.instance.missionNumber == 3)
        {
            GenerateTextInMainTextBox("Bring me blue box. Check in your computer");
            

            taskbar.transform.GetChild(0).GetComponent<Toggle>().isOn = true;
            taskbar.transform.GetChild(1).gameObject.transform.Find("Label").GetComponent<Text>().text = "Bring blue box to your boss";
        }


    }
    
    void ResetAndClearTarget()
    {
        targetObject = null;
        if (previousTarget != null)
        {
            previousTarget.transform.gameObject.GetComponentInParent<StorageUnit>().isSelected = false;
        }
        
    }
    void GenerateTextInMainTextBox(string text)
    {
        mainText.gameObject.GetComponent<TMP_Text>().text = text;
        mainText.gameObject.GetComponent<Animator>().SetTrigger("Open");
        inMenu = true;
    }
    public void LeftMenu()
    {
        inMenu = false;
    }
    public void AddMoney(int moneyFlow)
    {
        money = money + moneyFlow;
        GameObject.Find("Money").GetComponent<TextMeshProUGUI>().text = "Money: " + money;
        audioEnvironment.gameObject.transform.Find("MoneyAudio").GetComponent<AudioSource>().Play();
    }
    void AddPoints(int points)
    {
        GameManager.instance.currentPoint += points;
        GameManager.instance.ChangeLevelPoints();
    }
    
}
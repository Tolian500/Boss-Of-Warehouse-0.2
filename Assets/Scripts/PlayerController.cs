using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using TMPro;


public class PlayerController : HumanController
{
    [SerializeField] GameObject selectedCharacter;

    public static PlayerController instance;
    [SerializeField] private GameObject box1;

    //public bool hasBox;
    //[SerializeField] int objectType;
    public Canvas canvas;
    //public Material mainMaterial;
    //[SerializeField] private Material previousMaterial;
    public GameObject task1Toggle;
    public GameObject task2Toggle;
    [SerializeField] TextMeshProUGUI mainText;
    private GameObject taskbar;

    //[SerializeField] float loadTime;
    //private IEnumerator coroutine;
    public bool inMenu;
    //[SerializeField] AudioSource clickSound;
    //private GameObject audioEnvironment;
    [SerializeField] string playerName;
    int boxHasSet;

    // [SerializeField] GameObject selectedObject;
    //[SerializeField] GameObject targetObject;
    //private GameObject previousTarget;

    private Camera topCamera;
    [SerializeField] GameObject selectIndicator;
    [SerializeField] GameObject characterIndicator;



    //[SerializeField] private List<Material> previousMaterials;
    //[SerializeField] private Material[] typeMaterials;

    public int money;
    [SerializeField] int moneyFlow;



    private void Start()
    {
        instance = this;
        hasBox = false;


        isSelected = true;
        selectedCharacter = gameObject;

        clickSound = audioEnvironment.gameObject.transform.Find("ClickAudio").GetComponent<AudioSource>();



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
        // Selecting by keys
        if (Input.GetKeyDown("1"))
        {
            if (selectedCharacter.GetComponent<HumanController>().characterindicatorButton != null) //Deselecting all buttuns of players indicators 
            {
                selectedCharacter.GetComponent<HumanController>().characterindicatorButton.interactable = true;
            }
            selectedCharacter = gameObject;
            isSelected = true;
            selectedObject = null;
            Debug.Log("Main player selected");
            if (selectedCharacter.GetComponent<HumanController>().characterindicatorButton != null)
            {
                selectedCharacter.GetComponent<HumanController>().characterindicatorButton.interactable = false;
            } // Selecting buttun of player indicator if exist
            characterIndicator.transform.position = selectedCharacter.transform.position + new Vector3(0, 6, 0); // Set character indicator + hight offset 6m
            characterIndicator.gameObject.transform.SetParent(selectedCharacter.transform);
        }
        if (Input.GetKeyDown("2"))
        {
            GameObject selectedworker = GameObject.Find("Worker1");
            if (selectedworker != null)
            {
                if (selectedCharacter.GetComponent<HumanController>().characterindicatorButton != null) //Deselecting all buttuns of players indicators 
                {
                    selectedCharacter.GetComponent<HumanController>().characterindicatorButton.interactable = true;
                }
                selectedObject = selectedworker;
                selectedObject.GetComponent<HumanController>().isSelected = true;
                isSelected = false;
                Debug.Log("NPC selected - " + selectedworker.name);
                selectedCharacter = selectedworker;
                if (selectedCharacter.GetComponent<HumanController>().characterindicatorButton != null)
                {
                    selectedCharacter.GetComponent<HumanController>().characterindicatorButton.interactable = false;
                } // Selecting buttun of player indicator if exist
                characterIndicator.transform.position = selectedCharacter.transform.position + new Vector3(0, 6, 0); // Set character indicator + hight offset 6m
                characterIndicator.gameObject.transform.SetParent(selectedCharacter.transform);
            }
        }
        if (Input.GetKeyDown("3"))
        {
            GameObject selectedworker = GameObject.Find("Worker2");
            if (selectedworker != null)
            {
                if (selectedCharacter.GetComponent<HumanController>().characterindicatorButton != null) //Deselecting all buttuns of players indicators 
                {
                    selectedCharacter.GetComponent<HumanController>().characterindicatorButton.interactable = true;
                }
                selectedObject = selectedworker;
                selectedObject.GetComponent<HumanController>().isSelected = true;
                isSelected = false;
                Debug.Log("NPC selected - " + selectedworker.name);
                selectedCharacter = selectedworker;
                if (selectedCharacter.GetComponent<HumanController>().characterindicatorButton != null)
                {
                    selectedCharacter.GetComponent<HumanController>().characterindicatorButton.interactable = false;
                } // Selecting buttun of player indicator if exist
                characterIndicator.transform.position = selectedCharacter.transform.position + new Vector3(0, 6, 0); // Set character indicator + hight offset 6m
                characterIndicator.gameObject.transform.SetParent(selectedCharacter.transform);
            }
        }
        if (Input.GetKeyDown("4"))
        {
            GameObject selectedworker = GameObject.Find("Worker3");
            if (selectedworker != null)
            {
                if (selectedCharacter.GetComponent<HumanController>().characterindicatorButton != null) //Deselecting all buttuns of players indicators 
                {
                    selectedCharacter.GetComponent<HumanController>().characterindicatorButton.interactable = true;
                }
                selectedObject = selectedworker;
                selectedObject.GetComponent<HumanController>().isSelected = true;
                isSelected = false;
                Debug.Log("NPC selected - " + selectedworker.name);
                selectedCharacter = selectedworker;
                if (selectedCharacter.GetComponent<HumanController>().characterindicatorButton != null)
                {
                    selectedCharacter.GetComponent<HumanController>().characterindicatorButton.interactable = false;
                } // Selecting buttun of player indicator if exist
                characterIndicator.transform.position = selectedCharacter.transform.position + new Vector3(0, 6, 0); // Set character indicator + hight offset 6m
                characterIndicator.gameObject.transform.SetParent(selectedCharacter.transform);
            }
        }


        // Selecting by Mouse
        if (Input.GetMouseButtonDown(0)) // Left mouse - Selecting by mouse
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;



            if (Physics.Raycast(ray, out hit) && hit.transform.CompareTag("NPC")) // Selecting  NPC
            {
                if (selectedCharacter.GetComponent<HumanController>().characterindicatorButton != null) //Deselecting all buttuns of players indicators 
                {
                    selectedCharacter.GetComponent<HumanController>().characterindicatorButton.interactable = true;
                }
                selectedObject = hit.collider.gameObject;
                selectedObject.GetComponent<HumanController>().isSelected = true;
                isSelected = false;
                Debug.Log("NPC selected - " + hit.collider.gameObject.name);
                selectedCharacter = hit.collider.gameObject;


            }
            else if (Physics.Raycast(ray, out hit) && !hit.transform.CompareTag("NPC")) // Selecting main Player
            {
                if (selectedCharacter.GetComponent<HumanController>().characterindicatorButton != null) //Deselecting all buttuns of players indicators 
                {
                    selectedCharacter.GetComponent<HumanController>().characterindicatorButton.interactable = true;
                }
                selectedCharacter = gameObject;
                isSelected = true;
                selectedObject = null;
                Debug.Log("Main player selected");
            }
            if (selectedCharacter.GetComponent<HumanController>().characterindicatorButton != null)
            {
                selectedCharacter.GetComponent<HumanController>().characterindicatorButton.interactable = false;
            } // Selecting buttun of player indicator if exist
            characterIndicator.transform.position = selectedCharacter.transform.position + new Vector3(0, 6, 0); // Set character indicator + hight offset 6m
            characterIndicator.gameObject.transform.SetParent(selectedCharacter.transform);
        }

        if (Input.GetMouseButtonDown(1) && !inMenu)
        {
            //ResetAndClearTarget();
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {

                if (hit.transform.gameObject.GetComponent<StorageUnit>() && selectedCharacter.GetComponent<HumanController>().agent.enabled == true) // move to object
                {
                    selectedCharacter.GetComponent<HumanController>().agent.SetDestination(hit.transform.position);
                    hit.transform.gameObject.GetComponentInParent<StorageUnit>().isSelected = true;

                    if (hit.transform.gameObject.CompareTag("Track")) // move to truck - Slided Truck direction
                    {
                        selectedCharacter.GetComponent<HumanController>().agent.SetDestination(hit.transform.position + new Vector3(3.7f, 0));
                    }
                    else if (hit.transform.gameObject.CompareTag("Resources"))
                    {
                        selectedCharacter.GetComponent<HumanController>().agent.SetDestination(hit.transform.position + new Vector3(-0.7f, 0));
                    }

                    selectedCharacter.GetComponent<HumanController>().targetObject = hit.transform.gameObject;
                    selectedCharacter.GetComponent<HumanController>().previousTarget = targetObject;
                    selectIndicator.transform.position = hit.transform.position + new Vector3(0, 3);
                }

                else if (selectedCharacter.GetComponent<HumanController>().agent.enabled == true) // move to point
                {


                    selectedCharacter.GetComponent<HumanController>().agent.SetDestination(hit.point);
                    selectIndicator.transform.position = hit.point;
                    //targetObject = null;



                }
                clickSound.Play();
            }
        } // right mouse - navigating


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


    void GenerateTextInMainTextBox(string text)
    {
        mainText.gameObject.GetComponent<TMP_Text>().text = text;
        mainText.gameObject.GetComponent<Animator>().SetTrigger("Open");
        inMenu = true;
    }
    public void EnterMenu()
    {
        inMenu = true;
        Camera.main.GetComponent<CameraControl>().movingLocked = true;
    }
    public void LeftMenu()
    {
        inMenu = false;
        Camera.main.GetComponent<CameraControl>().movingLocked = false;
    }
    public void AddMoney(int moneyFlow)
    {
        money += moneyFlow;
        GameObject.Find("Money").GetComponent<TextMeshProUGUI>().text = "Money: " + money;
        //audioEnvironment.gameObject.transform.Find("MoneyAudio").GetComponent<AudioSource>().Play();
    }
    public void RemoveMoney(int moneyFlow)
    {
        money -= moneyFlow;
        GameObject.Find("Money").GetComponent<TextMeshProUGUI>().text = "Money: " + money;
    }
    public void CheckMoney()
    {
        GameObject.Find("Money").GetComponent<TextMeshProUGUI>().text = "Money: " + money;
    }
    public void SelectPlayerOutside(string selectedPlayer)
    {

        selectedCharacter = GameObject.Find(selectedPlayer);
        characterIndicator.transform.position = selectedCharacter.transform.position + new Vector3(0, 6, 0); // Set character indicator + hight offset 6m
        characterIndicator.gameObject.transform.SetParent(selectedCharacter.transform);


    }


}
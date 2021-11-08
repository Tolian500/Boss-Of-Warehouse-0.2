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

    [SerializeField] float loadTime;
    private IEnumerator coroutine;
    public bool inMenu;
    [SerializeField] AudioSource clickSound;
    [SerializeField] GameObject audioEnvironment;
    [SerializeField] string playerName;
    int boxHasSet;
    [SerializeField] GameObject selectedObject;

    private Camera topCamera;

    [SerializeField] private List<Material> previousMaterials;

    [SerializeField] private Material[] typeMaterials;





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
        loadSlider = gameObject.transform.GetChild(1).transform.GetChild(0).GetComponent<Slider>();

    }


    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(1) & !inMenu & isSelected)
        {

            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit)) // Navigatin main player
            {
                Debug.Log(hit.transform.position);
                if (hit.transform.gameObject.CompareTag("Rack"))
                {
                    agent.SetDestination(hit.transform.position);
                    clickSound.Play();
                    Debug.Log("Rack was hited = " + hit.transform.position);
                }
                else
                {
                    agent.SetDestination(hit.point);
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
            }
            else if (Physics.Raycast(ray, out hit) && !hit.transform.CompareTag("NPC"))
            {
                isSelected = true;
                selectedObject = null;
            }
        }



    }



    private void OnTriggerExit(Collider other)
    {

        loadSlider.gameObject.SetActive(false);
        loadSlider.transform.rotation = gameObject.transform.rotation;

        coroutine = playerLoading(loadTime);
        StopCoroutine(coroutine);
        loadSlider.value = loadSlider.minValue;
        if (other.gameObject.name == "Computer")
        {
            topCamera = GameObject.Find("Top Camera").GetComponent<Camera>();
            topCamera.enabled = false;

            foreach (GameObject targetRack in GameManager.instance.storedRacks)
            {
                
                targetRack.transform.GetComponentInChildren<MeshRenderer>().material = mainMaterial;
            }

            GameManager.instance.targetRacks[0].transform.GetComponentInChildren<MeshRenderer>().material = previousMaterial;
            if (GameManager.instance.missionNumber == 2)
            {
                GameManager.instance.targetRacks[0].transform.GetComponentInChildren<MeshRenderer>().material = previousMaterial;
            }
            cam = Camera.main;
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Resources")) // taking box from Resoure palette
        {
            if (other.gameObject.transform.GetChild(0).gameObject.activeSelf & !hasBox & loadSlider.value == loadSlider.maxValue)
            {
                TakingBox(other);
                audioEnvironment.gameObject.transform.Find("TakeBoxAudio").GetComponent<AudioSource>().Play();
                loadSlider.value = 0;
                ChangeSliderColour(1);

                if (GameManager.instance.missionNumber == 0)
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
                audioEnvironment.gameObject.transform.Find("PutBoxAudio").GetComponent<AudioSource>().Play();


                loadSlider.value = 0;
                ChangeSliderColour(0);

                GameManager.instance.storedRacks.Add(other.gameObject);
                GameManager.instance.emptyRacks.Remove(other.gameObject);

            }
            else if (other.gameObject.transform.GetChild(0).gameObject.activeSelf & !hasBox & loadSlider.value == loadSlider.maxValue) // taking box from Rack
            {
                TakingBox(other);

                audioEnvironment.gameObject.transform.Find("TakeBoxAudio").GetComponent<AudioSource>().Play();
                task1Toggle.GetComponent<Toggle>().isOn = true;
                loadSlider.value = 0;
                ChangeSliderColour(1);

                GameManager.instance.storedRacks.Remove(other.gameObject);
                GameManager.instance.emptyRacks.Add(other.gameObject);

            }

        }

        else if (other.gameObject.CompareTag("Target"))
        {
            if (hasBox & loadSlider.value == loadSlider.maxValue)
            {
                PuttingBox(other);

                audioEnvironment.gameObject.transform.Find("PutBoxAudio").GetComponent<AudioSource>().Play();
                //other.transform.GetComponentInChildren<MeshRenderer>().material = mainMaterial;
                other.transform.GetChild(1).GetComponent<MeshRenderer>().material = mainMaterial;
                //other.transform.tag = "Rack";
                GameManager.instance.targetRacks.Remove(other.gameObject);
                GameManager.instance.storedTargetRacks.Add(other.gameObject);

                if (GameManager.instance.missionNumber == 0)
                {
                    task2Toggle = GameObject.Find("Task2Toggle");
                    task2Toggle.GetComponent<Toggle>().isOn = true;
                    AltCheckTast1Condition();
                }
                else if (GameManager.instance.missionNumber == 1)

                {
                    boxHasSet++;
                    GameManager.instance.taskBar.transform.GetChild(0).gameObject.transform.Find("Label").GetComponent<Text>().text = boxHasSet.ToString() + " of 4 box has set";
                    CheckTask2Condition();
                }

                loadSlider.value = 0;
                ChangeSliderColour(0);
            }
            else if (other.gameObject.GetComponent<StorageUnit>().isStored & !hasBox & loadSlider.value == loadSlider.maxValue) // taking box from Rack
            {
                TakingBox(other);
                other.transform.GetComponentInChildren<MeshRenderer>().material = GameManager.instance.gameTaskMat;

                audioEnvironment.gameObject.transform.Find("TakeBoxAudio").GetComponent<AudioSource>().Play();
                loadSlider.value = 0;
                ChangeSliderColour(1);

                GameManager.instance.storedTargetRacks.Remove(other.gameObject);
                GameManager.instance.targetRacks.Add(other.gameObject);
            }

        }
    }
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("OnTriggerEnter");
        loadSlider.value = loadSlider.minValue;
        loadSlider.gameObject.SetActive(true);
        loadSlider.transform.rotation = new Quaternion(gameObject.transform.rotation.x, -gameObject.transform.rotation.y, gameObject.transform.rotation.z, 0f);

        task1Toggle = GameObject.Find("Task1Toggle");
        task2Toggle = GameObject.Find("Task2Toggle");

        if (other.gameObject.CompareTag("Resources"))
        {
            if (other.gameObject.transform.GetChild(0).gameObject.activeSelf & !hasBox)
            {
                coroutine = playerLoading(loadTime);
                StartCoroutine(coroutine);
            }


        }

        else if (other.gameObject.CompareTag("Rack"))
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
        else if (other.gameObject.CompareTag("Target"))
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
            foreach (GameObject targetRack in GameManager.instance.storedRacks)
            {
                previousMaterials.Add(targetRack.transform.GetComponentInChildren<MeshRenderer>().material);

                targetRack.transform.GetComponentInChildren<MeshRenderer>().material = typeMaterials[targetRack.GetComponent<StorageUnit>().objectType];

            }
            previousMaterial = GameManager.instance.targetRacks[0].transform.GetComponentInChildren<MeshRenderer>().material;
            GameManager.instance.targetRacks[0].transform.GetComponentInChildren<MeshRenderer>().material = GameManager.instance.secondMat;
            if (GameManager.instance.missionNumber == 2)
            {
                GameManager.instance.targetRacks[0].transform.GetComponentInChildren<MeshRenderer>().material = GameManager.instance.secondMat;
            }
        }
        else if (other.gameObject.name == "Boss")
        {
            CheckTask3Condition();
        }

    } // Starting loading after colliding 




    IEnumerator playerLoading(float loadTime)
    {
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
    }
    void PuttingBox(Collider other)
    {
        other.gameObject.transform.GetChild(0).gameObject.SetActive(true);
        other.gameObject.GetComponent<StorageUnit>().isStored = true;
        other.GetComponent<StorageUnit>().objectType = objectType;
        hasBox = false;
    }

    void AltCheckTast1Condition()
    {
        if (GameManager.instance.storedTargetRacks.Count == 1 && GameManager.instance.missionNumber == 0)
        {
            // ClearTargetRack();

            mainText.gameObject.GetComponent<TMP_Text>().text = "NICE JOB, " + playerName + "! CONGRATULATION! \n Your next tastk - 4 boxes.";
            mainText.gameObject.GetComponent<Animator>().SetTrigger("Open");
            //mainText.gameObject.SetActive(true);
            inMenu = true;
            audioEnvironment.gameObject.transform.Find("MissionDoneAudio").GetComponent<AudioSource>().Play();

            GameObject taskbar = GameObject.Find("Mission1Panel");
            taskbar.GetComponent<Animator>().SetTrigger("CloseTaskBar");
            GameManager.instance.missionNumber++;
            Debug.Log("Mission number: " + GameManager.instance.missionNumber);
        }
    }
    void CheckTask2Condition()
    {
        if (GameManager.instance.storedTargetRacks.Count == 4 && GameManager.instance.missionNumber == 1)
        {

            inMenu = true;
            audioEnvironment.gameObject.transform.Find("MissionDoneAudio").GetComponent<AudioSource>().Play();
            mainText.gameObject.GetComponent<TMP_Text>().text = "Excellent!, " + playerName + "! \n There no more missions yet. \n Thx for testing the game";
            mainText.gameObject.GetComponent<Animator>().SetTrigger("Open");
            GameObject taskbar = GameObject.Find("Mission1Panel");
            taskbar.GetComponent<Animator>().SetTrigger("CloseTaskBar");
            GameManager.instance.missionNumber++;
            Debug.Log("Mission number: " + GameManager.instance.missionNumber);
        }
    }
    void CheckTask3Condition()
    {
        if(hasBox&& objectType == 0)
        {
            Debug.Log("You brought it! Thx! And now come back to work!!!!");
            GameManager.instance.missionNumber++;
        }
        if(hasBox && objectType > 0)
        {
            Debug.Log("Wrong one! I need blue blue");
        }
        if (!hasBox)
        {
            Debug.Log("Bring me blue box. Check in your computer");
        }


    }
}
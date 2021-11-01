using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using TMPro;


public class PlayerController : MonoBehaviour
{
    public static PlayerController   instance;
    public Camera cam;
    [SerializeField] private GameObject box1;

    public NavMeshAgent agent;
    public bool hasBox;
    public Canvas canvas;
    public Material mainMaterial;
    public GameObject task1Toggle;
    public GameObject task2Toggle;
    [SerializeField] TextMeshProUGUI mainText;
    [SerializeField] Slider loadSlider;
    [SerializeField] float loadTime;
    private IEnumerator coroutine;
    public bool inMenu;
    [SerializeField] AudioSource clickSound;
    [SerializeField] GameObject audioEnvironment;
    [SerializeField] string playerName;
    int boxHasSet;

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
        
           
        
    }


    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) & !inMenu)
        {

            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {

                agent.SetDestination(hit.point);
                clickSound.Play();
            }
        }



    }
    void CheckTask1Condition()
    {
        if (task1Toggle.GetComponent<Toggle>().isOn && task2Toggle.GetComponent<Toggle>().isOn && GameManager.instance.missionNumber == 0)
        {
            mainText.gameObject.GetComponent<TMP_Text>().text = "NICE JOB, " + playerName+"! CONGRATULATION! \n Your next tastk - 4 boxes.";
            mainText.gameObject.GetComponent<Animator>().SetTrigger("Open");
            //mainText.gameObject.SetActive(true);
            inMenu = true;
            audioEnvironment.gameObject.transform.Find("MissionDoneAudio").GetComponent<AudioSource>().Play();

            GameObject taskbar = GameObject.Find("Mission1Panel");
            taskbar.GetComponent<Animator>().SetTrigger("CloseTaskBar");
            GameManager.instance.missionNumber++;
            Debug.Log("Mission number: "+GameManager.instance.missionNumber);
        }
    }
    void CheckTask2Condition()
    {
        if (boxHasSet == 4)
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
    
    private void OnTriggerExit(Collider other)
    {
        
        loadSlider.gameObject.SetActive(false);
        loadSlider.transform.rotation = gameObject.transform.rotation;

        coroutine = playerLoading(loadTime);
        StopCoroutine(coroutine);
        loadSlider.value = loadSlider.minValue;
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Resources")) // taking box from Resoure palette
        {
            if (other.gameObject.transform.GetChild(1).gameObject.activeSelf & !hasBox & loadSlider.value == loadSlider.maxValue)
            {
                other.gameObject.transform.GetChild(1).gameObject.SetActive(false);
                hasBox = true;

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
                
                other.gameObject.transform.GetChild(0).gameObject.SetActive(true);

                audioEnvironment.gameObject.transform.Find("PutBoxAudio").GetComponent<AudioSource>().Play();
                hasBox = false;
                loadSlider.value = 0;
                ChangeSliderColour(0);
            }
            else if (other.gameObject.transform.GetChild(0).gameObject.activeSelf & !hasBox & loadSlider.value == loadSlider.maxValue) // taking box from Rack
            {
                other.gameObject.transform.GetChild(0).gameObject.SetActive(false);
                hasBox = true;

                audioEnvironment.gameObject.transform.Find("TakeBoxAudio").GetComponent<AudioSource>().Play();
                task1Toggle.GetComponent<Toggle>().isOn = true;
                loadSlider.value = 0;
                ChangeSliderColour(1);
            }

        }

        else if (other.gameObject.CompareTag("Target"))
        {
            if (hasBox & loadSlider.value == loadSlider.maxValue)
            {
                Debug.Log("targetRack TriggerEnter");

                other.gameObject.transform.GetChild(0).gameObject.SetActive(true);
                hasBox = false;

                audioEnvironment.gameObject.transform.Find("PutBoxAudio").GetComponent<AudioSource>().Play();
                other.transform.GetComponentInChildren<MeshRenderer>().material = mainMaterial;
                other.transform.tag = "Rack";
                if(GameManager.instance.missionNumber == 0)
                {
                    task2Toggle = GameObject.Find("Task2Toggle");
                    task2Toggle.GetComponent<Toggle>().isOn = true;
                    CheckTask1Condition();
                }
                else if (GameManager.instance.missionNumber == 1)
                    
                {
                    boxHasSet++;
                    GameManager.instance.taskBar.transform.GetChild(0).gameObject.transform.Find("Label").GetComponent<Text>().text = boxHasSet.ToString()+ " of 4 box has set";
                                     
                    CheckTask2Condition();
                }

                    
                loadSlider.value = 0;
                ChangeSliderColour(0);
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
            if (other.gameObject.transform.GetChild(1).gameObject.activeSelf & !hasBox)
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
        else if (other.gameObject.CompareTag("Target") & hasBox)
        {
            coroutine = playerLoading(loadTime);
            StartCoroutine(coroutine);
        }


    }
    


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

}

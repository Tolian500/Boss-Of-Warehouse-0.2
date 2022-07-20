using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using TMPro;

public class HumanController : MonoBehaviour
{

    //public static HumanController instance;
    [SerializeField] protected GameObject audioEnvironment;

    [SerializeField] protected int objectType;
    public NavMeshAgent agent;
    [SerializeField] protected Material mainMaterial; // should be problem with this
    public bool hasBox;
    //protected GameObject selectIndicator;

    public Camera cam;
    private Camera topCamera;

    public bool isSelected;
    [SerializeField] protected Slider loadSlider;
    [SerializeField] protected float loadTime;
    protected IEnumerator coroutine;

    [SerializeField] protected List<Material> previousMaterials;
    [SerializeField] protected Material[] typeMaterials;

    [SerializeField] protected GameObject selectedObject;
    [SerializeField] public GameObject targetObject;
    public GameObject previousTarget;

    [SerializeField] public Button characterindicatorButton;

    private float boxSpawnTime = 1.7f;

    protected bool lockMotion = false;

    [SerializeField] private List<AudioSource> sounds;
    protected AudioSource clickSound;
    [SerializeField] private float weightCoef = 2;







    // Start is called before the first frame update
    void Start()
    {
        //audioEnvironment = GameObject.Find("AudioEnvironment");

        cam = Camera.main;
        agent = gameObject.GetComponent<NavMeshAgent>();
        //loadSlider = gameObject.transform.GetChild(0).transform.GetChild(0).GetComponent<Slider>();

    }

    void Update()
    {

    }
    void MoveHuman()
    {
        if (Input.GetMouseButtonDown(1) & isSelected)
        {

            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit) && !hit.transform.CompareTag("NPC"))
            {
                agent.SetDestination(hit.point);

            }
        }
    }

    protected void OnTriggerExit(Collider other)
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
    protected void OnTriggerStay(Collider other)
    {
        if (other.gameObject.transform.GetComponent<StorageUnit>() && other.gameObject == targetObject)
        {

            if (other.gameObject.CompareTag("Resources") && !hasBox && other.gameObject.GetComponent<StorageUnit>().isStored && loadSlider.value == loadSlider.maxValue) // taking box from Resoure palette -- previus code --  
            {
                TakingBox(other);

                GameManager.instance.emptyResources.Add(other.gameObject);
                GameManager.instance.storedResources.Remove(other.gameObject);
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

                    int pointsToAdd = 200 * other.gameObject.GetComponent<StorageUnit>().waitPoints / other.gameObject.GetComponent<StorageUnit>().nextWaitPoints;
                    Debug.Log(pointsToAdd + " points added");
                    //other.gameObject.GetComponent<BusStorageUnit>().moneySprite.GetComponent<TextMeshPro>().text = pointsToAdd.ToString() + " POINTS";
                    other.gameObject.GetComponent<BusStorageUnit>().AddBoxToLoad();
                    other.gameObject.GetComponent<BusStorageUnit>().CheckLoad();

                    //other.gameObject.GetComponentInChildren<Animator>().SetTrigger("Start");

                    AddPoints(pointsToAdd);
                    other.gameObject.GetComponent<BusStorageUnit>().waitPoints = other.gameObject.GetComponent<StorageUnit>().waitPoints + 20; //Add more waitpoints 
                    hasBox = false;



                }
                else if (loadSlider.value == loadSlider.maxValue && objectType != other.gameObject.transform.GetComponent<StorageUnit>().objectType) // Wrong Track
                {

                    sounds[4].Play();
                    AddPoints(-50);
                    hasBox = false;
                    gameObject.transform.GetChild(1).gameObject.SetActive(false);

                }
                else if (other.gameObject.name == "Computer")
                {
                    foreach (GameObject targetRack in GameManager.instance.storedRacks)
                    {
                        targetRack.transform.GetComponentInChildren<MeshRenderer>().material = mainMaterial;
                    }
                }

            }
        }
        else { }

    }
    protected void OnTriggerEnter(Collider other) // Start loading to put/take box but without taking
    {
        if (other.gameObject == targetObject)
        {
            LoadBeforeTaking();
            Debug.Log("Start loading with " + other.gameObject.name);

        }
        /*

        if (other.gameObject.CompareTag("Resources") && other.gameObject == targetObject && other.gameObject.GetComponent<StorageUnit>().isStored && !hasBox)
        {
            Debug.Log("Start loading with " + other.gameObject.name);
            coroutine = playerLoading(loadTime);
            StartCoroutine(coroutine);
        }

        else if (other.gameObject.CompareTag("Rack") && other.gameObject == targetObject)
        {
            Debug.Log("Start loading with " + other.gameObject.name);
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
            Debug.Log("Start loading with " + other.gameObject.name);
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
        */

        else if (other.gameObject.name == "Computer") // Enter the computer
        {
            topCamera = GameObject.Find("Top Camera").GetComponent<Camera>();
            topCamera.enabled = true;
            cam = topCamera;

            foreach (GameObject racks in GameManager.instance.storedRacks)
            {
                //previousMaterials.Add(racks.transform.GetComponentInChildren<MeshRenderer>().material);
                racks.transform.GetComponentInChildren<MeshRenderer>().material = typeMaterials[racks.GetComponent<StorageUnit>().objectType];
            }
        }

        else if (other.gameObject.CompareTag("Track") && other.gameObject == targetObject && GameManager.instance.missionNumber >= 3 && hasBox) // Start loading with tracks // Unlock after mission 3
        {
            LoadBeforeTaking();
        }


    }




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
    void LoadBeforeTaking() // or putting
    {
        coroutine = playerLoading(loadTime);
        StartCoroutine(coroutine);
    }
    void TakingBox(Collider other)
    {
        hasBox = true;
        other.gameObject.transform.GetChild(0).gameObject.SetActive(false);
        other.gameObject.GetComponent<StorageUnit>().isStored = false;
        objectType = other.GetComponent<StorageUnit>().objectType;

        agent.acceleration /= weightCoef;
        agent.speed /= weightCoef;


        gameObject.transform.GetChild(1).gameObject.SetActive(true); //box indicator
        gameObject.transform.GetChild(2).gameObject.SetActive(true); //box visual

        loadSlider.value = 0;
        ChangeSliderColour(1);
        gameObject.GetComponent<AnimatorHandler>().BoxAnimating(hasBox);

    }
    void PuttingBox(Collider other)
    {
        hasBox = false;
        gameObject.GetComponent<AnimatorHandler>().BoxAnimating(hasBox);
        StartCoroutine(BoxDisapearCoroutine());

        agent.acceleration *= weightCoef;
        agent.speed *= weightCoef;

        other.gameObject.GetComponent<StorageUnit>().SelfPuttingBox();


        other.GetComponent<StorageUnit>().objectType = objectType;
        targetObject = null;




        loadSlider.value = 0;
        ChangeSliderColour(0);


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
    public void AddPoints(int points)
    {
        GameManager.instance.currentPoint += points;
        GameManager.instance.ChangeLevelPoints();
    }
    protected void ResetAndClearTarget()
    {
        targetObject = null;
        if (previousTarget != null)
        {
            previousTarget.transform.gameObject.GetComponentInParent<StorageUnit>().isSelected = false;
        }

    }
    IEnumerator BoxDisapearCoroutine()
    {

        yield return new WaitForSeconds(boxSpawnTime);
        gameObject.transform.GetChild(1).gameObject.SetActive(false); //box indicator
        gameObject.transform.GetChild(2).gameObject.SetActive(false); //box visual

    }
    public void DisableMovment()
    {
        //lockMotion = true;
        agent.enabled = false;
    }
    public void EnableMovment()
    {
        //lockMotion = false;
        agent.enabled = true;
    }
    public void PlayTakeBoxSound()
    {
        sounds[0].Play();
    }
    public void PlayPutBoxSound()
    {
        sounds[1].Play();
    }
    public void PlayStepOneSound()
    {

        sounds[2].Play();


    }
    public void PlayStepTwoSound()
    {

        sounds[3].Play();


    }



}

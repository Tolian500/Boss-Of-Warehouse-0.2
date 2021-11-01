using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject[] Racks;
    [SerializeField] private GameObject[] Resources;
    [SerializeField] private int waitTime = 1;

    public static GameManager instance;

    public GameObject taskBar;
    public Material gameTaskMat;
    public int missionNumber;
    //private IEnumerator coroutine;
    //private int maxRackNumber;
    //int currentMission = 1;
    //int maxAttempts = 0;

    // Start is called before the first frame update
    void Start()
    {

        instance = this;

        Racks = GameObject.FindGameObjectsWithTag("Rack");
        Resources = GameObject.FindGameObjectsWithTag("Resources");
        missionNumber = 0;
    }
    private void Awake()
    {
        GameObject Task1Panel = GameObject.Find("MissionTask1");
        Task1Panel.GetComponent<Animator>().SetTrigger("Open");
    }

    // Update is called once per frame
    void Update()
    {

    }
    IEnumerator Disable(int waitTime)
    {
        while (true)
        {

            yield return new WaitForSeconds(waitTime);
            int randomRack = Random.Range(0, Racks.Length);
            int randomResource = Random.Range(0, Resources.Length);


            Racks[randomRack].SetActive(false);
            Resources[randomResource].gameObject.transform.GetChild(1).gameObject.SetActive(false);
            print("WaitAndPrint " + Time.time);


        }

    }
    public void Mission1()
    {
        taskBar.GetComponent<Animator>().SetTrigger("OpenTaskBar");
        int randomResource = Random.Range(0, Resources.Length);
        Resources[randomResource].gameObject.transform.GetChild(1).gameObject.SetActive(true);
        int randomRack = Random.Range(0, Racks.Length);
        //Racks[randomRack].transform.Rotate(0f, 90f, 0f);
        Racks[randomRack].transform.GetComponentInChildren<MeshRenderer>().material = gameTaskMat;
        Racks[randomRack].tag = "Target";
        PlayerController.instance.inMenu = false;

    }
    public void Mission2()
    {
        taskBar.GetComponent<Animator>().SetTrigger("OpenTaskBar");
        //taskBar.transform.GetChild(3).gameObject.SetActive(true);
        PlayerController.instance.inMenu = false;
        foreach (GameObject resourePallete in Resources)
        {
            resourePallete.gameObject.transform.GetChild(1).gameObject.SetActive(true);
        }
        for (int i = 0; i < Resources.Length; i++)
        {
            int randomRack = Random.Range(0, Racks.Length);
            Racks[randomRack].transform.GetComponentInChildren<MeshRenderer>().material = gameTaskMat;
            Racks[randomRack].tag = "Target";
        }
        taskBar.transform.GetChild(0).gameObject.transform.Find("Label").GetComponent<Text>().text = "0 of 4 box has set";
        taskBar.transform.GetChild(0).GetComponent<Toggle>().isOn = false;
        taskBar.transform.GetChild(1).gameObject.SetActive(false);
        taskBar.transform.GetChild(2).GetComponent<Text>().text = "Mission 2 tasks:";


    }
    public void StartNextMission()
    {
        if (missionNumber == 0)
        {
            Mission1();
        }
        if (missionNumber == 1)
        {
            Mission2();
        }
        PlayerController.instance.inMenu = false;
    }
}

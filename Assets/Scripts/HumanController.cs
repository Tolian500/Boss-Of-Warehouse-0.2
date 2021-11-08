using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using TMPro;

public class HumanController : MonoBehaviour
{
    public NavMeshAgent agent;
    public Camera cam;
    public bool isSelected;
    [SerializeField] protected Slider loadSlider;




    // Start is called before the first frame update
    void Start()
    {
        agent = gameObject.GetComponent<NavMeshAgent>();
        loadSlider = gameObject.transform.GetChild(1).transform.GetChild(0).GetComponent<Slider>();
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
}

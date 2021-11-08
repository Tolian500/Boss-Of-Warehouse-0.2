using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossController : HumanController
{
    [SerializeField] int bossSpeed;



    // Start is called before the first frame update
    void Start()
    {
        agent.speed = bossSpeed;
    }
    public void BossEnter()
    {
        agent.SetDestination(new Vector3(-10, -5, -5)); 
    }
    public void BossExit()
    {
        agent.SetDestination(new Vector3(-7, -5, 22));
    }


    // Update is called once per frame


}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AnimatorHandler : MonoBehaviour
{
    private Animator m_Animator;
    private NavMeshAgent m_Agent;
    [SerializeField] float agentSpeed;

    // Start is called before the first frame update
    void Start()
    {
        m_Agent = GetComponent<NavMeshAgent>();
        m_Animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (m_Agent != null && m_Animator != null)
        {
            m_Animator.SetFloat("Speed", m_Agent.velocity.magnitude / m_Agent.speed);
            agentSpeed = m_Agent.velocity.magnitude / m_Agent.speed;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AnimatorHandler : MonoBehaviour
{
    const float locomotionAnimationSmoothTime = .2f;
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
            float speedPercent = m_Agent.velocity.magnitude / m_Agent.speed;
            
            
            
            m_Animator.SetFloat("speedPercent", speedPercent, locomotionAnimationSmoothTime, Time.deltaTime);
            agentSpeed = m_Agent.velocity.magnitude / m_Agent.speed;
            
        }
    }
    public void BoxAnimating(bool hasBox)
    {
        int putAnimIndex = Random.Range(0, 3);
        m_Animator.SetInteger("putAnimIndex", putAnimIndex);
        Debug.Log(putAnimIndex + " = putAnimIndex");
        if (hasBox == true)
        {
            m_Animator.SetBool("hasBox", true);
            Debug.Log("WithBoxAnim starts");
        }
        else
        {
            m_Animator.SetBool("hasBox", false);
            Debug.Log("WithBoxAnim ends");
        }
    }
}

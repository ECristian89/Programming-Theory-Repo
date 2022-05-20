using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;



[RequireComponent(typeof(NavMeshAgent))]
public abstract class Unit : MonoBehaviour
{
    private int _HitPoints { get; set; }
    public int HitPoints;

    public float Speed = 3f;
    protected NavMeshAgent m_Agent;
    protected Unit m_Target;


    private void Awake()
    {
        m_Agent = GetComponent<NavMeshAgent>();
        m_Agent.speed = Speed;
        m_Agent.acceleration = 999;
        m_Agent.angularSpeed = 999;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // method to move on the NavMesh
    public virtual void GoTo(Unit target)
    {
        m_Target = target;
        if(m_Target !=null)
        {
            m_Agent.SetDestination(m_Target.transform.position);
            m_Agent.isStopped = false;
        }
    }

    // POLYMORPHISM
    public virtual void GoTo(Vector3 position)
    {
        m_Target = null;
        m_Agent.SetDestination(position);
        m_Agent.isStopped = false;
    }

    // ABSTRACTION
    protected abstract void TargetInRange();

    private void Attack(int AttackPower)
    {

    }

    private void TakeDamage()
    {

    }

    private void Die()
    {

    }

    private void Upgrade()
    {

    }
}

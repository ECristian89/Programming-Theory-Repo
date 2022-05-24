using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;



[RequireComponent(typeof(NavMeshAgent))]
public abstract class Unit : MonoBehaviour
{
    // ENCAPSULATION
    private int m_MaxHitPoints;
    public int MaxHitPoints
    {
        get { return m_MaxHitPoints; }
        set { m_MaxHitPoints = value; }
    }
    private int m_HitPoints;
    public int HitPoints
    {
        get { return m_HitPoints; }
        set 
        { 
            if(value<1)
            {
                Debug.LogError("can't set a negative value for HitPoints");
            }
            else
            {
                m_HitPoints = value; 
            }
        }
    }
    private int m_AttackPower;
    protected int AttackPower
    {
        get { return m_AttackPower; }
        set { if (value<1)
            {
                Debug.LogError("can't set a negative value for AttackPower");
            }
        else
            {
                m_AttackPower = value;
            }
        }
    }
    private float m_AttackSpeed;
    protected float AttackSpeed
    {
        get { return m_AttackSpeed; }
        set { if (value < 0.1f)
            {
                Debug.Log("can't set an AttackSPeed lower than 0.1");
            }
        else
            {
                m_AttackSpeed = value;
            }
        }
    }

    protected bool isAttacking;
    
    public float Speed = 3f;
    public float Range = 2.0f;
    protected NavMeshAgent m_Agent;    
    [SerializeField]
    protected Unit m_Target;
    [SerializeField]
    protected Building m_BTarget;

    private HitPointsSync uiRef;
    public GameObject HitPoint_pf;  

    private void Awake()
    {        
        m_Agent = GetComponent<NavMeshAgent>();
        m_Agent.speed = Speed;
        m_Agent.acceleration = 999;
        m_Agent.angularSpeed = 999;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position + transform.forward * Range,transform.lossyScale.x*2);
    }
    // Update is called once per frame
    void Update()
    {        
        var ray = new Ray(this.transform.position, this.transform.forward);
        RaycastHit hit;
        if (Physics.SphereCast(transform.position, transform.lossyScale.x,this.transform.forward, out hit, Range))
        {
            m_Target = hit.transform.gameObject.GetComponentInParent<Unit>();
            m_BTarget= hit.transform.gameObject.GetComponentInParent<Building>();
        }

        CheckTarget(m_Target,m_BTarget);
        
    }
    
    protected void CheckTarget(Unit target, Building Btarget)
    {
        if (m_Target != null)
        {
            float distance = Vector3.Distance(m_Target.transform.position, transform.position);
            if (distance < Range)
            {
                m_Agent.isStopped = true;
                TargetInRange();
            }
        }
        else if(m_BTarget!=null)
        {
            float distance = Vector3.Distance(m_BTarget.transform.position, transform.position);
            if (distance < Range)
            {
                m_Agent.isStopped = true;
                TargetInRange();
            }
        }
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

    public virtual void Attack(int AttackPower,Unit target)
    {
        if(target!=null)
        {
        target.TakeDamage(AttackPower);
        Debug.Log($"{target} was attacked with {AttackPower} attack power.");
        Debug.Log($"{target} HP is: {target.HitPoints}");
        }
    }
    
    //POLYMORPHISM
    // overload Attack method to take in either a Unit type or Building type as target
    public virtual void Attack(int AttackPower, Building target)
    {
        if (target != null)
        {
            target.TakeDamage(AttackPower);
            Debug.Log($"{target} was attacked with {AttackPower} attack power.");            
        }
    }

    public virtual void TakeDamage(int damage)
    {
        m_HitPoints -= damage;
        uiRef.UpdateValue(MaxHitPoints, HitPoints);
        if(m_HitPoints<=0)
        { 
            Die();
        }
    }

    public virtual void Die()
    {
        // reset Marker 
        if(this.transform.childCount!=0)
        {
            this.transform.GetChild(0).gameObject.transform.parent = null;
        }
        Destroy(uiRef.gameObject);
            Destroy(gameObject);
    }    

    protected IEnumerator InitiateAttack(int attackPower, float attackSpeed, Unit target)
    {
        yield return new WaitForSeconds(attackSpeed);        
        Attack(attackPower, target);
        isAttacking = false;
    }

    protected IEnumerator InitiateAttack(int attackPower, float attackSpeed, Building target)
    {
        yield return new WaitForSeconds(attackSpeed);
        Attack(attackPower, target);
        isAttacking = false;
    }

    // use this in child objects to initialize
    protected void InitializeUnitStats(int hitPoints, int attackPower, float attackSpeed)
    {
        HitPoints = hitPoints;
        AttackPower = attackPower;
        AttackSpeed = attackSpeed;
        MaxHitPoints = hitPoints;
        var hpUi = Instantiate(HitPoint_pf, transform.position, HitPoint_pf.transform.rotation);
        uiRef = hpUi.GetComponentInChildren<HitPointsSync>();
        hpUi.transform.GetComponent<HitPointsSync>().SetFollowTarget(gameObject);
    }

}

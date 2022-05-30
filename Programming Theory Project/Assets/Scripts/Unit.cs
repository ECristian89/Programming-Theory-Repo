using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System;

[RequireComponent(typeof(NavMeshAgent))]
public abstract class Unit : MonoBehaviour
{
    protected enum UnitType { Warrior=0,Swordsman=1,Knight=2}
    [SerializeField]
    protected UnitType m_UnitType;
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

    public float Speed = 3f;    // Movement speed
    public float SRange = 8.0f; // Scan range
    public float AtkRange;  // Attacking Range
    protected bool isAttacking;   
    protected NavMeshAgent m_Agent;  
    protected Unit m_Target;    
    protected Building m_BTarget;    
    private HitPointsSync uiRef;
    public GameObject HitPoint_pf;
    public Sprite[] Thbnail;
    private Collider[] ScannedTargets;
    public LayerMask ScanLayer; // the layer to scan hostile targets

    private void Awake()
    {        
        m_Agent = GetComponent<NavMeshAgent>();
        m_Agent.speed = Speed;
        m_Agent.acceleration = 999;
        m_Agent.angularSpeed = 999;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position +transform.forward*1.0f,transform.lossyScale.x*2*SRange);        
    }
    // Update is called once per frame
    void Update()
    {
        ScanTargets(); 
             
    }

    // Check if any hostile targets are in the view range
    protected virtual void ScanTargets()
    {
        ScannedTargets = Physics.OverlapSphere(transform.position, SRange,ScanLayer);

        foreach (var item in ScannedTargets)
        {
            m_Target = item.transform.gameObject.GetComponentInParent<Unit>();
            m_BTarget = item.transform.gameObject.GetComponentInParent<Building>();
            CheckTarget(m_Target, m_BTarget);
        }
    }
    
    // Check if attackable targets are in attack range
    // If not, close enough, move towards the target
    protected virtual void CheckTarget(Unit target, Building Btarget)
    {
        if (m_Target != null)
        {
            GoTo(target);
            float distance = Vector3.Distance(m_Target.transform.position, transform.position);
            if (distance < 1.2f)  // replace with unit attack range
            {
                m_Agent.isStopped = true;
                TargetInRange();
            }
        }
        else if(m_BTarget!=null)
        {
            GoTo(Btarget);
            float distance = Vector3.Distance(m_BTarget.transform.position, transform.position);
            if (distance < 2.8f) // replace with building attack range
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

    public virtual void GoTo(Building target)
    {
        m_BTarget = target;
        if (m_BTarget != null)
        {
            m_Agent.SetDestination(m_BTarget.transform.position);
            m_Agent.isStopped = false;
        }
    }

    // POLYMORPHISM
    public virtual void GoTo(Vector3 position)
    {
        m_Target = null;
        m_BTarget = null;
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
       // Debug.Log($"{target} was attacked with {AttackPower} attack power.");
       // Debug.Log($"{target} HP is: {target.HitPoints}");
        }
    }
    
    //POLYMORPHISM
    // overload Attack method to take in either a Unit type or Building type as target
    public virtual void Attack(int AttackPower, Building target)
    {
        if (target != null)
        {
            target.TakeDamage(AttackPower);
          //  Debug.Log($"{target} was attacked with {AttackPower} attack power.");            
        }
    }

    public virtual void TakeDamage(int damage)
    {
        m_HitPoints -= damage;
        uiRef.UpdateValue(MaxHitPoints, HitPoints);
        gameObject.GetComponent<DetailsUI>().CurrentHitPoints = HitPoints;
        if (GetComponent<DetailsUI>().Equals(GameManager.Instance.GetCurrentUI()))
        {           
            if(GameManager.SelectionName.text!="")
            GameManager.UpdateUIHp();
        }

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
        GameManager.ClearDetails();

        GameManager.Instance.IsGameOver();
        GameManager.Instance.IsVictorious();
    }    

    protected IEnumerator InitiateAttack(int attackPower, float attackSpeed, Unit target)
    {
        yield return new WaitForSeconds(attackSpeed);        
        Attack(attackPower, target);
        isAttacking = false;
    }

    // overload with building type
    protected IEnumerator InitiateAttack(int attackPower, float attackSpeed, Building target)
    {
        yield return new WaitForSeconds(attackSpeed);
        Attack(attackPower, target);
        isAttacking = false;
    }

    // use this in child objects to initialize
    protected void InitializeUnitStats(string unitName,int hitPoints,float speed, int attackPower, float attackSpeed,float attackRange,int productionCost,int upgradeCost,Sprite thumbnail)
    {
        HitPoints = hitPoints;
        Speed = speed;
        AtkRange = attackRange;
        AttackPower = attackPower;
        AttackSpeed = attackSpeed;
        MaxHitPoints = hitPoints;


        // envelop stats in a readonly structure and send it to the details script attached to this gameObject
        if(gameObject.GetComponent<DetailsUI>()!=null)
        {
        var stats = new Stats(unitName, hitPoints,speed, attackPower, attackSpeed,attackRange,productionCost,upgradeCost,thumbnail);
        //SendStats(stats);
            Util.SendStats(gameObject, stats);
        }
        // create the visual HitPoint object for visual feedback       
        Util.AddHitPointVisual(HitPoint_pf, transform,ref uiRef);


    }

    protected void InitializeUnitStats(UnitType unitType)
    {
       // m_UnitType = unitType;
        switch(unitType)
        {
            case UnitType.Warrior:   
                {
                    InitializeUnitStats("Warrior", 200, 3, 10, 1, 2, 42, 600,Thbnail[0]);
                    break;
                }
            case UnitType.Swordsman:
                { 
                    InitializeUnitStats("Swordsman", 240, 3, 25, 1, 2, 54, 900, Thbnail[1]);
                    break;
                }
            case UnitType.Knight:
                {                    
                    InitializeUnitStats("Knight", 420, 2, 58, 1.3f, 2, 78, 1700, Thbnail[2]);
                    break;
                }
        }
    }
    
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public abstract class Unit : MonoBehaviour, Util.UIInfoContent
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

    public float Speed = 3f;
    public float SRange = 8.0f; // Scan range
    public float AtkRange;  // Attacking Range
    protected bool isAttacking;    
    protected NavMeshAgent m_Agent;    
    [SerializeField]
    protected Unit m_Target;
    [SerializeField]
    protected Building m_BTarget;    
    private HitPointsSync uiRef;
    public GameObject HitPoint_pf; 
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

    // overload with building type
    protected IEnumerator InitiateAttack(int attackPower, float attackSpeed, Building target)
    {
        yield return new WaitForSeconds(attackSpeed);
        Attack(attackPower, target);
        isAttacking = false;
    }

    // use this in child objects to initialize
    protected void InitializeUnitStats(string unitName,int hitPoints,float speed, int attackPower, float attackSpeed,float attackRange,int productionCost,int upgradeCost)
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
        var stats = new Stats(unitName, hitPoints,speed, attackPower, attackSpeed,attackRange,productionCost,upgradeCost);
        SendStats(stats);
        }
        // create the visual HitPoint object for visual feedback       
        Util.AddHitPointVisual(HitPoint_pf, transform,ref uiRef);


    }

    void SendStats(Stats stats)
    {
        var _detail=gameObject.GetComponent<DetailsUI>();
        _detail.EntityName = stats.FullName;
        _detail.ProductionCost = stats.ProductionCost;
        _detail.UpgradeCost = stats.UpgradeCost;
        _detail.Properties = $"HP:{stats.HP} Attack:{stats.AttackPower} Speed:{stats.Speed} Attack speed:{stats.AttackSpeed} Range:{stats.Range}";
        _detail.Description = $"{stats.FullName}";
        

    }
    public virtual string GetName()
    {
        throw new System.NotImplementedException();
    }

    public string GetDescription()
    {
        throw new System.NotImplementedException();
    }

    public string GetProperties()
    {
        throw new System.NotImplementedException();
    }

    public int GetProductionCost()
    {
        throw new System.NotImplementedException();
    }

    public int GetUpgradeCost()
    {
        throw new System.NotImplementedException();
    }
}

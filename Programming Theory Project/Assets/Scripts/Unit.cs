using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System;

[RequireComponent(typeof(NavMeshAgent))]
public abstract class Unit : MonoBehaviour
{
    // possible player unit types :Recruit, Soldier, Archer, Knight, Cavalier, Paladin, Warlord
    // possible enemy unit types  :Samurai, Crusher, Bandit, Outlaw, Enforcer, Skeleton, Vampire, Liche, Shadow, Gnoll, Rider, Reaver
    protected enum UnitType { Recruit=0,Soldier=1,Archer=2,Knight=3}
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
                Debug.Log("can't set an AttackSpeed lower than 0.1");
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
    protected DetailsUI m_details;
    public LayerMask ScanLayer; // the layer to scan hostile targets

    private void Awake()
    {
        m_details = GetComponent<DetailsUI>();
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
        m_details.CurrentHitPoints = HitPoints;
        if (m_details.Equals(GameManager.Instance.GetCurrentUI()))
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
        //GameManager.ClearDetails();

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
        MaxHitPoints = hitPoints;
        HitPoints = hitPoints;
        Speed = speed;
        AtkRange = attackRange;
        AttackPower = attackPower;
        AttackSpeed = attackSpeed;        
        

        // envelop stats in a readonly structure and send it to the details script attached to this gameObject
        if(m_details!=null)
        {
        var stats = new Stats(unitName, hitPoints,speed, attackPower, attackSpeed,attackRange,productionCost,upgradeCost,thumbnail);
        
            Util.SendStats(gameObject, stats);
        }
        // create the visual HitPoint object for visual feedback       
        Util.AddHitPointVisual(HitPoint_pf, transform,ref uiRef);


    }

    protected void InitializeUnitStats(UnitType unitType)
    {       
        switch(unitType)  
        {
            case UnitType.Recruit:   
                {
                    InitializeUnitStats("Recruit", 200, 3, 10, 1, 2, 42, 60,Thbnail[0]);
                    break;
                }
            case UnitType.Soldier:
                { 
                    InitializeUnitStats("Soldier", 240, 3, 25, 1, 2, 54, 90, Thbnail[1]);
                    break;
                }
            case UnitType.Archer:
                {
                    InitializeUnitStats("Archer", 160, 5, 14, 1.2f, 2, 48, 110, Thbnail[2]);
                    break;
                }
            case UnitType.Knight:
                {                    
                    InitializeUnitStats("Knight", 420, 2, 58, 1.3f, 2, 78, 120, Thbnail[3]);
                    break;
                }
        }
    }
    public int UpgradeLevel = 0;
    public void UpgradeUnit()
    {
        GameManager.Instance.SubtractGold(transform.GetComponent<DetailsUI>().UpgradeCost);
        if (GameManager.canSpendGold)
        {

            UpgradeLevel++;

            // set new values for UI update (Default)
            string UpgradeName = "Common";
            var n_HP = HitPoints + 12;
            var n_speed = Speed;
            var n_AtkR = AtkRange;
            var n_Atk = AttackPower + 1;
            var n_AtkSpeed = AttackSpeed;
            var n_upgrade = m_details.UpgradeCost * UpgradeLevel + 6;

            switch (UpgradeLevel)
            {
                case 1:
                    {
                        UpgradeName = "Mercenary";
                        break;
                    }
                case 2:
                    {
                        UpgradeName = "Veteran";
                        n_HP = HitPoints + 10;
                        n_speed = Speed + 0.1f;
                        n_AtkR = AtkRange;
                        n_Atk = AttackPower + 2;
                        n_AtkSpeed = AttackSpeed - 0.05f;
                        break;
                    }
                case 3:
                    {
                        UpgradeName = "Champion";
                        n_HP = HitPoints + 24;
                        n_speed = Speed + 0.1f;
                        n_AtkR = AtkRange;
                        n_Atk = AttackPower + 5;
                        n_AtkSpeed = AttackSpeed - 0.08f;
                        break;
                    }
                case 4:
                    {
                        UpgradeName = "Elite";
                        n_HP = HitPoints + 30;
                        n_speed = Speed + 0.1f;
                        n_AtkR = AtkRange;
                        n_Atk = AttackPower + 8;
                        n_AtkSpeed = AttackSpeed - 0.12f;
                        break;
                    }
                case 5:
                    {
                        UpgradeName = "Hero";
                        n_HP = HitPoints + 55;
                        n_speed = Speed + 0.2f;
                        n_AtkR = AtkRange;
                        n_Atk = AttackPower + 12;
                        n_AtkSpeed = AttackSpeed - 0.15f;
                        break;
                    }
                case 6:
                    {
                        UpgradeName = "Legendary";
                        n_HP = HitPoints + 200;
                        n_speed = Speed + 0.8f;
                        n_AtkR = AtkRange;
                        n_Atk = AttackPower + 50;
                        n_AtkSpeed = AttackSpeed - 0.2f;
                        break;
                    }
            }

            if (UpgradeLevel > 6)
            {
                UpgradeName = "Legendary";
            }

            // test edge case and set limits
            if (n_AtkSpeed < 0.1f)
            {
                n_AtkSpeed = 0.1f;
            }
            if (n_upgrade > 9999)
            {
                n_upgrade = 9990;
            }

            // asign the values 
            AsignNewValues(UpgradeName, n_HP, n_speed, n_AtkR, n_Atk, n_AtkSpeed, n_upgrade);

            Util.ShowGraphicNotification("upgraded", transform.GetComponent<DetailsUI>());
        }
        else Util.ShowTextNotification();
    }
    
    private void AsignNewValues(string n_rankName, int n_HP, float n_speed,float n_AtkR,int n_Atk,float n_AtkSpeed,int n_upgrade)
    {
        HitPoints = n_HP;
        Speed = n_speed;
        AtkRange = n_AtkR;
        AttackPower = n_Atk;
        AttackSpeed = n_AtkSpeed;

        if (m_details != null)
        {
            var stats = new Stats(n_rankName + " " + m_UnitType, n_HP, n_speed, n_Atk, n_AtkSpeed, n_AtkR, m_details.ProductionCost, n_upgrade, m_details.Thumbnail);
            Util.SendStats(gameObject, stats); // visual only
            MaxHitPoints = m_details.MaxHitPoints;
        }
    }

    
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{
    protected enum BuildingType { Tent=0,Campsite=1,Cave=2,Garrison=3}
    [SerializeField]
    protected BuildingType m_BuildingType;
    private int m_MaxHitPoints;
    public int MaxHitPoints 
    {
        get { return m_MaxHitPoints; }
        set { m_MaxHitPoints = value; }
    }
    private int m_HitPoints;
    public int HitPoints 
    {
        get {return m_HitPoints; }
        set {m_HitPoints = value; }
    }
    private int m_AttackPower;
    public int AttackPower
    {
        get { return m_AttackPower; }
        set { m_AttackPower = value; }
    }
    private float m_AtkRange;
    public float AtkRange
    {
        get { return m_AtkRange; }
        set { m_AtkRange = value; }
    }
    private float m_AttackSpeed;
    public float AttackSpeed
    {
        get { return m_AttackSpeed; }
        set
        {
            if (value < 0.1f)
            {
                Debug.LogError("CAn't set an attack speed lower than 0.1"); 
            }
            else { m_AttackSpeed = value; }
        }
    }

    private float Speed = 0;  // no speed for buildings, should remove
    public float Range;
    public GameObject[] UnitPf = new GameObject[6];
    public Sprite[] Thbnail;
    public GameObject UpgradePf;
    public Transform SpawnPoint;
    private HitPointsSync uiRef;
    private bool isAttacking;
    [SerializeField]
    private Collider[] target;   
    public GameObject HitPoint_pf;
    public LayerMask ScanLayer;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, Range);
    }
    private void ScanEnemy()
    {
        target = Physics.OverlapSphere(transform.position, Range, ScanLayer);
        if (!isAttacking)
        {
            StartCoroutine(InitiateAttack());
        }
    }

    private void Update()
    {
        ScanEnemy();
    }

    protected void ScanTarget()
    {
        AttackSingle();
        //AutoAttackAll();
    }

    IEnumerator InitiateAttack()
    {
        isAttacking = true;
        yield return new WaitForSecondsRealtime(AttackSpeed);
        AttackSingle();
        isAttacking = false;
    }
    private void AutoAttackAll()
    {
        if (target.Length != 0)
        {
            foreach (var item in target)
            {
                item.transform.gameObject.GetComponentInParent<Unit>().TakeDamage(AttackPower);
            }            
        }

    }    

    private void AttackSingle()
    {
        if (target.Length != 0)
        {
            target[0].transform.gameObject.GetComponentInParent<Unit>().TakeDamage(AttackPower);
        }

    }
    public virtual void CreateUnit()  // use this for testing and enemies
    {
        var unit=Instantiate(UnitPf[0], SpawnPoint.position, UnitPf[0].transform.rotation);        
    }

    public virtual void CreateUnit(int index)    // use this for player
    {
        var unit = Instantiate(UnitPf[index], SpawnPoint.position, UnitPf[index].transform.rotation);
    }

    private void GetDestroyed()
    {
        Destroy(uiRef.gameObject);
        Destroy(gameObject);

        GameManager.Instance.IsGameOver();
        GameManager.Instance.IsVictorious();
    }
    public virtual void TakeDamage(int damage)
    {
        m_HitPoints -= damage;
        uiRef.UpdateValue(MaxHitPoints, HitPoints);
        gameObject.GetComponent<DetailsUI>().CurrentHitPoints = HitPoints;
        if (GetComponent<DetailsUI>().Equals(GameManager.Instance.GetCurrentUI()))  // check only when active selection
        {
            if (GameManager.SelectionName.text != "") // double check for edge cases
                GameManager.UpdateUIHp();
        }
        if (m_HitPoints <=0)
        {            
            GetDestroyed();
        }
    }
    protected void HandleVisual()
    {       
        Util.AddHitPointVisual(HitPoint_pf, transform, ref uiRef,transform.lossyScale.y);
    }

    protected void InitializeBuildingStats(string unitName, int hitPoints, float speed, int attackPower, float attackSpeed, float attackRange, int productionCost, int upgradeCost, Sprite thumbnail)
    {
        HitPoints = hitPoints;
        Speed = speed;
        AtkRange = attackRange;
        AttackPower = attackPower;
        AttackSpeed = attackSpeed;
        MaxHitPoints = hitPoints;

        // envelop stats in a readonly structure and send it to the details script attached to this gameObject
        if (gameObject.GetComponent<DetailsUI>() != null)
        {
            var stats = new Stats(unitName, hitPoints, speed, attackPower, attackSpeed, attackRange, productionCost, upgradeCost, thumbnail);          
            Util.SendStats(gameObject, stats);
        }

        // create the visual HitPoint object for visual feedback       
        HandleVisual();
    }

    protected void InitializeBuildingStats(BuildingType buildingType)   // use template buildings
    {

        switch(buildingType)
        {
            case BuildingType.Tent:
                {
                    InitializeBuildingStats(BuildingType.Tent.ToString(), 600, 0, 0, 2.5f, 0, 150, 948, Thbnail[0]);
                    break;
                }
            case BuildingType.Campsite:
                {
                    InitializeBuildingStats(BuildingType.Campsite.ToString(), 900, 0, 15, 2.5f, 5, 1098, 1200, Thbnail[1]);
                    break;
                }
            case BuildingType.Garrison:
                {
                    InitializeBuildingStats(BuildingType.Garrison.ToString(), 1440, 0, 45, 2f, 0, 2298, 2680, Thbnail[3]);
                    break;
                }
            case BuildingType.Cave:
                {
                    InitializeBuildingStats(BuildingType.Cave.ToString(), 600, 0, 0, 2.5f, 0, 150, 948, Thbnail[2]);
                    break;
                }
        }
    }

    public virtual void UpgradeBuilding()
    {
        // check if upgrade is possible maybe
        if (UpgradePf != null)
        {
            GameManager.Instance.SubtractGold(transform.GetComponent<DetailsUI>().UpgradeCost);
            if (GameManager.canSpendGold)
            {                
                var building = Instantiate(UpgradePf, transform.position, UpgradePf.transform.rotation);
                GameManager.ClearDetails();
                Util.ShowGraphicNotification("upgraded", transform.GetComponent<DetailsUI>());
                GetDestroyed();
            }
            else
            {
                Util.ShowTextNotification();
            }
        }
    }    
}

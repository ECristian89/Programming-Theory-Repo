using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{
    protected enum BuildingType { Tent=0,Campsite=1,Cave=2}
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
    public GameObject[] UnitPf = new GameObject[6];
    public Sprite[] Thbnail;
    public GameObject UpgradePf;
    public Transform SpawnPoint;
    private HitPointsSync uiRef;
    public GameObject HitPoint_pf;      

    private void ScanEnemy()
    {

    }   
    private void AutoAttack()
    {

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
        GameManager.UpdateUIHp();
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
                    InitializeBuildingStats("Tent", 600, 0, 0, 2.5f, 0, 150, 948, Thbnail[0]);
                    break;
                }
            case BuildingType.Campsite:
                {
                    InitializeBuildingStats("Campsite", 900, 0, 15, 2.5f, 5, 1098, 1200, Thbnail[1]);
                    break;
                }
            case BuildingType.Cave:
                {
                    InitializeBuildingStats("Cave", 600, 0, 0, 2.5f, 0, 150, 948, Thbnail[2]);
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
            var building = Instantiate(UpgradePf, transform.position, UpgradePf.transform.rotation);            
            GameManager.ClearDetails();
            GetDestroyed();
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{
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
    public GameObject[] UnitPf = new GameObject[6];
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
        if (m_HitPoints <=0)
        {            
            GetDestroyed();
        }
    }
    protected void HandleVisual()
    {
        Util.AddHitPointVisual(HitPoint_pf, transform, ref uiRef,transform.lossyScale.y);
    }

    protected void InitializeBuildingStats(int HP)
    {
        HitPoints = HP;
        MaxHitPoints = HitPoints;
    }

    public virtual void UpgradeBuilding()
    {
        // check if upgrade is possible maybe
        if (UpgradePf != null)
        {
            GameManager.Instance.SubtractGold(transform.GetComponent<DetailsUI>().UpgradeCost);
            var building = Instantiate(UpgradePf, transform.position, UpgradePf.transform.rotation);
            building.transform.GetComponent<Building>().InitializeBuildingStats(850);
            GetDestroyed();
            GameManager.ClearDetails();
        }
    }
}

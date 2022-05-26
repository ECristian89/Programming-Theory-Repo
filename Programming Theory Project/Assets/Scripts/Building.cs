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
    public GameObject UnitPf;
    public Transform SpawnPoint;
    private HitPointsSync uiRef;
    public GameObject HitPoint_pf;      

    private void ScanEnemy()
    {

    }

    public virtual string GetName()
    {
        return "Building";
    }
    private void AutoAttack()
    {

    }    
    public virtual void CreateUnit()
    {
        Instantiate(UnitPf, SpawnPoint.position, UnitPf.transform.rotation);        
    }
    private void GetDestroyed()
    {
        Destroy(uiRef.gameObject);
        Destroy(gameObject);
    }
    public virtual void TakeDamage(int damage)
    {
        m_HitPoints -= damage;
        uiRef.UpdateValue(MaxHitPoints, HitPoints);            
        if (m_HitPoints <=0)
        {
            Debug.Log("Building is destroyed");
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
}

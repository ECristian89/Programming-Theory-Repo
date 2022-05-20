using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// INHERITANCE
// Subclass of Unit that will attack enemy units in range
public class PlayerUnit : Unit
{
    private Unit m_CurrentTarget; 


    private void Start()
    {
        InitializeUnitStats(200, 10, 1);
    }
    public override void GoTo(Vector3 position)
    {
        base.GoTo(position);
        m_CurrentTarget = null;
    }
    protected override void TargetInRange()
    {
       

        if (m_Target.GetComponentInParent<EnemyUnit>())
        {
            if(!isAttacking)
            {
            isAttacking = true;
                StartCoroutine(base.InitiateAttack(AttackPower,AttackSpeed));
            }
        }        
        
    }    
    
}

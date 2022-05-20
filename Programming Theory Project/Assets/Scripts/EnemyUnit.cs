using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// INHERITANCE
public class EnemyUnit : Unit
{
    private void Start()
    {
        InitializeUnitStats(100, 60, 2);
    }
    protected override void TargetInRange()
    {

        if (m_Target.GetComponentInParent<PlayerUnit>())
        {
            if (!isAttacking)
            {
                isAttacking = true;
                StartCoroutine(base.InitiateAttack(AttackPower, AttackSpeed));
            }
        }
    }
   
}

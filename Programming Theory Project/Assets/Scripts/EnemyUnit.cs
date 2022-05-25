using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// INHERITANCE
public class EnemyUnit : Unit
{   
    private Transform DefaultDestination;
    private void Start()
    {
        InitializeUnitStats(100, 60, 2);
        DefaultDestination = GameObject.Find("EnemyDestination").transform;
        GoTo(DefaultDestination.position);
    }

    public override void GoTo(Vector3 position)
    {
        base.GoTo(position);
        m_Target = null;
        m_BTarget = null;
    }
    protected override void TargetInRange()
    {
        if (m_Target != null)
        {
            // set the player unit in range as target
            if (m_Target.GetComponentInParent<PlayerUnit>())
            {
                if (!isAttacking)
                {
                    isAttacking = true;
                    StartCoroutine(base.InitiateAttack(AttackPower, AttackSpeed, m_Target));
                }
            }
        }
        else if(m_BTarget!=null)
        {
            // set the building in range as target
            if (m_BTarget.GetComponentInParent<PlayerBuilding>())
            {
                if (!isAttacking)
                {
                    isAttacking = true;
                    StartCoroutine(base.InitiateAttack(AttackPower, AttackSpeed, m_BTarget));
                }
            }
        }

    }
    private void LateUpdate()
    {
        if (m_Target == null &&m_BTarget ==null)
        {
            GoTo(DefaultDestination.position);
        }       
    }   
}

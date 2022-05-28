using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// INHERITANCE
// Subclass of Unit that will attack enemy units in range
public class PlayerUnit : Unit
{    
   

    private void Start()
    {
        InitializeUnitStats("Warrior",200,3,10,1,2,42,600);
        GoTo(transform.position + new Vector3(0,0,2));
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
                // set the enemy unit in range as target
                if (m_Target.GetComponentInParent<EnemyUnit>())
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
                if (m_BTarget.GetComponentInParent<EnemyBuilding>())
                {
                    if (!isAttacking)
                    {
                        isAttacking = true;
                        StartCoroutine(base.InitiateAttack(AttackPower, AttackSpeed, m_BTarget));
                    }
                }
            }
               
        
    }   

}

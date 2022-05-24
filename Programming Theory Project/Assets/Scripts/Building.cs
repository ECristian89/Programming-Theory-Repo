using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{
    public GameObject UnitPf;
    public Transform SpawnPoint;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void ScanEnemy()
    {

    }

    private void AutoAttack()
    {

    }

    private void OnMouseDown()
    {
        CreateUnit();
    }
    private void CreateUnit()
    {
        Instantiate(UnitPf, SpawnPoint.position, UnitPf.transform.rotation);
    }
    private void GetDestroyed()
    {

    }
    public virtual void TakeDamage(int damage)
    {
        Debug.Log($"Building took {damage} damage");
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBuilding : Building
{
    [SerializeField]
    private float SpawnRate;
    [SerializeField]
    private int TotalUnits;

    // Start is called before the first frame update
    void Start()
    {
        base.HandleVisual();
        InitializeBuildingStats(600);
        TotalUnits = 25;
        StartCoroutine(Spawn());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
   IEnumerator Spawn()
    {
        while(TotalUnits>0)
        {
        yield return new WaitForSeconds(SpawnRate);
        CreateUnit();
            TotalUnits--;
        }
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBuilding : Building
{
    [SerializeField]
    private float SpawnRate;
    [SerializeField]
    private int TotalUnits;
    private bool hasSpawnBurst;
    public bool RandomizeSpawns;
    public GameObject CameraRightLimit;
    public GameObject[] SectionPf;
    private float[] Section = new float[5];
    protected int goldValue;

    // Start is called before the first frame update
    void Start()
    {        
        InitializeBuildingStats(m_BuildingType);
        TotalUnits = 25;
        StartCoroutine(Spawn());
        Section[0] = 152f;
        goldValue = 420;
    }
    
   IEnumerator Spawn()
    {
        while(TotalUnits>0)
        {
        yield return new WaitForSeconds(SpawnRate);
        CreateUnit();
            TotalUnits--;
            if(RandomizeSpawns)
            {
                SpawnRate = Random.Range(1.5f, 7f);
            }
        }
    }

    //open a new section and set the camera limit to its bounds
    private void OpenNextSection(GameObject section, float cameraLimit)
    {        
        
            section.SetActive(true);
            CameraRightLimit.transform.position = new Vector3(CameraRightLimit.transform.position.x, CameraRightLimit.transform.position.y, cameraLimit);
        
    }
    void SpawnBurst()
    {       
            CreateUnit();
            TotalUnits--;        
    }

    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);
        if(HitPoints<=250 && !hasSpawnBurst)
        {
            hasSpawnBurst = true;
            for (int i = 0; i < 3; i++)
            {
             SpawnBurst();
            }
            if (SectionPf != null && CameraRightLimit != null)
                OpenNextSection(SectionPf[0],Section[0]);
        }
    }
    protected override void GetDestroyed()
    {
        base.GetDestroyed();
        GameManager.Instance.AddGold(goldValue);
    }
}

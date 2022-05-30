using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBuilding : Building
{
    [SerializeField]
    private int[] UnitCost= new int[6];
    // Start is called before the first frame update
    void Start()
    {
        base.HandleVisual();
        InitializeBuildingStats(800);

        for (int i = 0; i < UnitCost.Length; i++)
        {
            if (GetComponent<DetailsUI>().Interactable[i] != null)
            {               
                UnitCost[i] = GetComponent<DetailsUI>().Interactable[i].transform.GetComponent<AsignValue>().Production;
            }
        }
    }   
   
    public override void CreateUnit(int unitIndex)
    {
        
        // since this requires spending gold, check if we have enough balance
            GameManager.Instance.SubtractGold(UnitCost[unitIndex]);
        if (GameManager.canSpendGold)
        {           
            var unit = Instantiate(UnitPf[unitIndex], SpawnPoint.position, UnitPf[unitIndex].transform.rotation);   // we need to override the base call to reference the thumbnail for notification
            GameManager.Instance.ShowNotification(GameManager.Instance.NotificationPf[1], "created", unit.GetComponent<DetailsUI>().Thumbnail);
        }
        else
            GameManager.Instance.ShowNotification(GameManager.Instance.NotificationPf[0]);
    }
    
}

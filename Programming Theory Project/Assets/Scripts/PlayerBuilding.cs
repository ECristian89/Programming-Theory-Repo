using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBuilding : Building
{
    // Start is called before the first frame update
    void Start()
    {
        base.HandleVisual();
        InitializeBuildingStats(600);
    }

    public override string GetName()
    {
        return "Tent";
    }

    public override void CreateUnit()
    {
        // since this requires spending gold, check if we have enough balance
            GameManager.Instance.SubtractGold(42);
        if (GameManager.canSpendGold)
        {           
            var unit = Instantiate(UnitPf, SpawnPoint.position, UnitPf.transform.rotation);   // we need to override the base call to reference the thumbnail for notification
            GameManager.Instance.ShowNotification(GameManager.Instance.NotificationPf[1], "created", unit.GetComponent<DetailsUI>().Thumbnail);
        }
        else
            GameManager.Instance.ShowNotification(GameManager.Instance.NotificationPf[0]);
    }
}

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
            base.CreateUnit();
        }
        else
            Debug.Log("Not enough gold!");
    }
}

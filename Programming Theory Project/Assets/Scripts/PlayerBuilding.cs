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
        base.CreateUnit();
        GameManager.Instance.SubtractGold(10);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBuilding : Building
{
    // Start is called before the first frame update
    void Start()
    {
        base.HandleVisual();
        InitializeStats(600);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnMouseDown()
    {
        CreateUnit();
    }
}

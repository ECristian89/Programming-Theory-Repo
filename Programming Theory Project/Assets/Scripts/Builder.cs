using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Builder : MonoBehaviour
{
    public delegate void BuildingUnit();
    BuildingUnit worker;

    public void AsignDel(BuildingUnit order)
    {
        worker = order;        
    }

    public void StartBuild()
    {
        worker();
    }
   
}

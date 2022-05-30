using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Util : MonoBehaviour
{
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="HitPointPf"> The prefab of hte UI element</param>
    /// <param name="Pos"> The current transform</param>
    /// <param name="UiRef">The script attached to the prefab</param>
    public static void AddHitPointVisual(GameObject HitPointPf,Transform Pos,ref HitPointsSync UiRef)
    {
        var hpUi = Instantiate(HitPointPf, Pos.transform.position, HitPointPf.transform.rotation);
        UiRef = hpUi.GetComponentInChildren<HitPointsSync>();     
        hpUi.transform.GetComponent<HitPointsSync>().SetFollowTarget(Pos.gameObject);
    }

    public static void AddHitPointVisual(GameObject HitPointPf, Transform Pos, ref HitPointsSync UiRef,float height)
    {
        var hpUi = Instantiate(HitPointPf, Pos.transform.position, HitPointPf.transform.rotation);
        UiRef = hpUi.GetComponentInChildren<HitPointsSync>();
        UiRef.SetHeight(height);
        hpUi.transform.GetComponent<HitPointsSync>().SetFollowTarget(Pos.gameObject);
    }

    public static void SendStats(GameObject targetObject, Stats stats)
    {
        var _detail = targetObject.GetComponent<DetailsUI>();

        _detail.EntityName = stats.FullName;
        _detail.Description = $"{stats.FullName}";

        _detail.ProductionCost = stats.ProductionCost;
        _detail.UpgradeCost = stats.UpgradeCost;

        _detail.MaxHitPoints = stats.HP;
        _detail.CurrentHitPoints = stats.HP;

        var atkspd = (1 / stats.AttackSpeed).ToString("N1");
        _detail.Properties = $"HP:{stats.HP}  Attack:{stats.AttackPower}\nSpeed:{stats.Speed}  Attack speed:{atkspd}\nRange:{stats.Range} ";
        _detail.Thumbnail = stats.Thumbnail;
    }
   
}

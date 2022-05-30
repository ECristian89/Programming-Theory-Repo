
using UnityEngine;

public class Stats
{
    // restrict access
    // we only need to read these values
    public readonly string FullName;
    public readonly int HP;
    public readonly float Speed;
    public readonly int AttackPower;
    public readonly float AttackSpeed;
    public readonly float Range;
    public readonly int ProductionCost;
    public readonly int UpgradeCost;
    public readonly Sprite Thumbnail;

    public Stats(string name,int hitPoints, float speed,int atkPower,float atkSpeed,float range,int prodCost,int upgradeCost,Sprite thumbnail)
    {
        FullName = name;
        HP = hitPoints;
        Speed = speed;
        AttackPower = atkPower;
        AttackSpeed = atkSpeed;
        Range = range;
        ProductionCost = prodCost;
        UpgradeCost = upgradeCost;
        Thumbnail = thumbnail;
    }   

}

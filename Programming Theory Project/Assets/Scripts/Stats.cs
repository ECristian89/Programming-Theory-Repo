
using UnityEngine;

public class Stats
{
    // Common, Mercenary, Veteran,Champion, Elite, Hero, Legendary
    public enum UpgradeType { Gray=0,Green=1,Blue=2,White=3,Purple=4,Orange=5,Red=6} 
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
 

    public Stats(string name,int hitPoints, float speed,int atkPower,float atkSpeed,float range,int prodCost,int upgradeCost,Sprite thumbnail) // for initialization
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

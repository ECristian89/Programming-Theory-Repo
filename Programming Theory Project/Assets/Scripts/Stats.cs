
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

    public Stats(string name,int hitPoints, float speed,int atkPower,float atkSpeed,float range,int prodCost,int upgradeCost)
    {
        FullName = name;
        HP = hitPoints;
        Speed = speed;
        AttackPower = atkPower;
        AttackSpeed = atkSpeed;
        Range = range;
        ProductionCost = prodCost;
        UpgradeCost = upgradeCost;

    }   

}

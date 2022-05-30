using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DetailsUI : MonoBehaviour
{
    public string EntityName;
    public string Description;

    public int ProductionCost;
    public int UpgradeCost;

    public int CurrentHitPoints;
    public int MaxHitPoints;
    public int AttackPower;
    public float AttackSpeed;
    public float Range;

    public string Properties;
    public Sprite Thumbnail;
    public GameObject[] Interactable= new GameObject[6];
    public GameObject UpgradeBtn;
    public Image HpBar;
   
}

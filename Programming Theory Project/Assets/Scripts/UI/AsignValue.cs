using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AsignValue : MonoBehaviour
{
    public int Production;
    public int Upgrade;
    public TextMeshProUGUI ProductionText;
    public TextMeshProUGUI UpgradeText;

    // Start is called before the first frame update
    void Start()
    {
        if(ProductionText)
        ProductionText.text = $"{Production}";

        if(UpgradeText)
        UpgradeText.text = $"{Upgrade}";
    }   
}

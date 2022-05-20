using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HitPointsSync : MonoBehaviour
{
    Image FillBar;
    private float hpValue;
    private Vector3 offset = new Vector3(0,1.52f,0);
    [SerializeField]
    private Transform followTarget;


    private void Awake()
    {
        FillBar = GetComponentInChildren<Image>();
    }

    private void Update()
    {
        transform.position = followTarget.position + offset;
    }
    public void UpdateValue(int maxValue,int targetValue)
    {
        hpValue = (float)targetValue / maxValue;
        FillBar.fillAmount = hpValue;
    }

    public void SetFollowTarget(GameObject obj)
    {
        followTarget = obj.transform;
    }
}

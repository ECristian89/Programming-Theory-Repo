using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeOut : MonoBehaviour
{
    [SerializeField]
    private float viewDuration;  // how long should this element be visible
    
    void Start()
    {
        StartCoroutine(DeleteElement());
    }

    IEnumerator DeleteElement()
    {
        yield return new WaitForSecondsRealtime(viewDuration);
        Destroy(gameObject);
    }
   
}

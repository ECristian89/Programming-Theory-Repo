using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableTimer : MonoBehaviour
{
    private float duration=2f;

    private void OnEnable()
    {
        StartCoroutine(StartCounter());
    }

    public void RefreshCounter()
    {
        StopAllCoroutines();
        StartCoroutine(StartCounter());
    }

    IEnumerator StartCounter()
    {
        yield return new WaitForSecondsRealtime(duration);
        gameObject.SetActive(false);
    }
}

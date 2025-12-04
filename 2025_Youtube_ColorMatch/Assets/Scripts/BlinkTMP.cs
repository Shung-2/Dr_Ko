using System;
using UnityEngine;
using TMPro;
using System.Collections;

public class BlinkTMP : MonoBehaviour
{
    private TextMeshProUGUI text;

    private void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
    }

    private void OnEnable()
    {
        StartCoroutine(nameof(Process));
    }

    private void OnDisable()
    {
        StopCoroutine(nameof(Process));
    }
    
    private IEnumerator Process()
    {
        while (true)
        {
            yield return StartCoroutine(FadeEffect.Fade(text, 1f, 0f, 1f));
            yield return StartCoroutine(FadeEffect.Fade(text, 0f, 1f, 1f));
        }
    }
}

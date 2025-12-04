using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class CountingEffect : MonoBehaviour
{
    private TextMeshProUGUI text;   // 카운팅 효과에 사용되는 텍스트

    private void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
    }
    
    public void Play(int start, int end, float playTime = 1f, UnityAction action = null)
    {
        StartCoroutine(Process(start, end, playTime, action));
    }

    private IEnumerator Process(int start, int end, float playTime, UnityAction acction)
    {
        float percent = 0f;
        
        while (percent < 1f)
        {
            percent += Time.deltaTime / playTime;
            text.text = Mathf.RoundToInt(Mathf.Lerp(start, end, percent)).ToString("F0");
            yield return null;
        }
        
        acction?.Invoke();
    }
}

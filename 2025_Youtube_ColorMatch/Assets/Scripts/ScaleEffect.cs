using System.Collections;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Events;

public class ScaleEffect : MonoBehaviour
{
    public void Play(Vector3 start, Vector3 end, float playTime = 1f, UnityAction action = null)
    {
        StartCoroutine(Process(start, end, playTime, action));
    }

    private IEnumerator Process(Vector3 start, Vector3 end, float playTime, UnityAction action)
    {
        float percent = 0f;
        
        while (percent < 1f)
        {
            percent += Time.deltaTime / playTime;
            transform.localScale = Vector3.Lerp(start, end, percent);
            yield return null;
        }
        
        action?.Invoke();
    }
}

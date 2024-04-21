using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class ScrollViewPosition : MonoBehaviour
{
    public TextMeshProUGUI tmp;
    ScrollRect scrollRect;

    void Start()
    {
        scrollRect = GetComponent<ScrollRect>();
        tmp.text = scrollRect.normalizedPosition.ToString();
    }

    public void SetText()
    {
        tmp.text = scrollRect.normalizedPosition.ToString();
    }

    public void ScrollTop()
    {
        scrollRect.normalizedPosition = new Vector2(0,1);
    }

    public void ScrollBottom()
    {
        scrollRect.normalizedPosition = new Vector2(0,0);
    }

    public void ScrollUp()
    {
        StartCoroutine(ScrollUpCoroutine());
    }

    public void ScrollDown()
    {
        StartCoroutine(ScrollDownCoroutine());
    }

    IEnumerator ScrollUpCoroutine()
    {
        Vector2 start = scrollRect.normalizedPosition;
        Vector2 end = new Vector2(0, 1);
        float duration = 1; // 변경 시간
        float startTime = Time.time;

        while(Time.time - startTime < duration)
        {
            float t = (Time.time - startTime) / duration; // 시간에 따른 보간값 계산
            // float curveValue = Mathf.Sin(t * Mathf.PI * 0.5f); // 사인 곡선을 사용한 보간
            float easedT = EaseInOutQuad(0, 1, t);
            scrollRect.normalizedPosition = Vector2.Lerp(start, end, easedT);
            yield return null;
        }

        ScrollTop();
    }

    IEnumerator ScrollDownCoroutine()
    {
        Vector2 start = scrollRect.normalizedPosition;
        Vector2 end = new Vector2(0, 0);
        float duration = 1; // 변경 시간
        float startTime = Time.time;

        while(Time.time - startTime < duration)
        {
            float t = (Time.time - startTime) / duration;
            // float curveValue = Mathf.Sin(t * Mathf.PI * 0.5f);
            float easedT = EaseInOutQuad(0, 1, t);
            scrollRect.normalizedPosition = Vector2.Lerp(start, end, easedT);
            yield return null;
        }

        ScrollBottom();
    }

    float EaseInOutQuad(float start, float end, float t)
    {
        t = Mathf.Clamp01(t);
        t = t < 0.5f? 2.0f*t*t : -1.0f+(4.0f - 2.0f * t) * t;
        return Mathf.Lerp(start, end, t);
    }
}

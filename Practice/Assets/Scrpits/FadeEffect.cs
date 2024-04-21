using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FadeEffect : MonoBehaviour
{
    public Image fadeImage;

    [SerializeField]
    private float fadeTime = 0.5f;

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.A))
        {
            print("Fade In");
            StopAllCoroutines();
            StartCoroutine(Fade(1,0)); // Fade In
        }

        if(Input.GetKeyDown(KeyCode.S))
        {
            print("Fade Out");
            StopAllCoroutines();
            StartCoroutine(Fade(0,1)); // Fade Out
        }

        if(Input.GetKeyDown(KeyCode.D))
        {
            print("Fade Loop");
            StopAllCoroutines();
            StartCoroutine(FadeLoop()); // Fade In, Out(Loop)
        }
    }

    private IEnumerator Fade(float start, float end)
    {
        float current = 0;
        float percent = 0;

        while(percent < 1)
        {
            current += Time.deltaTime;
            percent = current / fadeTime;

            Color color = fadeImage.color;
            color.a = Mathf.Lerp(start, end, percent);
            fadeImage.color = color;
            yield return null;
        }
    }

    private IEnumerator FadeLoop()
    {
        while(true)
        {
            yield return StartCoroutine(Fade(1,0)); // Fade In
            yield return StartCoroutine(Fade(0,1)); // Fade Out
        }
    }
}

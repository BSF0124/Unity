using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BackgroundManager : MonoBehaviour
{
    public Sprite[] backgrounds;
    public Image backgroundImage1; // 024
    public Image backgroundImage2; // 135
    private float duration = 1f;
    private int max = 5;
    private int bgType = 0;

    private void Update()
    {
        if(bgType != PlayerPrefs.GetInt("Score") / 10)
        {
            bgType = PlayerPrefs.GetInt("Score") / 10;
            if(bgType % 2 == 0)
                StartCoroutine(ChangeBackground(false));
            else
                StartCoroutine(ChangeBackground(true));
        }
    }

    IEnumerator ChangeBackground(bool odd)
    {
        if(bgType < max)
        {
            if(odd)
            {
                if(!backgroundImage2.gameObject.activeSelf)
                {
                    backgroundImage2.gameObject.SetActive(true);
                    backgroundImage2.sprite = backgrounds[bgType];
                    Sequence bgSequence = DOTween.Sequence();
                    bgSequence.Append(backgroundImage2.DOFade(1, duration))
                    .Join(backgroundImage1.DOFade(0, duration));
                    yield return bgSequence.WaitForCompletion();
                    backgroundImage1.gameObject.SetActive(false);
                }
            }
            else
            {
                if(!backgroundImage1.gameObject.activeSelf)
                {
                    backgroundImage1.gameObject.SetActive(true);
                    backgroundImage1.sprite = backgrounds[bgType];
                    Sequence bgSequence = DOTween.Sequence();
                    bgSequence.Append(backgroundImage1.DOFade(1, duration))
                    .Join(backgroundImage2.DOFade(0, duration));
                    yield return bgSequence.WaitForCompletion();
                    backgroundImage2.gameObject.SetActive(false);
                }
            }
        }
    }
}

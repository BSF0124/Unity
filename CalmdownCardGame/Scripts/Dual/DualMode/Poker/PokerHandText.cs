using UnityEngine;
using TMPro;
using DG.Tweening;
using System.Collections;

public class PokerHandText : MonoBehaviour
{
    private RectTransform rectTransform;
    private TextMeshProUGUI tmp;
    private Vector2 initPosition;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        tmp = GetComponent<TextMeshProUGUI>();
        initPosition = GetComponent<RectTransform>().anchoredPosition;
    }
    
    private void OnEnable()
    {
        tmp.color = new Color(1,1,1,1);
        rectTransform.DOAnchorPosY(rectTransform.anchoredPosition.y + 50f, 0.5f);
    }

    public IEnumerator TextEffect()
    {
        yield return rectTransform.DOAnchorPosY(rectTransform.anchoredPosition.y + 50f, 0.5f);
        yield return tmp.DOFade(0, 0.5f).WaitForCompletion();
        rectTransform.anchoredPosition = initPosition;
        gameObject.SetActive(false);
    }
}

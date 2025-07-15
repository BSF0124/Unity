using UnityEngine;
using DG.Tweening;
using TMPro;

public class TextEffect_Blackjack : MonoBehaviour
{
    private Vector2 init_Position;
    private float duration = 0.7f;

    private void Awake()
    {
        init_Position = GetComponent<RectTransform>().anchoredPosition;
    }

    private void OnEnable()
    {
        GetComponent<RectTransform>().anchoredPosition = init_Position;
        GetComponent<TextMeshProUGUI>().color = new Color(1f,1f,1f,1f);

        GetComponent<RectTransform>().DOAnchorPosY(GetComponent<RectTransform>().anchoredPosition.y + 120f, duration);
        GetComponent<TextMeshProUGUI>().DOFade(0, duration)
        .OnComplete(()=> 
        {
            gameObject.SetActive(false);
        } );
    }
}

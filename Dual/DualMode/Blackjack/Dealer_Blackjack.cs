using System.Collections;
using UnityEngine;
using DG.Tweening;

public class Dealer_Blackjack : MonoBehaviour
{
    // 카드들을 왼쪽으로 이동
    public IEnumerator CardMove()
    {
        foreach(Transform card in transform)
        {
            yield return card.GetComponent<RectTransform>().DOAnchorPosX(card.GetComponent<RectTransform>().anchoredPosition.x - 109f, 0.3f).WaitForCompletion();
        }
    }
}

using System.Collections;
using UnityEngine;
using DG.Tweening;

public class BlackjackField : MonoBehaviour
{
    public int score = 0;
    private float duration = 0.2f;

    // 카드들을 왼쪽으로 이동
    public IEnumerator CardMove()
    {
        foreach(Transform card in transform)
        {
            card.GetComponent<RectTransform>().DOAnchorPosX(card.GetComponent<RectTransform>().anchoredPosition.x - 109f, duration)
            .OnComplete(()=> 
                card.GetComponent<BlackjackCard>().initalPosition = card.GetComponent<BlackjackCard>().rectTransform.anchoredPosition
            );
            yield return null;
        }
        yield return new WaitForSeconds(duration);
    }

    public void CalculateScore()
    {
        score = 0;
        foreach(Transform item in transform)
        {
            switch(CardDataManager.instance.GetCardByID(item.GetComponent<BlackjackCard>().cardID).cardRarity)
            {
                case CardRarity.N:
                    score += 3;
                    break;
                case CardRarity.R:
                    score += 4;
                    break;
                case CardRarity.SR:
                    score += 6;
                    break;
            }
        }

        // 텍스트 업데이트
        transform.parent.GetComponent<BlackjackManager>().scoreTexts.UpdateText();

        // 버스트 체크
        if(score > 15)
        {
            DualManager.isSequenceRunning = true;
            StartCoroutine(transform.parent.GetComponent<BlackjackManager>().Bust());
        }
        else
        {
            if(transform.parent.GetComponent<BlackjackManager>().player)
                DualManager.isSequenceRunning = false;
            
            else
                StartCoroutine(transform.parent.GetComponent<BlackjackManager>().DealerTurn());
        }
    }
}

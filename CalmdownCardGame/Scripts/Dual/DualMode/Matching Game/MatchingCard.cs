using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class MatchingCard : Card, IPointerClickHandler
{
    public Sprite cardSprite;
    public int cardID;
    public bool IsFlipped = false;

    public void Init(int cardID)
    {
        this.cardID = cardID;
        cardSprite = CardDataManager.instance.GetCardByID(cardID).cardSprite;
    }

    public void Update()
    {
        if(DualManager.isSequenceRunning)
        {
            cardImage.transform.DOScale(Vector3.one, 0.5f);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(!DualManager.isSequenceRunning && transform.parent.parent.GetComponent<MatchingGameManager>().playerTurn && !IsFlipped)
        {
            transform.parent.parent.GetComponent<MatchingGameManager>().SelectCard(this);
        }
    }

    public override void OnBeginDrag(PointerEventData eventData)
    {
    }

    public override void OnDrag(PointerEventData eventData)
    {
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
    }
}

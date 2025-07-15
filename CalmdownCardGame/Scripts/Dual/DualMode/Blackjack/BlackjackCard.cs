using System.Collections;
using UnityEngine.EventSystems;

public class BlackjackCard : Card
{
    public int cardID;  // 카드ID

    public IEnumerator DestroyCard()
    {
        yield return StartCoroutine(Disappear((int)CardDataManager.instance.GetCardByID(cardID).cardRarity));
        Destroy(gameObject);
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
    }

    // 드래그 시작 위치 저장
    public override void OnBeginDrag(PointerEventData eventData)
    {
    }

    // 카드의 위치를 드래그한 위치로 이동
    public override void OnDrag(PointerEventData eventData)
    {
    }

    // 드래그 종료 후 초기 위치로 이동
    public override void OnEndDrag(PointerEventData eventData)
    {
    }
}

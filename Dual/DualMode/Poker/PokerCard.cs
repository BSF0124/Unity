using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

public class PokerCard : Card, IPointerClickHandler
{
    [HideInInspector] public CardData cardData;
    [HideInInspector] public int life;
    [HideInInspector] public Status_Poker status;

    public bool isPlayerCard = true;
    
    private Tween posTween;
    private bool isSelected = false;    // 카드 선택 확인

    // 초기 위치와 회전값 설정
    private void OnEnable()
    {
        initalPosition = rectTransform.anchoredPosition;
        initalRotation = rectTransform.rotation;
    }

    // 카드 초기 설정
    public void Init(int cardID)
    {
        cardData = CardDataManager.instance.GetCardByID(cardID);
        switch(cardData.cardRarity)
        {
            case CardRarity.N:
                life = 1;
                break;
            case CardRarity.R:
                life = 2;
                break;
            case CardRarity.SR:
                life = 3;
                break;
        }
        
        if(DualManager.stage.dualMode.Contains(DualMode.Equality))
        {
            life = 1;
        }

        cardRect.GetComponent<Image>().sprite = CardDataManager.instance.GetCardByID(cardID).cardSprite;
        status = transform.GetChild(0).GetChild(0).GetComponent<Status_Poker>();
    }

    // 카드 선택
    public void Select()
    {
        transform.parent.parent.GetComponent<PokerManager>().SelectCard(this);
        isSelected = true;
        rectTransform.DOAnchorPos(initalPosition + new Vector2(0, 50), duration);
        initalPosition.y += 50f;
    }

    // 카드 선택 해제
    public void Deselect()
    {
        isSelected = false;
        initalPosition.y -= 50f;
        rectTransform.DOAnchorPos(initalPosition, duration);
        transform.parent.parent.GetComponent<PokerManager>().DeselectCard(this);
    }

    // 카드 파괴 애니메이션
    public IEnumerator DestroyCard()
    {
        AudioManager.instance.PlaySfx(AudioManager.instance.sfxClips_DualMode[3]);
        yield return StartCoroutine(Disappear((int)cardData.cardRarity));

        if(isSelected)
            Deselect();

        Destroy(gameObject);
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        if(!DualManager.isSequenceRunning && !DualManager.isDragging)
        {
            if(isPlayerCard)
            {
                transform.SetAsLastSibling();
                cardRect.rotation = Quaternion.Euler(Vector3.zero);
                cardRect.localScale = Vector3.one * 1.2f;
                posTween = cardRect.DOAnchorPosY(50, duration);
            }
            else
            {
                transform.SetAsLastSibling();
                cardRect.DOScale(new Vector3(1.1f, 1.1f, 1.1f), duration);
            }

            status.ShowStatus();
        }
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        if(!DualManager.isSequenceRunning && !DualManager.isDragging)
        {
            if(isPlayerCard)
            {
                transform.parent.GetComponent<Hand_Poker>().SetObjectSibling();
                cardRect.rotation = initalRotation;
                cardRect.localScale = Vector3.one;
                cardRect.anchoredPosition = Vector2.zero;
            }
            else
            {
                cardRect.DOScale(Vector3.one, duration);
            }
            posTween.Kill();
            status.HideStatus();
        }
    }

    // 드래그 시작 위치 저장
    public override void OnBeginDrag(PointerEventData eventData)
    {
        if(!DualManager.isSequenceRunning)
        {
            if(isPlayerCard)
            {
                DualManager.isDragging = true;

                transform.parent.GetComponent<Hand_Poker>().SetObjectSibling();
                posTween.Kill();
                cardRect.rotation = initalRotation;
                cardRect.localScale = Vector3.one;
                cardRect.anchoredPosition = Vector2.zero;
                status.HideStatus();

                RectTransformUtility.ScreenPointToLocalPointInRectangle(DualManager.canvas.transform as RectTransform, eventData.position, eventData.pressEventCamera, out var localPointerPosition);
                dragOffset = localPointerPosition - rectTransform.anchoredPosition;
            }
            else
            {
                DualManager.isDragging = true;
                cardRect.localScale = Vector3.one;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(DualManager.canvas.transform as RectTransform, eventData.position, eventData.pressEventCamera, out var localPointerPosition);
                dragOffset = localPointerPosition - rectTransform.anchoredPosition;
            }
        }
    }

    // 카드를 드래그한 위치로 이동
    public override void OnDrag(PointerEventData eventData)
    {
        if(!DualManager.isSequenceRunning)
        {
            if(isPlayerCard)
            {
                cardRect.localScale = Vector3.one;
                transform.DORotate(Vector3.zero, duration);
                Vector2 mousePosition;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(DualManager.canvas.transform as RectTransform, eventData.position, eventData.pressEventCamera, out mousePosition);
                rectTransform.anchoredPosition = mousePosition - dragOffset;
            }
            else
            {
                cardRect.localScale = Vector3.one;
                transform.DORotate(Vector3.zero, duration);
                Vector2 mousePosition;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(DualManager.canvas.transform as RectTransform, eventData.position, eventData.pressEventCamera, out mousePosition);
                rectTransform.anchoredPosition = mousePosition - dragOffset;
            }
        }
    }

    // 드래그 종료 후 초기 위치로 이동
    public override void OnEndDrag(PointerEventData eventData)
    {
        if(!DualManager.isSequenceRunning)
        {
            if(isPlayerCard)
            {
                rectTransform.DOAnchorPos(initalPosition, duration);
                transform.DORotateQuaternion(initalRotation, duration)
                    .OnComplete(()=>{
                        DualManager.isDragging = false;
                        transform.rotation = initalRotation;
                        cardRect.localScale = Vector3.one;
                        transform.parent.GetComponent<Hand>().SetObjectSibling();
                    });
            }
            else
            {
                rectTransform.DOAnchorPos(initalPosition, duration);
                transform.DORotateQuaternion(initalRotation, duration)
                    .OnComplete(()=>{
                        DualManager.isDragging = false;
                        transform.rotation = initalRotation;
                        cardRect.localScale = Vector3.one;
                        status.HideStatus();
                    });
            }
        }
    }

    // 카드 선택 / 선택 해제
    public void OnPointerClick(PointerEventData eventData)
    {
        if(!DualManager.isDragging && !DualManager.isSequenceRunning)
        {
            if(isPlayerCard)
            {
                if(isSelected)
                    Deselect();
                else
                    Select();
            }
            
            else
            {
                HandRanking handRanking = transform.parent.parent.GetComponent<PokerManager>().handRanking;
                if(handRanking.gameObject.activeSelf)
                {
                    handRanking.HideHandRanking();
                    transform.parent.parent.GetComponent<PokerManager>().isHandRankingActivated = false;
                }
                else
                {
                    handRanking.ShowHandRanking(); 
                    transform.parent.parent.GetComponent<PokerManager>().isHandRankingActivated = true;
                }
            }
        }
        
    }
}

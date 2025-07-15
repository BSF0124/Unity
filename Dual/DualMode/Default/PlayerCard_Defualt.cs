using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;

public class PlayerCard_Defualt : Card, IPointerClickHandler
{
    public CardStatus cardStatus;

    [HideInInspector] public Status status;
    private Tween posTween;
    private bool isSelected = false;    // 카드 선택 확인

    // 초기 위치와 회전값 설정
    private void OnEnable()
    {
        initalPosition = rectTransform.anchoredPosition;
        if(isSelected)
        {
            initalPosition += new Vector2(0, 50f);
            rectTransform.anchoredPosition = initalPosition;
        }

        initalRotation = rectTransform.rotation;
    }

    // 카드 초기 설정
    public void Init(int cardID)
    {
        cardStatus = new CardStatus(cardID);
        if(DualManager.stage.dualMode.Contains(DualMode.Equality))
        {
            cardStatus.life = 1;
        }

        cardRect.GetComponent<Image>().sprite = CardDataManager.instance.GetCardByID(cardID).cardSprite;
        status = transform.GetChild(0).GetChild(0).GetComponent<Status>();
    }

    // 카드 선택
    public void Select()
    {
        transform.parent.parent.GetComponent<DefaultManager>().SelectCard(this);
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
        transform.parent.parent.GetComponent<DefaultManager>().DeselectCard(this);
    }

    // 카드 파괴 애니메이션
    public IEnumerator DestroyCard()
    {
        AudioManager.instance.PlaySfx(AudioManager.instance.sfxClips_DualMode[3]);
        yield return StartCoroutine(Disappear((int)cardStatus.cardData.cardRarity));

        if(isSelected)
            Deselect();

        transform.parent.GetComponent<Hand>().RemoveItem(this);
        Destroy(gameObject);
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        if(!DualManager.isSequenceRunning && !DualManager.isDragging)
        {
            transform.SetAsLastSibling();

            cardRect.rotation = Quaternion.Euler(Vector3.zero);
            cardRect.localScale = Vector3.one * 1.2f;
            posTween = cardRect.DOAnchorPosY(50, duration);
            status.ShowStatus();
        }

        if(transform.parent.parent.GetComponent<DefaultManager>().minusOneRunning)
        {
            transform.DOScale(Vector3.one*1.1f, duration);
        }
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        if(!DualManager.isSequenceRunning && !DualManager.isDragging)
        {
            transform.parent.GetComponent<Hand>().SetObjectSibling();
            posTween.Kill();
            cardRect.rotation = initalRotation;
            cardRect.localScale = Vector3.one;
            cardRect.anchoredPosition = Vector2.zero;
            status.HideStatus();
        }

        if(transform.parent.parent.GetComponent<DefaultManager>().minusOneRunning)
        {
            transform.DOScale(Vector3.one, duration);
        }
    }

    // 드래그 시작 위치 저장
    public override void OnBeginDrag(PointerEventData eventData)
    {
        if(!DualManager.isSequenceRunning)
        {
            DualManager.isDragging = true;

            transform.parent.GetComponent<Hand>().SetObjectSibling();
            posTween.Kill();
            cardRect.rotation = initalRotation;
            cardRect.localScale = Vector3.one;
            cardRect.anchoredPosition = Vector2.zero;
            status.HideStatus();

            RectTransformUtility.ScreenPointToLocalPointInRectangle(DualManager.canvas.transform as RectTransform, eventData.position, eventData.pressEventCamera, out var localPointerPosition);
            dragOffset = localPointerPosition - rectTransform.anchoredPosition;
        }
    }

    // 카드를 드래그한 위치로 이동
    public override void OnDrag(PointerEventData eventData)
    {
        if(!DualManager.isSequenceRunning)
        {
            cardRect.localScale = Vector3.one;
            transform.DORotate(Vector3.zero, duration);
            Vector2 mousePosition;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(DualManager.canvas.transform as RectTransform, eventData.position, eventData.pressEventCamera, out mousePosition);
            rectTransform.anchoredPosition = mousePosition - dragOffset;
        }
    }

    // 드래그 종료 후 초기 위치로 이동
    public override void OnEndDrag(PointerEventData eventData)
    {
        if(!DualManager.isSequenceRunning)
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
    }

    // 카드 선택 / 선택 해제
    public void OnPointerClick(PointerEventData eventData)
    {
        if(!DualManager.isDragging && !DualManager.isSequenceRunning)
        {
            if(isSelected)
                Deselect();
                
            else
                Select();
        }

        if(transform.parent.parent.GetComponent<DefaultManager>().minusOneRunning)
        {
            AudioManager.instance.PlaySfx(AudioManager.instance.sfxClips_Main[2]);
            var defaultManager = transform.parent.parent.GetComponent<DefaultManager>();
        
            // moCoroutine이 실행 중인지 확인 후 중지
            if (defaultManager.moCoroutine != null)
            {
                defaultManager.StopCoroutine(defaultManager.moCoroutine);
            }

            // 선택한 카드를 설정하고 Minus_One_Result 코루틴 시작
            defaultManager.selectedCard = this;
            defaultManager.moCoroutine = defaultManager.StartCoroutine(defaultManager.Minus_One_Result());
        }
    }
}

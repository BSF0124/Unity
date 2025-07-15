using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;
using System;

public class Tracker : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    public Transform verticalLayoutGroup;
    public GameObject trackerListPrefab;
    public TrackerCardImage trackerCardImage;
    public float duration = 0.2f;

    [HideInInspector] public float space;
    [HideInInspector] public float cardHeight;
    public Vector2 initalPosition = new Vector2(-790f, 100f);

    private RectTransform rectTransform;
    private Vector2 dragOffset;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        space = verticalLayoutGroup.GetComponent<VerticalLayoutGroup>().spacing;
        cardHeight = trackerListPrefab.GetComponent<RectTransform>().sizeDelta.y;
    }

    // 드래그 시작 위치 저장
    public void OnBeginDrag(PointerEventData eventData)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(DualManager.canvas.transform as RectTransform, eventData.position, eventData.pressEventCamera, out var localPointerPosition);
        dragOffset = localPointerPosition - rectTransform.anchoredPosition;
    }

    // 트래커를 드래그한 위치로 이동
    public void OnDrag(PointerEventData eventData)
    {
        Vector2 mousePosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(DualManager.canvas.transform as RectTransform, eventData.position, eventData.pressEventCamera, out mousePosition);
        rectTransform.anchoredPosition = mousePosition - dragOffset;
    }

    // 트래커가 화면 밖으로 나갈 경우, 화면 안쪽으로 배치되도록 보정
    public void OnEndDrag(PointerEventData eventData)
    {
        Vector2 newPosition = rectTransform.anchoredPosition;

        Vector3 minPosition = new Vector2(-960 + rectTransform.sizeDelta.x/2, -540 + rectTransform.sizeDelta.y/2);
        Vector3 maxPosition = new Vector2(960 - rectTransform.sizeDelta.x/2, 540 - rectTransform.sizeDelta.y/2);

        newPosition.x = Math.Clamp(newPosition.x, minPosition.x, maxPosition.x);
        newPosition.y = Math.Clamp(newPosition.y, minPosition.y, maxPosition.y);

        rectTransform.anchoredPosition = newPosition;
        initalPosition = rectTransform.anchoredPosition;
    }

    // 트래커 활성화
    public void ShowTracker()
    {
        DualManager.isSequenceRunning = true;
        gameObject.SetActive(true);
        rectTransform.DOAnchorPos(initalPosition, duration)
        .OnComplete(()=> DualManager.isSequenceRunning = false);
    }

    // 트래커 비활성화
    public void HideTracker()
    {
        DualManager.isSequenceRunning = true;

        float posX = rectTransform.anchoredPosition.x >=  0? 1400 : -1400;

        rectTransform.DOAnchorPosX(posX, duration)
        .OnComplete(()=> 
        {
            gameObject.SetActive(false);
            DualManager.isSequenceRunning = false;
        }
        );
    }

    // 아이템 추가
    public void AddItem(CardStatus cardStatus)
    {
        GameObject temp = Instantiate(trackerListPrefab, verticalLayoutGroup);
        TrackerList trackerCard = temp.GetComponent<TrackerList>();
        trackerCard.Init(cardStatus);
        SetHeight();
    }

    // 아이템 삭제
    public void SetGrayTrackerList(int index)
    {
        verticalLayoutGroup.GetChild(index).GetComponent<TrackerList>().SetGrayScale();
    }

    // 트래커 높이 조절
    private void SetHeight()
    {
        int count = verticalLayoutGroup.childCount;
        float height;
        height = count>=10? (cardHeight+space) * 10 - space : (cardHeight+space) * count - space;
        rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, height+100f);

        RectTransform scrollView = transform.GetChild(0).GetComponent<RectTransform>();
        scrollView.sizeDelta = new Vector2(rectTransform.sizeDelta.x, height);
    }

    public void CheckCardReveal(int index)
    {
        TrackerList trackerList = verticalLayoutGroup.GetChild(index).GetComponent<TrackerList>();
        if(!trackerList.cardReveal)
            trackerList.CardReveal();
    }
}

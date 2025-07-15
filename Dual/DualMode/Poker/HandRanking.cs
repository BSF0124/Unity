using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using TMPro;

public enum PokerHand
{
    None = -1,
    TwoPair,
    Triple,
    Flush,
    FullHouse,
    FourOfAKind,
    FiveOfAKind,
    FlushHouse,
    FlushFive,
}

public class HandRanking : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    public float duration = 0.2f;
    public Vector2 initalPosition = new Vector2(-710f, 351.65f);

    private RectTransform rectTransform;
    private Vector2 dragOffset;

    private bool isMoving = false;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    // 드래그 시작 위치 저장
    public void OnBeginDrag(PointerEventData eventData)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(DualManager.canvas.transform as RectTransform, eventData.position, eventData.pressEventCamera, out var localPointerPosition);
        dragOffset = localPointerPosition - rectTransform.anchoredPosition;
    }

    // 드래그한 위치로 이동
    public void OnDrag(PointerEventData eventData)
    {
        Vector2 mousePosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(DualManager.canvas.transform as RectTransform, eventData.position, eventData.pressEventCamera, out mousePosition);
        rectTransform.anchoredPosition = mousePosition - dragOffset;
    }

    // 화면 밖으로 나갈 경우, 화면 안쪽으로 배치되도록 보정
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

    // 활성화
    public void ShowHandRanking()
    {
        if(!isMoving)
        {
            isMoving = true;
            gameObject.SetActive(true);
            rectTransform.DOAnchorPos(initalPosition, duration)
                .OnComplete(()=> 
                {
                    DualManager.isSequenceRunning = false;
                    isMoving = false;
                });
        }
    }

    // 비활성화
    public void HideHandRanking()
    {
        if(!isMoving)
        {
            isMoving = true;
            float posX = rectTransform.anchoredPosition.x >=  0? 1400 : -1400;
            rectTransform.DOAnchorPosX(posX, duration)
            .OnComplete(()=> 
            {
                gameObject.SetActive(false);
                isMoving = false;
            }
            );
        }
    }

    //
    public PokerHand CalculateBestPokerHand(List<CardData> communityCards, List<CardData> playerCards)
    {
        var allCards = new List<CardData>(communityCards);
        allCards.AddRange(playerCards);

        if(allCards.Count >= 5)
        {
            return GetBestPokerHandFromCards(allCards);
        }
        else
        {
            return PokerHand.None;
        }

    }

    private PokerHand GetBestPokerHandFromCards(List<CardData> cards)
    {
        List<List<CardData>> combinations = new List<List<CardData>>();

        if(cards.Count > 5)
        {
            for(int  i = 0; i < cards.Count; i++)
            {
                List<CardData> combination = new List<CardData>(cards);
                combination.RemoveAt(i);
                combinations.Add(combination);
            }
        }
        else
        {
            combinations.Add(cards);
        }

        PokerHand bestPokerHand = PokerHand.None;

        // 모든 5장 조합을 확인
        foreach (var pokerHand in combinations)
        {
            var currentPokerHand = EvaluatePokerHand(pokerHand);
            if (currentPokerHand > bestPokerHand)
            {
                bestPokerHand = currentPokerHand;
            }
        }

        return bestPokerHand;
    }
    
    // 족보 계산
    private PokerHand EvaluatePokerHand(List<CardData> hand)
    {
        bool isFlush = hand.All(card => card.cardRarity == hand[0].cardRarity);

        var typeGroups = hand.GroupBy(c => c.cardType).OrderByDescending(g => g.Count()).ThenByDescending(g => g.Key).ToList();

        int maxGroupCount = typeGroups[0].Count();
        int secondGroupCount = typeGroups.Count > 1 ? typeGroups[1].Count() : 0;

        // 플러시 파이브 체크
        if(isFlush && maxGroupCount == 5)
            return PokerHand.FlushFive;

        // 플러시 하우스 체크
        if(isFlush && maxGroupCount == 3 && secondGroupCount == 2)
            return PokerHand.FlushHouse;

        // 파이브 카드 체크
        if(maxGroupCount == 5)
            return PokerHand.FiveOfAKind;

        // 포카드 체크
        if (maxGroupCount == 4)
            return PokerHand.FourOfAKind;

        // 풀하우스 체크
        if (maxGroupCount == 3 && secondGroupCount == 2)
            return PokerHand.FullHouse;

        // 플러시 체크
        if (isFlush)
            return PokerHand.Flush;

        // 트리플 체크
        if (maxGroupCount == 3)
            return PokerHand.Triple;

        // 투페어 체크
        if (maxGroupCount == 2 && secondGroupCount == 2)
            return PokerHand.TwoPair;

        return PokerHand.TwoPair;
    }


    public void SetTextColor(int index)
    {
        for(int i = 0; i < 8; i++)
        {
            if(index == i)
            {
                transform.GetChild(0).GetChild(i).GetComponent<TextMeshProUGUI>().color = Color.white;
            }
            else
            {
                transform.GetChild(0).GetChild(i).GetComponent<TextMeshProUGUI>().color = new Color(0.7f,0.7f,0.7f);
            }
        }
    }
}
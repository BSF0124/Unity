using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Hand : MonoBehaviour
{
    // 카드 프리팹
    public GameObject playerCardPrefab;

    // 카드 오브젝트 리스트
    public List<PlayerCard_Defualt> cardObjects = new List<PlayerCard_Defualt>();
    // 표시할 카드 인덱스
    [HideInInspector] public int currentIndex = -1;
    private RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = transform.GetComponent<RectTransform>();
    }

    // 카드 오브젝트 생성
    public void CreateCards(int name)
    {
        GameObject card = Instantiate(playerCardPrefab, transform);
        card.name = name.ToString();
        cardObjects.Add(card.GetComponent<PlayerCard_Defualt>());
        card.SetActive(false);
    }

    // 손패 교체
    public IEnumerator ChangeHand()
    {
        DualManager.isSequenceRunning = true;
        DualManager.isDragging = false;

        if(transform.parent.GetComponent<DefaultManager>().selectedCard != null)
            transform.parent.GetComponent<DefaultManager>().selectedCard.Deselect();

        HideHand();
        yield return new WaitForSeconds(0.5f);

        SetHand();
        foreach(Transform item in transform)
        {
            PlayerCard_Defualt card = item.GetComponent<PlayerCard_Defualt>();
            card.cardRect.rotation = card.initalRotation;
            card.cardRect.localScale = Vector3.one;
            card.cardRect.anchoredPosition = Vector2.zero;
            card.status.HideStatus();
        }

        ShowHand();
        yield return new WaitForSeconds(0.5f);
        DualManager.isSequenceRunning = false;
    }

    // 손패 설정
    public void SetHand()
    {
        // 현재 인덱스가 마지막이 아닌 경우
        if(currentIndex < (cardObjects.Count-1)/5)
        {
            // 모든 카드 오브젝트 비활성화
            foreach(Transform child in transform)
            {child.gameObject.SetActive(false);}

            currentIndex++;

            // currentIndex 보정
            if(currentIndex < 0)
                currentIndex = 0;

            // 다음으로 활성화 해야 할 오브젝트의 인덱스 계산
            int temp = cardObjects.Count - 1 - (currentIndex*5);

            // 5장 배치
            if(temp >= 4)
            {
                SetCards(5, 
                    new Vector2[] { 
                        new Vector2(-380, -30f),
                        new Vector2(-190, -7.5f),
                        new Vector2(0, 0),
                        new Vector2(190, -7.5f),
                        new Vector2(380, -30)},
                    new float[] { 10, 5, 0, -5, -10 });
            }

            // 5장 이하 배치
            else
            {
                switch(temp)
                {
                    case 0:
                        SetCard(currentIndex*5, new Vector2(0, 0), 0);
                        break;

                    case 1:
                        SetCards(2,
                            new Vector2[] { 
                                new Vector2(-100, 0), 
                                new Vector2(100, 0)},
                            new float[] {7.5f, -7.5f});
                        break;

                    case 2:
                        SetCards(3,
                            new Vector2[] { 
                                new Vector2(-200, -7.5f), 
                                new Vector2(0, 0), 
                                new Vector2(200, -7.5f)},
                            new float[] {5, 0, -5});
                        break;

                    case 3:
                        SetCards(4,
                            new Vector2[] { 
                                new Vector2(-300, -45), 
                                new Vector2(-100, 0), 
                                new Vector2(100, 0), 
                                new Vector2(300, -45)},
                            new float[] {15, 7.5f, -7.5f, -15});
                        break;
                }
            }
        }

        // 현재 인덱스가 마지막인 경우
        else
        {
            // 핸드에 1장만 남은 경우
            if(cardObjects.Count == 1)
            {
                cardObjects[0].GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                cardObjects[0].transform.Rotate(0, 0, 0);
                cardObjects[0].gameObject.SetActive(true);
            }

            // 1장이 아닌 경우
            else
            {
                currentIndex = -1;
                SetHand();
            }
        }
    }

    // 카드 위치 설정
    private void SetCards(int count, Vector2[] positions, float[] rotations)
    {
        for (int i = 0; i < count; i++)
        {
            SetCard(currentIndex * 5 + i, positions[i], rotations[i]);
        }
    }
    private void SetCard(int index, Vector2 position, float rotation)
    {
        cardObjects[index].GetComponent<RectTransform>().anchoredPosition = position;
        cardObjects[index].transform.rotation = Quaternion.Euler(0, 0, rotation);
        cardObjects[index].gameObject.SetActive(true);
    }

    // 핸드 표시
    public void ShowHand()
    {
        rectTransform.DOAnchorPosY(-340, 0.5f);
    }

    // 핸드 숨김
    public void HideHand()
    {
        rectTransform.DOAnchorPosY(-720, 0.5f);
    }
    
    // 카드 오브젝트 순서 초기화
    public void SetObjectSibling()
    {
        int i = 0;
        foreach(PlayerCard_Defualt card in cardObjects)
        {
            card.transform.SetSiblingIndex(i);
            i++;
        }
    }

    // Null 아이템 삭제
    public void RemoveItem(PlayerCard_Defualt card)
    {
        cardObjects.Remove(card);
    }
}

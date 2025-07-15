using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Hand_Poker : MonoBehaviour
{
    // 포커 카드 프리팹
    public GameObject pokerCardPrefab;

    // 카드 오브젝트 리스트
    public List<PokerCard> cardObjects = new List<PokerCard>();

    private RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = transform.GetComponent<RectTransform>();
    }

    // 카드 오브젝트 생성
    public void CreateCards(int index)
    {
        GameObject card = Instantiate(pokerCardPrefab, transform);
        card.SetActive(false);
        card.name = index.ToString();
        cardObjects.Add(card.GetComponent<PokerCard>());

        if(DualManager.playerDeckList[index] == 11)
        {
            int id;
            do{id = Random.Range(0, 30);}
            while(id == 11);
            card.GetComponent<PokerCard>().Init(id);
        }
        else
            card.GetComponent<PokerCard>().Init(DualManager.playerDeckList[index]);
    }

    // 손패 교체
    public IEnumerator ChangeHand()
    {
        DualManager.isSequenceRunning = true;

        HideHand();
        yield return new WaitForSeconds(0.5f);
        ShuffleObjects();
        SetObjectSibling();

        SetHand();

        ShowHand();
        yield return new WaitForSeconds(0.5f);
        DualManager.isSequenceRunning = false;
    }

    // 손패 설정
    public void SetHand()
    {
        foreach(Transform child in transform)
        {child.gameObject.SetActive(false);}
        
        if(cardObjects.Count >= 5)
        {
            SetCards(5, 
                    new Vector2[] {new Vector2(-380, -30f),new Vector2(-190, -7.5f),new Vector2(0, 0),new Vector2(190, -7.5f),new Vector2(380, -30)},
                    new float[] { 10, 5, 0, -5, -10 });
        }
        else
        {
            switch(cardObjects.Count)
            {
                case 4:
                    SetCards(4,
                            new Vector2[] {new Vector2(-300, -45),new Vector2(-100, 0),new Vector2(100, 0),new Vector2(300, -45)},
                            new float[] {15, 7.5f, -7.5f, -15});
                    break;
                case 3:
                    SetCards(3,
                        new Vector2[] {new Vector2(-200, -7.5f),new Vector2(0, 0),new Vector2(200, -7.5f)},
                        new float[] {5, 0, -5});
                    break;
                case 2:
                    SetCards(2,
                        new Vector2[] {new Vector2(-100, 0),new Vector2(100, 0)},
                        new float[] {7.5f, -7.5f});
                    break;
                case 1:
                    SetCard(0, new Vector2(0, 0), 0);
                    break;
            }
        }
    }

    // 카드 위치 설정
    private void SetCards(int count, Vector2[] positions, float[] rotations)
    {
        for (int i = 0; i < count; i++)
        {
            SetCard(i, positions[i], rotations[i]);
        }
    }
    private void SetCard(int index, Vector2 position, float rotation)
    {
        transform.GetChild(index).GetComponent<RectTransform>().anchoredPosition = position;
        transform.GetChild(index).transform.rotation = Quaternion.Euler(0, 0, rotation);
        transform.GetChild(index).gameObject.SetActive(true);
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
        foreach(PokerCard card in cardObjects)
        {
            card.transform.SetSiblingIndex(i);
            i++;
        }
    }

    // 오브젝트 순서 섞기
    public void ShuffleObjects()
    {
        for(int i = 0; i < cardObjects.Count; i++)
        {
            int index = Random.Range(0, cardObjects.Count);
            PokerCard temp = cardObjects[i];
            cardObjects[i] = cardObjects[index];
            cardObjects[index] = temp;
        }
    }
}

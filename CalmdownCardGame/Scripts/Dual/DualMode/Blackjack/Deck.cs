using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Deck : MonoBehaviour
{
    public GameObject blackjackcardPrefab;          // 블랙잭 카드 프리팹
    public List<int> deckList = new List<int>();    // 덱에 남은 카드 리스트
    private BlackjackManager blackjackManager;      // 블랙잭 매니저
    private float duration = 0.3f;

    // 블랙잭 매니저 불러오기 및 덱 생성
    private void Start()
    {
        blackjackManager = transform.parent.GetComponent<BlackjackManager>();
        for(int i = 0; i < 30; i++)
        {
            deckList.Add(i);
        }
        deckList.Remove(11);
    }

    // 새로운 덱 생성
    private void CreateNewDeckList()
    {
        for(int i = 0; i < 30; i++)
        {
            deckList.Add(i);
        }
        deckList.Remove(11);
        
        transform.GetChild(1).gameObject.SetActive(true);
        AudioManager.instance.PlaySfx(AudioManager.instance.sfxClips_DualMode[12]);
        GetComponent<RectTransform>().DOShakeAnchorPos(0.5f, 30, 30);
    }

    // 히트
    public IEnumerator Hit(bool player)
    {
        
        DualManager.isSequenceRunning = true;
        // 카드가 이동할 x 좌표
        float pos_X = player ? blackjackManager.player.transform.childCount * 109f : blackjackManager.dealer.transform.childCount * 109f;
        
        // 덱에 남은 카드가 없다면 새로운 덱 생성
        if(deckList.Count <= 0)
        {
            CreateNewDeckList();
        }

        // 덱 리스트에서 무작위 카드를 선택
        int cardID = deckList[Random.Range(0,deckList.Count)];
        deckList.Remove(cardID);

        AudioManager.instance.PlaySfx(AudioManager.instance.sfxClips_Main[8]);
        // 가지고 있는 카드들을 왼쪽으로 이동시킴
        if(player)
        {
            yield return StartCoroutine(blackjackManager.player.CardMove());
        }
        else
        {
            yield return StartCoroutine(blackjackManager.dealer.CardMove());
        }

        // 블랙잭 카드 생성 및 이동
        GameObject card = Instantiate(blackjackcardPrefab, blackjackManager.transform); // 생성
        BlackjackCard blackjackCard = card.GetComponent<BlackjackCard>();   // 컴포넌트 불러오기
        blackjackCard.rectTransform.anchoredPosition = new Vector2(556.41f,0);  // 초기 위치 설정
        blackjackCard.cardID = cardID;  // 카드 ID 값 설정
        //blackjackCard.index = player ? blackjackManager.player.transform.childCount : blackjackManager.dealer.transform.childCount; // SiblingIndex 값 설정
        if(player)
        {
            blackjackCard.transform.SetParent(blackjackManager.player.transform);
        }
        else
        {
            blackjackCard.transform.SetParent(blackjackManager.dealer.transform);
        }

        // 딜러의 2번째 카드를 제외한 카드는 뒤집음
        if((!player && blackjackManager.dealer.transform.childCount != 2) || player)
        {
            StartCoroutine(blackjackCard.Flip(CardDataManager.instance.GetCardByID(cardID).cardSprite));
        }
        yield return blackjackCard.rectTransform.DOAnchorPos(new Vector2(pos_X, 0), duration)
        .OnComplete(()=> 
            blackjackCard.initalPosition = blackjackCard.rectTransform.anchoredPosition
        ).WaitForCompletion();
    
        // 점수 계산
        if(player)
            blackjackManager.player.CalculateScore();
        else
            blackjackManager.dealer.CalculateScore();
    }
}
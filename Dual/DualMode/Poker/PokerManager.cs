using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using Unity.VisualScripting;

public class PokerManager : MonoBehaviour
{
    public Button dualButton;           // 듀얼 버튼
    public HandRanking handRanking;     // 트래커
    public EnemyCard_Poker enemyCard;   // 적 카드 오브젝트
    public CommunityCard communityCard;
    public Hand_Poker hand;             // 손패 구역
    public Transform field;             // 필드 구역

    [HideInInspector] public List<PokerCard> selectCards = new List<PokerCard>();   // 플레이어가 선택한 카드
    [HideInInspector] public bool isHandRankingActivated = false;   // 핸드 랭킹 On/Off 여부

    // 적 카드 인덱스
    private List<int> enemyIndex = new List<int>();

    // 포커핸드
    private PokerHand playerPokerHand;
    private PokerHand enemyPokerHand;

    private string[] pokerHandTexts = {
        "투 페어",
        "트리플",
        "플러시",
        "풀 하우스",
        "포 카드",
        "파이브 카드",
        "플러시 하우스",
        "플러시 파이브"};

    private void Start()
    {
        dualButton.transform.localScale = Vector3.zero;
        enemyCard.transform.localScale = Vector3.zero;

        Init();
        StartCoroutine(StartAnimation());
        handRanking.SetTextColor(-1);
    }

    private void Update()
    {
        // 우클릭으로 선택한 카드 취소
        if(!DualManager.isSequenceRunning && Input.GetMouseButtonDown(1))
        {
            for(int i = selectCards.Count - 1; i >= 0; i--)
            {
                selectCards[i].Deselect();
            }
        }
    }

    private void Init()
    {
        // 플레이어 카드 리스트 추가
        for(int i=0; i<DualManager.playerDeckList.Count; i++)
        {
            hand.CreateCards(i);
        }

        // 적 카드 리스트 추가
        for(int i=0; i<DualManager.enemyDeckList.Count; i++)
        {
            enemyCard.enemyDatas.Add(new Enemy(DualManager.enemyDeckList[i]));
            if(DualManager.stage.dualMode.Contains(DualMode.Equality))
            {
                enemyCard.enemyDatas[i].life = 1;
            };
        }
        handRanking.gameObject.SetActive(false);
    }

    // 플레이어 카드 선택
    public void SelectCard(PokerCard card)
    {
        if(!selectCards.Contains(card))
        {
            if(selectCards.Count == 2)
            {
                selectCards[0].Deselect();
            }
            selectCards.Add(card);

            if(!DualManager.isSequenceRunning)
            {
                List<CardData> playerCards = new List<CardData>();
                foreach(var item in selectCards)
                {
                    playerCards.Add(item.cardData);
                }
                playerPokerHand = handRanking.CalculateBestPokerHand(communityCard.comunityCards, playerCards);
                handRanking.SetTextColor((int)playerPokerHand);
            }
        }
        DualButtonChange();
    }

    // 플레이어 카드 선택 취소
    public void DeselectCard(PokerCard card)
    {
        selectCards.Remove(card);
        DualButtonChange();
        
        if(!DualManager.isSequenceRunning)
        {
            List<CardData> playerCards = new List<CardData>();
            foreach(var item in selectCards)
            {
                playerCards.Add(item.cardData);
            }
            playerPokerHand = handRanking.CalculateBestPokerHand(communityCard.comunityCards, playerCards);
            handRanking.SetTextColor((int)playerPokerHand);
        }
    }

    // 듀얼 버튼 변경
    private void DualButtonChange()
    {
        if(selectCards.Count > 0)
        {
            dualButton.interactable = true;
            dualButton.GetComponent<Image>().color = new Color(0.95f, 0.28f, 0.13f);
            dualButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().fontSize = 60f;
            dualButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "듀얼";
            dualButton.transform.DOShakePosition(0.3f, 15, 15)
                .OnComplete(()=>dualButton.GetComponent<RectTransform>().anchoredPosition3D = Vector3.zero);
        }

        else
        {
            dualButton.interactable = false;
            dualButton.GetComponent<Image>().color = Color.gray;
            dualButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().fontSize = 40f;
            dualButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = $"카드선택\n(최대 2장)";
        }
    }

    // 듀얼 버튼 클릭
    public void DualButton()
    {
        if(!DualManager.isSequenceRunning)
        {
            DualManager.isSequenceRunning = true;

            if(selectCards.Count <= 0)
                return;

            enemyIndex.Clear();
            // 적 카드 선택
            if(enemyCard.enemyDatas.Count == 1)
                enemyIndex.Add(0);
            else
            {
                enemyIndex.Add(Random.Range(0, enemyCard.enemyDatas.Count));
                
                int index;
                do{index = Random.Range(0, enemyCard.enemyDatas.Count);}
                while(enemyIndex[0] == index);
                enemyIndex.Add(index);

                if(enemyIndex[0] > enemyIndex[1])
                {
                    int temp = enemyIndex[0];
                    enemyIndex[0] = enemyIndex[1];
                    enemyIndex[1] = temp;
                }
            }
            List<CardData> enemyCards = new List<CardData>();
            foreach(var item in enemyIndex)
            {
                enemyCards.Add(enemyCard.enemyDatas[item].cardData);
            }
            enemyPokerHand = handRanking.CalculateBestPokerHand(communityCard.comunityCards, enemyCards);
            
            StartCoroutine(Dual());
        }
    }

    // 애니메이션

    // 듀얼 시작 애니메이션
    private IEnumerator StartAnimation()
    {   
        DualManager.isSequenceRunning = true;

        Tween buttonTween = dualButton.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);
        Tween cardTween = enemyCard.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);
        yield return new WaitWhile(() =>
            buttonTween.IsActive() && !buttonTween.IsComplete() ||
            cardTween.IsActive() && !cardTween.IsComplete()
        );

        yield return StartCoroutine(hand.ChangeHand());
        DualButtonChange();
        yield return new WaitForSeconds(0.5f);
        yield return communityCard.CreateComunityCards();
        DualManager.isSequenceRunning = false;
    }

    // 듀얼 애니메이션
    IEnumerator Dual()
    {
        yield return StartCoroutine(HideInterface(isHandRankingActivated));

        // 플레이어 카드 이동
        if(selectCards.Count == 1)
        {
            selectCards[0].transform.SetParent(field);
            selectCards[0].transform.SetAsFirstSibling();
            yield return selectCards[0].rectTransform.DOAnchorPos(Vector2.zero, 0.5f);
            yield return selectCards[0].rectTransform.DORotate(Vector2.zero, 0.5f).WaitForCompletion();
        }
        else
        {
            selectCards[0].transform.SetParent(field);
            selectCards[1].transform.SetParent(field);
            selectCards[0].transform.SetAsFirstSibling();
            selectCards[1].transform.SetSiblingIndex(1);
            yield return selectCards[0].rectTransform.DOAnchorPos(new Vector2(-200f, 0), 0.5f);
            yield return selectCards[0].rectTransform.DORotate(Vector2.zero, 0.5f);
            yield return selectCards[1].rectTransform.DOAnchorPos(new Vector2(200f, 0), 0.5f);
            yield return selectCards[1].rectTransform.DORotate(Vector2.zero, 0.5f).WaitForCompletion();
        }

        // 적 카드 이동 및 공개
        if(enemyIndex.Count == 1)
        {
            enemyCard.transform.GetChild(0).GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            yield return enemyCard.transform.GetChild(0).DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack).WaitForCompletion();
            yield return StartCoroutine(enemyCard.Flip_Poker_1(enemyCard.enemyDatas[enemyIndex[0]].cardData.cardSprite));
        }
        else
        {
            enemyCard.transform.GetChild(0).GetComponent<RectTransform>().anchoredPosition = new Vector2(-200, 0);
            enemyCard.transform.GetChild(1).GetComponent<RectTransform>().anchoredPosition = new Vector2(200, 0);
            
            yield return enemyCard.transform.GetChild(0).DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);
            yield return enemyCard.transform.GetChild(1).DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack).WaitForCompletion();

            StartCoroutine(enemyCard.Flip_Poker_1(enemyCard.enemyDatas[enemyIndex[0]].cardData.cardSprite));
            yield return StartCoroutine(enemyCard.Flip_Poker_2(enemyCard.enemyDatas[enemyIndex[1]].cardData.cardSprite));
        }

        // 스테이터스 공개
        AudioManager.instance.PlaySfx(AudioManager.instance.sfxClips_CutScene[0]);
        selectCards[0].status.ShowStatus();
        if(selectCards.Count > 1)
        {
            selectCards[1].status.ShowStatus();
        }
        Status_Poker status_1 = enemyCard.transform.GetChild(0).GetChild(0).GetComponent<Status_Poker>();
        status_1.cardData = enemyCard.enemyDatas[enemyIndex[0]].cardData;
        status_1.life = enemyCard.enemyDatas[enemyIndex[0]].life;
        status_1.ShowStatus();

        if(enemyIndex.Count > 1)
        {
            Status_Poker status_2 = enemyCard.transform.GetChild(1).GetChild(0).GetComponent<Status_Poker>();
            status_2.cardData = enemyCard.enemyDatas[enemyIndex[1]].cardData;
            status_2.life = enemyCard.enemyDatas[enemyIndex[1]].life;
            status_2.ShowStatus();
        }
        yield return new WaitForSeconds(1f);

        // 포커 핸드 텍스트 표시
        AudioManager.instance.PlaySfx(AudioManager.instance.sfxClips_Main[8]);
        field.GetChild(field.childCount-1).GetComponent<TextMeshProUGUI>().text = pokerHandTexts[(int)playerPokerHand];
        enemyCard.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = pokerHandTexts[(int)enemyPokerHand];
        field.GetChild(field.childCount-1).gameObject.SetActive(true);
        enemyCard.transform.GetChild(2).gameObject.SetActive(true);
        yield return new WaitForSeconds(1f);

        // 승리
        if((int)playerPokerHand > (int)enemyPokerHand)
        {
            AudioManager.instance.PlaySfx(AudioManager.instance.sfxClips_DualMode[12]);
            field.GetChild(field.childCount-1).GetComponent<TextMeshProUGUI>().text = "승리!";
            yield return field.GetChild(field.childCount-1).GetComponent<RectTransform>().DOShakeAnchorPos(0.5f, 30, 30);
            enemyCard.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = "패배..";
            yield return enemyCard.transform.GetChild(2).GetComponent<RectTransform>().DOShakeAnchorPos(0.5f, 30, 30).WaitForCompletion();
            yield return new WaitForSeconds(1f);

            StartCoroutine(Win());
            
            // 스테이터스 업데이트
            enemyCard.enemyDatas[enemyIndex[0]].life--;
            status_1.life = enemyCard.enemyDatas[enemyIndex[0]].life;
            status_1.StatusUpdate();
            if(enemyIndex.Count > 1)
            {
                enemyCard.enemyDatas[enemyIndex[1]].life--;
                enemyCard.transform.GetChild(1).GetChild(0).GetComponent<Status_Poker>().life = enemyCard.enemyDatas[enemyIndex[1]].life;
                enemyCard.transform.GetChild(1).GetChild(0).GetComponent<Status_Poker>().StatusUpdate();
            }
        }
        // 패배
        else if((int)playerPokerHand < (int)enemyPokerHand)
        {
            AudioManager.instance.PlaySfx(AudioManager.instance.sfxClips_DualMode[12]);
            field.GetChild(field.childCount-1).GetComponent<TextMeshProUGUI>().text = "패배..";
            yield return field.GetChild(field.childCount-1).GetComponent<RectTransform>().DOShakeAnchorPos(0.5f, 30, 30);
            enemyCard.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = "승리!";
            yield return enemyCard.transform.GetChild(2).GetComponent<RectTransform>().DOShakeAnchorPos(0.5f, 30, 30).WaitForCompletion();
            yield return new WaitForSeconds(1f);

            StartCoroutine(Lose());
            
            // 스테이터스 업데이트
            selectCards[0].life--;
            selectCards[0].status.StatusUpdate();
            if(selectCards.Count > 1)
            {
                selectCards[1].life--;
                selectCards[1].status.StatusUpdate();
            }
        }
        // 무승부
        else
        {
            AudioManager.instance.PlaySfx(AudioManager.instance.sfxClips_DualMode[12]);
            field.GetChild(field.childCount-1).GetComponent<TextMeshProUGUI>().text = "무승부";
            yield return field.GetChild(field.childCount-1).GetComponent<RectTransform>().DOShakeAnchorPos(0.5f, 30, 30);
            enemyCard.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = "무승부";
            yield return enemyCard.transform.GetChild(2).GetComponent<RectTransform>().DOShakeAnchorPos(0.5f, 30, 30).WaitForCompletion();
            yield return new WaitForSeconds(1f);

            StartCoroutine(Draw());
            
            // 스테이터스 업데이트
            selectCards[0].life--;
            selectCards[0].status.StatusUpdate();
            if(selectCards.Count > 1)
            {
                selectCards[1].life--;
                selectCards[1].status.StatusUpdate();
            }

            enemyCard.enemyDatas[enemyIndex[0]].life--;
            status_1.life = enemyCard.enemyDatas[enemyIndex[0]].life;
            status_1.StatusUpdate();
            if(enemyIndex.Count > 1)
            {
                enemyCard.enemyDatas[enemyIndex[1]].life--;
                enemyCard.transform.GetChild(1).GetChild(0).GetComponent<Status_Poker>().life = enemyCard.enemyDatas[enemyIndex[1]].life;
                enemyCard.transform.GetChild(1).GetChild(0).GetComponent<Status_Poker>().StatusUpdate();
            }
        }
        yield return new WaitForSeconds(1f);

        // 스테이터스 숨김
        selectCards[0].status.HideStatusForPokerManager();
        if(selectCards.Count > 1)
            selectCards[1].status.HideStatusForPokerManager();
        status_1.HideStatusForPokerManager();
        enemyCard.transform.GetChild(1).GetChild(0).GetComponent<Status_Poker>().HideStatusForPokerManager();
        yield return new WaitForSeconds(1f);

        // 카드 사라짐
        if(selectCards.Count > 1)
        {
            if(selectCards[1].life <= 0)
            {
                hand.cardObjects.Remove(selectCards[1]);
                yield return StartCoroutine(selectCards[1].DestroyCard());
            }
        }
        if(selectCards[0].life <= 0)
        {
            hand.cardObjects.Remove(selectCards[0]);
            yield return StartCoroutine(selectCards[0].DestroyCard());
        }
        if(enemyIndex.Count > 1)
        {
            if(enemyCard.enemyDatas[enemyIndex[1]].life <= 0)
            {
                AudioManager.instance.PlaySfx(AudioManager.instance.sfxClips_DualMode[3]);
                yield return StartCoroutine(enemyCard.Disappear_2((int)enemyCard.enemyDatas[enemyIndex[1]].cardData.cardRarity));
                enemyCard.transform.GetChild(1).localScale = Vector3.zero;
                enemyCard.transform.GetChild(1).GetComponent<Image>().material.SetFloat("_Fade", 1f);
                enemyCard.enemyDatas.RemoveAt(enemyIndex[1]);
            }
        }
        if(enemyCard.enemyDatas[enemyIndex[0]].life <= 0)
        {
            AudioManager.instance.PlaySfx(AudioManager.instance.sfxClips_DualMode[3]);
            yield return StartCoroutine(enemyCard.Disappear_1((int)enemyCard.enemyDatas[enemyIndex[0]].cardData.cardRarity));
            enemyCard.transform.GetChild(0).localScale = Vector3.zero;
            enemyCard.transform.GetChild(0).GetComponent<Image>().material.SetFloat("_Fade", 1f);
            enemyCard.enemyDatas.RemoveAt(enemyIndex[0]);
        }

        // 게임 오버
        if(hand.cardObjects.Count <= 0)
        {
            // 게임 오버(패배)
            transform.parent.GetComponent<DualManager>().GameOver(false);
        }
        else if(enemyCard.enemyDatas.Count <= 0)
        {
            // 게임 오버(승리)
            transform.parent.GetComponent<DualManager>().GameOver(true);
        }
        else
        {
            StartCoroutine(field.GetChild(field.childCount-1).GetComponent<PokerHandText>().TextEffect());
            yield return StartCoroutine(enemyCard.transform.GetChild(2).GetComponent<PokerHandText>().TextEffect());
            yield return new WaitForSeconds(0.5f);
    
            yield return enemyCard.transform.GetChild(1).DOScale(Vector3.zero, 0.5f).SetEase(Ease.InBack);
            yield return enemyCard.transform.GetChild(0).DOScale(Vector3.zero, 0.5f).SetEase(Ease.InBack).WaitForCompletion();
            enemyCard.transform.GetChild(1).GetComponent<RectTransform>().anchoredPosition = new Vector2(200, 0);
            enemyCard.transform.GetChild(0).GetComponent<RectTransform>().anchoredPosition = new Vector2(-200, 0);
            enemyCard.transform.GetChild(1).GetComponent<Image>().sprite = hand.cardObjects[0].backImage;
            enemyCard.transform.GetChild(0).GetComponent<Image>().sprite = hand.cardObjects[0].backImage;
    
            yield return field.GetComponent<RectTransform>().DOAnchorPosY(-710f,0.5f).WaitForCompletion();
            
            for(int i = selectCards.Count - 1; i >= 0; i--)
            {
                selectCards[i].transform.SetParent(hand.transform);
                selectCards[i].Deselect();
            }
            
            
            field.GetComponent<RectTransform>().anchoredPosition = new Vector2(-600, -310f);
            handRanking.SetTextColor(-1);
            yield return StartCoroutine(communityCard.CreateComunityCards());
    
            yield return ShowInterface(isHandRankingActivated);
            DualManager.isSequenceRunning = false;
        }
    }

    // 승리 애니메이션
    IEnumerator Win()
    {
        AudioManager.instance.PlaySfx(AudioManager.instance.sfxClips_DualMode[5]);
        if(PlayerPrefs.GetInt("DualVoice", 1) == 1)
            AudioManager.instance.PlaySfx(AudioManager.instance.sfxClips_DualWin[Random.Range(0,AudioManager.instance.sfxClips_DualWin.Length)]);

        enemyCard.transform.GetChild(0).GetChild(1).gameObject.SetActive(true);
        if(enemyIndex.Count > 1)
        {
            enemyCard.transform.GetChild(1).GetChild(1).gameObject.SetActive(true);
            yield return enemyCard.transform.GetChild(1).GetComponent<RectTransform>().DOPunchAnchorPos(new Vector2(30, 30), 0.5f, 10, 0.5f);
        }
        yield return enemyCard.transform.GetChild(0).GetComponent<RectTransform>().DOPunchAnchorPos(new Vector2(30, 30), 0.5f, 10, 0.5f).WaitForCompletion();

    }

    // 패배 애니메이션
    IEnumerator Lose()
    {
        AudioManager.instance.PlaySfx(AudioManager.instance.sfxClips_DualMode[5]);
        if(PlayerPrefs.GetInt("DualVoice", 1) == 1)
            AudioManager.instance.PlaySfx(AudioManager.instance.sfxClips_DualLose[Random.Range(0,AudioManager.instance.sfxClips_DualLose.Length)]);

        selectCards[0].transform.GetChild(1).gameObject.SetActive(true);
        if(selectCards.Count == 2)
        {
            selectCards[1].transform.GetChild(1).gameObject.SetActive(true);
            yield return selectCards[1].cardImage.GetComponent<RectTransform>().DOPunchAnchorPos(new Vector2(30, 30), 0.5f, 10, 0.5f);
        }
        yield return selectCards[0].cardImage.GetComponent<RectTransform>().DOPunchAnchorPos(new Vector2(30, 30), 0.5f, 10, 0.5f).WaitForCompletion();

        //Tween shakeTween = GetComponent<RectTransform>().DOShakeAnchorPos(0.5f, 30, 30);
    }

    // 모두 패배 애니메이션
    IEnumerator Draw()
    {
        AudioManager.instance.PlaySfx(AudioManager.instance.sfxClips_DualMode[5]);
        if(PlayerPrefs.GetInt("DualVoice", 1) == 1)
            AudioManager.instance.PlaySfx(AudioManager.instance.sfxClips_DualLose[Random.Range(0,AudioManager.instance.sfxClips_DualLose.Length)]);

        foreach(PokerCard card in selectCards)
        {
            card.transform.GetChild(1).gameObject.SetActive(true);
        }
        enemyCard.transform.GetChild(0).GetChild(1).gameObject.SetActive(true);
        if(enemyIndex.Count > 1)
        {
            enemyCard.transform.GetChild(1).GetChild(1).gameObject.SetActive(true);
        }

        yield return GetComponent<RectTransform>().DOShakeAnchorPos(0.5f, 30, 30).WaitForCompletion();

    }
    
    // 인터페이스 숨김
    IEnumerator HideInterface(bool isHandRankingActivated)
    {
        if(!DualManager.isGameOver)
        {
            yield return dualButton.transform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InBack)
                .OnComplete(()=>
                {
                    hand.HideHand();
                }).WaitForCompletion();

            if(isHandRankingActivated)
            {
                RectTransform handRankingRect = handRanking.GetComponent<RectTransform>();
                float posX = handRankingRect.anchoredPosition.x >= 0? 1400 : -1400;
                yield return handRanking.GetComponent<RectTransform>().DOAnchorPosX(posX, 0.2f).WaitForCompletion();
                handRanking.gameObject.SetActive(false);
            }
        }
    }

    // 인터페이스 표시
    IEnumerator ShowInterface(bool isHandRankingActivated)
    {
        if(!DualManager.isGameOver)
        {
            yield return dualButton.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack).WaitForCompletion();
            yield return StartCoroutine(hand.ChangeHand());

            if(isHandRankingActivated)
            {
                handRanking.gameObject.SetActive(true);
                yield return handRanking.GetComponent<RectTransform>().DOAnchorPos(handRanking.initalPosition, 0.2f).WaitForCompletion();;
            }
        }
    }
}

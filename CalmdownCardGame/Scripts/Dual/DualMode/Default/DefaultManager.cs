using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using Unity.VisualScripting;

public class CardStatus
{
    public CardData cardData;
    public int life;
    public List<CardType> ssrLife;

    // 카드 스텟(생명력 관리)
    public CardStatus(int cardID)
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
            case CardRarity.Null:
                life = 1;
                ssrLife = new List<CardType>(){CardType.Rock, CardType.Scissors, CardType.Paper};
                break;
        }
    }
}

public class DefaultManager : MonoBehaviour
{
    public Button dualButton;           // 듀얼 버튼
    public Tracker tracker;             // 트래커
    public EnemyCard_Default enemyCard; // 적 카드 오브젝트
    public Hand hand;                   // 손패 구역
    public Transform field;             // 필드 구역
    public Transform timer;                // 듀얼 버튼

    // 플레이어가 선택한 카드
    [HideInInspector] public PlayerCard_Defualt selectedCard;       // 듀얼에 사용할 내 카드
    [HideInInspector] public List<PlayerCard_Defualt> selectCards;  // 카드를 2장 이상 선택이 가능할 때, 고른 카드
    [HideInInspector] public bool minusOneRunning = false;          // 하나 빼기 실행중 체크(카드 고르기)
    [HideInInspector] public bool isTrackerActivated = false;       // 트래커 On/Off 여부
    [HideInInspector] public Coroutine moCoroutine;

    private int enemyIndex; // 적 카드 인덱스

    // 하나 빼기 적 카드 인덱스
    public int enemyIndex_MO_1;
    public int enemyIndex_MO_2;

    // 중복 금지 기믹 체크 변수
    private List<int> previousePlayerCard = new List<int>();
    private List<int> previouseEnemyCard = new List<int>();

    private void Start()
    {
        dualButton.transform.localScale = Vector3.zero;
        enemyCard.transform.localScale = Vector3.zero;

        Init();
        StartCoroutine(StartAnimation());
    }

    private void Update()
    {
        // 핸드 교체
        if(!DualManager.isGameOver && !DualManager.isSequenceRunning && (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(1)) && hand.cardObjects.Count > 5)
        {
            StartCoroutine(hand.ChangeHand());
        }

        // 우클릭으로 선택한 카드 취소
        if(!DualManager.isGameOver && !DualManager.isSequenceRunning && Input.GetMouseButtonDown(1))
        {
            if(selectCards.Count > 0)
            {
                for(int i = selectCards.Count - 1; i >= 0; i--)
                {
                    selectCards[i].Deselect();
                }
            }
            if(selectedCard != null)
            {
                selectedCard.Deselect();
            }
        }
    }

    public void ChangeHandMethod()
    {
        if(!DualManager.isSequenceRunning && hand.cardObjects.Count > 5)
        {
            StartCoroutine(hand.ChangeHand());
        }
    }

    private void Init()
    {
        // 덱 교체 기믹
        if(DualManager.stage.dualMode.Contains(DualMode.Change))
        {
            List<int> temp = DualManager.playerDeckList;
            DualManager.playerDeckList = DualManager.enemyDeckList;
            DualManager.enemyDeckList = temp;
        }
        // 복제 기믹
        else if(DualManager.stage.dualMode.Contains(DualMode.Replication))
        {
            DualManager.enemyDeckList = DualManager.playerDeckList;
        }
        // 랜덤 기믹
        else if(DualManager.stage.dualMode.Contains(DualMode.Random))
        {
            DualManager.enemyDeckList.Clear();
            for(int i=0; i < DualManager.playerDeckList.Count; i++)
            {
                DualManager.enemyDeckList.Add(Random.Range(0, 30));
            }
        }

        // 플레이어 카드 리스트 추가
        for(int i=0; i<DualManager.playerDeckList.Count; i++)
        {
            hand.CreateCards(i);
            hand.transform.GetChild(i).GetComponent<PlayerCard_Defualt>().Init(DualManager.playerDeckList[i]);
        }

        // 적 카드 리스트 및 트래커 카드 추가
        for(int i=0; i<DualManager.enemyDeckList.Count; i++)
        {
            enemyCard.enemyCards.Add(new CardStatus(DualManager.enemyDeckList[i]));
            if(DualManager.stage.dualMode.Contains(DualMode.Equality))
            {
                enemyCard.enemyCards[i].life = 1;
            };
            tracker.AddItem(enemyCard.enemyCards[i]);
        }
        tracker.gameObject.SetActive(false);
    }

    /// <summary>
    /// 가위바위보 대결 결과를 반환
    /// </summary>
    /// <param name="player">플레이어가 플레이 한 카드</param>
    /// <param name="opponent">적이 플레이 한 카드</param>
    /// <returns>승리: 1, 무승부: 0, 패배: -1, 동시 패배: -2</returns>
    private int Battle(CardStatus player, CardStatus opponent)
    {
        // 플레이어와 상대 카드가 모두 SSR인 경우
        if(player.cardData.cardType == CardType.All && opponent.cardData.cardType == CardType.All)
        {
            if(player.ssrLife.Count == 1 && opponent.ssrLife.Count == 1)
            {
                switch(player.ssrLife[0])
                {
                    case CardType.Rock:
                        if(opponent.ssrLife[0] == CardType.Scissors)
                        {
                            opponent.ssrLife.Remove(CardType.Scissors);
                            return 1;
                        }
                        else if(opponent.ssrLife[0] == CardType.Paper)
                        {
                            player.ssrLife.Remove(CardType.Rock);
                            return -1;
                        }
                        break;

                    case CardType.Paper:
                        if(opponent.ssrLife[0] == CardType.Rock)
                        {
                            opponent.ssrLife.Remove(CardType.Rock);
                            return 1;
                        }
                        else if(opponent.ssrLife[0] == CardType.Scissors)
                        {
                            player.ssrLife.Remove(CardType.Paper);
                            return -1;
                        }
                        break;

                        case CardType.Scissors:
                        if(opponent.ssrLife[0] == CardType.Paper)
                        {
                            opponent.ssrLife.Remove(CardType.Paper);
                            return 1;
                        }
                        else if(opponent.ssrLife[0] == CardType.Rock)
                        {
                            player.ssrLife.Remove(CardType.Scissors);
                            return -1;
                        }
                        break;
                }
                player.ssrLife.RemoveAt(0);
                opponent.ssrLife.RemoveAt(0);
                return -2;
            }
            else if(player.ssrLife.Count == 1)
            {
                switch(player.ssrLife[0])
                {
                    case CardType.Rock:
                        if(opponent.ssrLife.Contains(CardType.Paper))
                        {
                            player.ssrLife.Remove(CardType.Rock);
                            opponent.ssrLife.Remove(CardType.Paper);
                            return -1;
                        }
                        else
                        {
                            player.ssrLife.Remove(CardType.Rock);
                            opponent.ssrLife.Remove(CardType.Rock);
                            return -1;
                        }
                    case CardType.Scissors:
                        if(opponent.ssrLife.Contains(CardType.Rock))
                        {
                            player.ssrLife.Remove(CardType.Scissors);
                            opponent.ssrLife.Remove(CardType.Rock);
                            return -1;
                        }
                        else
                        {
                            player.ssrLife.Remove(CardType.Scissors);
                            opponent.ssrLife.Remove(CardType.Scissors);
                            return -1;
                        }
                    case CardType.Paper:
                        if(opponent.ssrLife.Contains(CardType.Scissors))
                        {
                            player.ssrLife.Remove(CardType.Paper);
                            opponent.ssrLife.Remove(CardType.Scissors);
                            return -1;
                        }
                        else
                        {
                            player.ssrLife.Remove(CardType.Paper);
                            opponent.ssrLife.Remove(CardType.Paper);
                            return -1;
                        }
                }
            }
            else if(opponent.ssrLife.Count == 1)
            {
                switch(opponent.ssrLife[0])
                {
                    case CardType.Rock:
                        if(player.ssrLife.Contains(CardType.Paper))
                        {
                            player.ssrLife.Remove(CardType.Paper);
                            opponent.ssrLife.Remove(CardType.Rock);
                            return 1;
                        }
                        else
                        {
                            player.ssrLife.Remove(CardType.Rock);
                            opponent.ssrLife.Remove(CardType.Rock);
                            return 1;
                        }
                    case CardType.Scissors:
                        if(player.ssrLife.Contains(CardType.Rock))
                        {
                            player.ssrLife.Remove(CardType.Rock);
                            opponent.ssrLife.Remove(CardType.Scissors);
                            return 1;
                        }
                        else
                        {
                            player.ssrLife.Remove(CardType.Scissors);
                            opponent.ssrLife.Remove(CardType.Scissors);
                            return 1;
                        }
                    case CardType.Paper:
                        if(player.ssrLife.Contains(CardType.Scissors))
                        {
                            player.ssrLife.Remove(CardType.Scissors);
                            opponent.ssrLife.Remove(CardType.Paper);
                            return 1;
                        }
                        else
                        {
                            player.ssrLife.Remove(CardType.Paper);
                            opponent.ssrLife.Remove(CardType.Paper);
                            return 1;
                        }
                }
            }
            else
            {
                foreach(CardType type in player.ssrLife)
                {
                    if(opponent.ssrLife.Contains(type))
                    {
                        player.ssrLife.Remove(type);
                        opponent.ssrLife.Remove(type);
                        return 0;
                    }
                }
            }
            return -2;
        }

        // 플레이어의 카드만 SSR인 경우
        else if(player.cardData.cardType == CardType.All)
        {
            if(opponent.cardData.cardType == CardType.Rock)
            {
                if(player.ssrLife.Contains(CardType.Paper))
                {
                    if(player.ssrLife.Count > 1)
                    player.ssrLife.Remove(CardType.Paper);
                        return 1;
                }
                else if(player.ssrLife.Contains(CardType.Rock))
                {   
                    if(player.ssrLife.Count > 1)
                        player.ssrLife.Remove(CardType.Rock);
                    return 1;
                }
                else
                {
                    player.ssrLife.RemoveAt(0);
                    return -1;
                }
            }
            else if(opponent.cardData.cardType == CardType.Scissors)
            {
                if(player.ssrLife.Contains(CardType.Rock))
                {
                    if(player.ssrLife.Count > 1)
                        player.ssrLife.Remove(CardType.Rock);
                    return 1;
                }
                else if(player.ssrLife.Contains(CardType.Scissors))
                {   
                    if(player.ssrLife.Count > 1)
                        player.ssrLife.Remove(CardType.Scissors);
                    return 1;
                }
                else
                {
                    player.ssrLife.RemoveAt(0);
                    return -1;
                }
            }
            else
            {
                if(player.ssrLife.Contains(CardType.Scissors))
                {
                    if(player.ssrLife.Count > 1)
                        player.ssrLife.Remove(CardType.Scissors);
                    return 1;
                }
                else if(player.ssrLife.Contains(CardType.Paper))
                {   
                    if(player.ssrLife.Count > 1)
                        player.ssrLife.Remove(CardType.Paper);
                    return 1;
                }
                else
                {
                    player.ssrLife.RemoveAt(0);
                    return -1;
                }
            }
        }

        // 상대 카드만 SSR인 경우
        else if(opponent.cardData.cardType == CardType.All)
        {
            if(player.cardData.cardType == CardType.Rock)
            {
                if(opponent.ssrLife.Contains(CardType.Paper))
                {
                    if(opponent.ssrLife.Count > 1)
                        opponent.ssrLife.Remove(CardType.Paper);
                    return -1;
                }
                else if(opponent.ssrLife.Contains(CardType.Rock))
                {   
                    if(opponent.ssrLife.Count > 1)
                        opponent.ssrLife.Remove(CardType.Rock);
                    return -1;
                }
                else
                {
                    opponent.ssrLife.RemoveAt(0);
                    return 1;
                }
            }
            else if(player.cardData.cardType == CardType.Scissors)
            {
                if(opponent.ssrLife.Contains(CardType.Rock))
                {
                    if(opponent.ssrLife.Count > 1)
                        opponent.ssrLife.Remove(CardType.Rock);
                    return -1;
                }
                else if(opponent.ssrLife.Contains(CardType.Scissors))
                {   
                    if(opponent.ssrLife.Count > 1)
                        opponent.ssrLife.Remove(CardType.Scissors);
                    return -1;
                }
                else
                {
                    opponent.ssrLife.RemoveAt(0);
                    return 1;
                }
            }
            else
            {
                if(opponent.ssrLife.Contains(CardType.Scissors))
                {
                    if(opponent.ssrLife.Count > 1)
                        opponent.ssrLife.Remove(CardType.Scissors);
                    return -1;
                }
                else if(opponent.ssrLife.Contains(CardType.Paper))
                {   
                    if(opponent.ssrLife.Count > 1)
                        opponent.ssrLife.Remove(CardType.Paper);
                    return -1;
                }
                else
                {
                    opponent.ssrLife.RemoveAt(0);
                    return 1;
                }
            }
        }

        // 플레이어와 상대 카드가 모두 SSR이 아닌 경우
        else
        {
            if(player.cardData.cardType == opponent.cardData.cardType)
            {
                if(player.cardData.attackPower > opponent.cardData.attackPower)
                    return 1;
                else if(player.cardData.attackPower < opponent.cardData.attackPower)
                    return -1;
                else
                    return -2;
            }

            else if((player.cardData.cardType == CardType.Rock && opponent.cardData.cardType == CardType.Scissors) || (player.cardData.cardType == CardType.Paper && opponent.cardData.cardType == CardType.Rock) ||
            (player.cardData.cardType == CardType.Scissors && opponent.cardData.cardType == CardType.Paper))
            {
                return 1;
            }

            else
            {
                return -1;
            }
        }
    }

    // 역 가위바위보 대결 결과
    private int ReverseBattle(CardStatus player, CardStatus opponent)
    {
        // 플레이어와 상대 카드가 모두 SSR인 경우
        if(player.cardData.cardType == CardType.All && opponent.cardData.cardType == CardType.All)
        {
            if(player.ssrLife.Count == 1 && opponent.ssrLife.Count == 1)
            {
                switch(player.ssrLife[0])
                {
                    case CardType.Rock:
                        if(opponent.ssrLife[0] == CardType.Paper)
                        {
                            opponent.ssrLife.Remove(CardType.Paper);
                            return 1;
                        }
                        else if(opponent.ssrLife[0] == CardType.Scissors)
                        {
                            player.ssrLife.Remove(CardType.Rock);
                            return -1;
                        }
                        break;

                    case CardType.Paper:
                        if(opponent.ssrLife[0] == CardType.Scissors)
                        {
                            opponent.ssrLife.Remove(CardType.Scissors);
                            return 1;
                        }
                        else if(opponent.ssrLife[0] == CardType.Rock)
                        {
                            player.ssrLife.Remove(CardType.Paper);
                            return -1;
                        }
                        break;

                        case CardType.Scissors:
                        if(opponent.ssrLife[0] == CardType.Rock)
                        {
                            opponent.ssrLife.Remove(CardType.Rock);
                            return 1;
                        }
                        else if(opponent.ssrLife[0] == CardType.Paper)
                        {
                            player.ssrLife.Remove(CardType.Scissors);
                            return -1;
                        }
                        break;
                }
                player.ssrLife.RemoveAt(0);
                opponent.ssrLife.RemoveAt(0);
                return -2;
            }
            else if(player.ssrLife.Count == 1)
            {
                switch(player.ssrLife[0])
                {
                    case CardType.Rock:
                        if(opponent.ssrLife.Contains(CardType.Scissors))
                        {
                            player.ssrLife.Remove(CardType.Rock);
                            opponent.ssrLife.Remove(CardType.Scissors);
                            return -1;
                        }
                        else
                        {
                            player.ssrLife.Remove(CardType.Rock);
                            opponent.ssrLife.Remove(CardType.Rock);
                            return -1;
                        }
                    case CardType.Scissors:
                        if(opponent.ssrLife.Contains(CardType.Paper))
                        {
                            player.ssrLife.Remove(CardType.Scissors);
                            opponent.ssrLife.Remove(CardType.Paper);
                            return -1;
                        }
                        else
                        {
                            player.ssrLife.Remove(CardType.Scissors);
                            opponent.ssrLife.Remove(CardType.Scissors);
                            return -1;
                        }
                    case CardType.Paper:
                        if(opponent.ssrLife.Contains(CardType.Rock))
                        {
                            player.ssrLife.Remove(CardType.Paper);
                            opponent.ssrLife.Remove(CardType.Rock);
                            return -1;
                        }
                        else
                        {
                            player.ssrLife.Remove(CardType.Paper);
                            opponent.ssrLife.Remove(CardType.Paper);
                            return -1;
                        }
                }
            }
            else if(opponent.ssrLife.Count == 1)
            {
                switch(opponent.ssrLife[0])
                {
                    case CardType.Rock:
                        if(player.ssrLife.Contains(CardType.Scissors))
                        {
                            opponent.ssrLife.Remove(CardType.Rock);
                            player.ssrLife.Remove(CardType.Scissors);
                            return 1;
                        }
                        else
                        {
                            opponent.ssrLife.Remove(CardType.Rock);
                            player.ssrLife.Remove(CardType.Rock);
                            return 1;
                        }
                    case CardType.Scissors:
                        if(player.ssrLife.Contains(CardType.Paper))
                        {
                            opponent.ssrLife.Remove(CardType.Scissors);
                            player.ssrLife.Remove(CardType.Paper);
                            return 1;
                        }
                        else
                        {
                            opponent.ssrLife.Remove(CardType.Scissors);
                            player.ssrLife.Remove(CardType.Scissors);
                            return 1;
                        }
                    case CardType.Paper:
                        if(player.ssrLife.Contains(CardType.Rock))
                        {
                            opponent.ssrLife.Remove(CardType.Paper);
                            player.ssrLife.Remove(CardType.Rock);
                            return 1;
                        }
                        else
                        {
                            opponent.ssrLife.Remove(CardType.Paper);
                            player.ssrLife.Remove(CardType.Paper);
                            return 1;
                        }
                }
            }
            else
            {
                foreach(CardType type in player.ssrLife)
                {
                    if(opponent.ssrLife.Contains(type))
                    {
                        player.ssrLife.Remove(type);
                        opponent.ssrLife.Remove(type);
                        return 0;
                    }
                }
            }
            return -2;
        }

        // 플레이어의 카드만 SSR인 경우
        else if(player.cardData.cardType == CardType.All)
        {
            if(opponent.cardData.cardType == CardType.Rock)
            {
                if(player.ssrLife.Contains(CardType.Scissors))
                {
                    if(player.ssrLife.Count > 1)
                        player.ssrLife.Remove(CardType.Scissors);
                    return 1;
                }
                else if(player.ssrLife.Contains(CardType.Rock))
                {   
                    if(player.ssrLife.Count > 1)
                        player.ssrLife.Remove(CardType.Rock);
                    return 1;
                }
                else
                {
                    player.ssrLife.RemoveAt(0);
                    return -1;
                }
            }
            else if(opponent.cardData.cardType == CardType.Scissors)
            {
                if(player.ssrLife.Contains(CardType.Paper))
                {
                    if(player.ssrLife.Count > 1)
                        player.ssrLife.Remove(CardType.Paper);
                    return 1;
                }
                else if(player.ssrLife.Contains(CardType.Scissors))
                {   
                    if(player.ssrLife.Count > 1)
                        player.ssrLife.Remove(CardType.Scissors);
                    return 1;
                }
                else
                {
                    player.ssrLife.RemoveAt(0);
                    return -1;
                }
            }
            else
            {
                if(player.ssrLife.Contains(CardType.Rock))
                {
                    if(player.ssrLife.Count > 1)
                        player.ssrLife.Remove(CardType.Rock);
                    return 1;
                }
                else if(player.ssrLife.Contains(CardType.Paper))
                {   
                    if(player.ssrLife.Count > 1)
                        player.ssrLife.Remove(CardType.Paper);
                    return 1;
                }
                else
                {
                    player.ssrLife.RemoveAt(0);
                    return -1;
                }
            }
        }

        // 상대 카드만 SSR인 경우
        else if(opponent.cardData.cardType == CardType.All)
        {
            if(player.cardData.cardType == CardType.Rock)
            {
                if(opponent.ssrLife.Contains(CardType.Scissors))
                {
                    if(opponent.ssrLife.Count > 1)
                        opponent.ssrLife.Remove(CardType.Scissors);
                    return -1;
                }
                else if(opponent.ssrLife.Contains(CardType.Rock))
                {   
                    if(opponent.ssrLife.Count > 1)
                        opponent.ssrLife.Remove(CardType.Rock);
                    return -1;
                }
                else
                {
                    opponent.ssrLife.RemoveAt(0);
                    return 1;
                }
            }
            else if(player.cardData.cardType == CardType.Scissors)
            {
                if(opponent.ssrLife.Contains(CardType.Paper))
                {
                    if(opponent.ssrLife.Count > 1)
                        opponent.ssrLife.Remove(CardType.Paper);
                    return -1;
                }
                else if(opponent.ssrLife.Contains(CardType.Scissors))
                {   
                    if(opponent.ssrLife.Count > 1)
                        opponent.ssrLife.Remove(CardType.Scissors);
                    return -1;
                }
                else
                {
                    opponent.ssrLife.RemoveAt(0);
                    return 1;
                }
            }
            else
            {
                if(opponent.ssrLife.Contains(CardType.Rock))
                {
                    if(opponent.ssrLife.Count > 1)
                        opponent.ssrLife.Remove(CardType.Rock);
                    return -1;
                }
                else if(opponent.ssrLife.Contains(CardType.Paper))
                {   
                    if(opponent.ssrLife.Count > 1)
                        opponent.ssrLife.Remove(CardType.Paper);
                    return -1;
                }
                else
                {
                    opponent.ssrLife.RemoveAt(0);
                    return 1;
                }
            }
        }

        // 플레이어와 상대 카드가 모두 SSR이 아닌 경우
        else
        {
            if(player.cardData.cardType == opponent.cardData.cardType)
            {
                if(player.cardData.attackPower > opponent.cardData.attackPower)
                    return 1;
                else if(player.cardData.attackPower < opponent.cardData.attackPower)
                    return -1;
                else
                    return -2;
            }

            else if((player.cardData.cardType == CardType.Rock && opponent.cardData.cardType == CardType.Scissors) || (player.cardData.cardType == CardType.Paper && opponent.cardData.cardType == CardType.Rock) ||
            (player.cardData.cardType == CardType.Scissors && opponent.cardData.cardType == CardType.Paper))
            {
                return -1;
            }

            else
            {
                return 1;
            }
        }
    }

    // 플레이어 카드 선택
    public void SelectCard(PlayerCard_Defualt card)
    {
        // 카드 선택이 2장 가능한 경우
        if(DualManager.stage.dualMode.Contains(DualMode.Minus_One))
        {
            if(!selectCards.Contains(card))
            {
                if(selectCards.Count == 2)
                {
                    selectCards[0].Deselect();
                }
                selectCards.Add(card);
            }
        }

        else
        {
            if(selectedCard != null && selectedCard != card)
            {
                selectedCard.Deselect();
            }

            selectedCard = card;
        }
        DualButtonChange();
    }

    // 플레이어 카드 선택 취소
    public void DeselectCard(PlayerCard_Defualt card)
    {
        if(DualManager.stage.dualMode.Contains(DualMode.Minus_One))
        {
            selectCards.Remove(card);
        }

        else
        {
            if(selectedCard == card)
            {
                selectedCard = null;
            }
        }
        DualButtonChange();
    }

    // 듀얼 버튼 변경
    private void DualButtonChange()
    {
        if(DualManager.stage.dualMode.Contains(DualMode.Minus_One))
        {
            if((hand.cardObjects.Count == 1 && selectCards.Count == 1) || selectCards.Count == 2)
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
                dualButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = hand.cardObjects.Count == 1?
                "카드선택" : $"카드선택\n({selectCards.Count}/2)";
            }
        }
        else
        {
            if(selectedCard != null)
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
                dualButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = $"카드선택";
            }
        }

        if(DualManager.stage.dualMode.Contains(DualMode.No_Duplicate))
        {
            if(previousePlayerCard.Count > 0 && CheckPlayerDuplicate())
            {
                dualButton.interactable = false;
                dualButton.GetComponent<Image>().color = Color.gray;
                dualButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().fontSize = 60f;
                dualButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = $"중복";
            }
        }
    }

    // 듀얼 버튼 클릭
    public void DualButton()
    {
        if(!DualManager.isSequenceRunning)
        {
            DualManager.isSequenceRunning = true;

            // 하나 빼기 기믹
            if(DualManager.stage.dualMode.Contains(DualMode.Minus_One))
            {
                if(DualManager.stage.dualMode.Contains(DualMode.No_Duplicate))
                {
                    if(CheckPlayerDuplicate())
                    {
                        return;
                    }
                    else
                    {
                        previousePlayerCard.Clear();
                        foreach(PlayerCard_Defualt card in selectCards)
                        {
                            previousePlayerCard.Add(card.cardStatus.cardData.cardID);
                        }
                    }
                }

                if(selectCards == null || (selectCards.Count == 1 && hand.cardObjects.Count > 1))
                {    
                    return;
                }
                else if(hand.cardObjects.Count == 1 && selectCards.Count == 1)
                {
                    selectedCard = selectCards[0];
                }
            }

            // 중복 금지 기믹
            else if(DualManager.stage.dualMode.Contains(DualMode.No_Duplicate) && selectedCard != null)
            {
                if(CheckPlayerDuplicate())
                {
                    return;
                }
                else
                {
                    previousePlayerCard.Clear();
                    previousePlayerCard.Add(selectedCard.cardStatus.cardData.cardID);
                }
            }

            // 선택한 카드가 없으면 종료
            else
            {
                if(selectedCard == null)
                    return;
            }
            
            // 적 카드 선택
            enemyIndex = Random.Range(0, enemyCard.enemyCards.Count);

            //하나 빼기일 경우 적 카드 선택 및 플레이어 카드 선택 애니메이션 재생
            if(DualManager.stage.dualMode.Contains(DualMode.Minus_One))
            {
                enemyIndex_MO_1 = Random.Range(0, enemyCard.enemyCards.Count);
                enemyIndex_MO_2 = Random.Range(0, enemyCard.enemyCards.Count);
                if(DualManager.stage.dualMode.Contains(DualMode.No_Duplicate))
                {
                    List<int> cardIdCount = new List<int>();
                    foreach(CardStatus item in enemyCard.enemyCards)
                    {
                        if(!cardIdCount.Contains(item.cardData.cardID))
                        {
                            cardIdCount.Add(item.cardData.cardID);
                        }
                    }

                    if(cardIdCount.Count > 3)
                    {
                        while(CheckOpponentDuplicate(enemyIndex_MO_1))
                        {
                            enemyIndex_MO_1 = Random.Range(0, enemyCard.enemyCards.Count);
                        }
                        while(CheckOpponentDuplicate(enemyIndex_MO_2) || enemyIndex_MO_1 == enemyIndex_MO_2)
                        {
                            enemyIndex_MO_2 = Random.Range(0, enemyCard.enemyCards.Count);
                        }
                        previouseEnemyCard.Clear();
                        previouseEnemyCard.Add(enemyIndex_MO_1);
                        previouseEnemyCard.Add(enemyIndex_MO_2);

                        if(enemyIndex_MO_2 == Enemy_Minus_One(enemyIndex_MO_1, enemyIndex_MO_2))
                        {
                            int temp = enemyIndex_MO_2;
                            enemyIndex_MO_2 = enemyIndex_MO_1;
                            enemyIndex_MO_1 = temp;
                        }
                        enemyIndex = enemyIndex_MO_1;
                    }

                    else if(enemyCard.enemyCards.Count > 2)
                    {
                        enemyIndex_MO_1 = Random.Range(0, enemyCard.enemyCards.Count);
                        while(enemyIndex_MO_1 == enemyIndex_MO_2)
                        {
                            enemyIndex_MO_2 = Random.Range(0, enemyCard.enemyCards.Count);
                        }
                        enemyIndex = Random.Range(0, 2) % 2 == 0 ? enemyIndex_MO_1 : enemyIndex_MO_2;
                    }

                    else if(enemyCard.enemyCards.Count == 2)
                    {
                        enemyIndex_MO_1 = 0;
                        enemyIndex_MO_2 = 1;
                        enemyIndex = Random.Range(0, 2) % 2 == 0 ? enemyIndex_MO_1 : enemyIndex_MO_2;
                    }

                    else
                    {
                        enemyIndex = enemyIndex_MO_1;
                    }
                }

                else
                {
                    if(enemyCard.enemyCards.Count > 1)
                    {
                        while(enemyIndex_MO_1 == enemyIndex_MO_2)
                        {
                            enemyIndex_MO_2 = Random.Range(0, enemyCard.enemyCards.Count);
                        }

                        if(enemyIndex_MO_2 == Enemy_Minus_One(enemyIndex_MO_1, enemyIndex_MO_2))
                        {
                            int temp = enemyIndex_MO_2;
                            enemyIndex_MO_2 = enemyIndex_MO_1;
                            enemyIndex_MO_1 = temp;
                        }
                        enemyIndex = enemyIndex_MO_1;

                        
                    }
                    else
                    {
                        enemyIndex = enemyIndex_MO_1;
                    }
                }
                
                moCoroutine = StartCoroutine(Minus_One());
                return;
            }

            // 중복 금지 기믹
            else if(DualManager.stage.dualMode.Contains(DualMode.No_Duplicate) && enemyCard.enemyCards.Count != 1)
            {
                while(CheckOpponentDuplicate(enemyIndex))
                {
                    enemyIndex = Random.Range(0, enemyCard.enemyCards.Count);
                }
                previouseEnemyCard.Clear();
                previouseEnemyCard.Add(enemyIndex);
            }

            StartCoroutine(Dual());
        }
    }
    
    // 중복 확인 기믹
    private bool CheckPlayerDuplicate()
    {
        List<int> cardIDs = new List<int>();
        foreach(PlayerCard_Defualt card in hand.cardObjects)
        {
            if(!cardIDs.Contains(card.cardStatus.cardData.cardID))
            {
                cardIDs.Add(card.cardStatus.cardData.cardID);
            }
        }

        if(DualManager.stage.dualMode.Contains(DualMode.Minus_One))
        {
            if(cardIDs.Count <= 3)
            {
                return false;
            }
            else
            {
                foreach(PlayerCard_Defualt card in selectCards)
                {
                    if(previousePlayerCard.Contains(card.cardStatus.cardData.cardID))
                    {
                        return true;
                    }
                }
            }
        }

        else
        {
            if(selectedCard == null)
                return false;
            else
            {
                if(cardIDs.Count <= 1)
                {
                    return false;
                }
                else
                {
                    if(previousePlayerCard.Contains(selectedCard.cardStatus.cardData.cardID))
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    private bool CheckOpponentDuplicate(int index)
    {
        List<int> cardIDs = new List<int>();
        foreach(PlayerCard_Defualt card in hand.cardObjects)
        {
            if(!cardIDs.Contains(card.cardStatus.cardData.cardID))
            {
                cardIDs.Add(card.cardStatus.cardData.cardID);
            }
        }

        if(DualManager.stage.dualMode.Contains(DualMode.Minus_One))
        {
            if(cardIDs.Count <= 3)
            {
                return false;
            }
            else
            {
                if(previouseEnemyCard.Contains(index))
                {
                    return true;
                }
            }
        }

        else
        {
            if(cardIDs.Count <= 1)
            {
                return false;
            }
            else
            {
                if(previouseEnemyCard.Contains(index))
                {
                    return true;
                }
            }
        }
        return false;
    }

    // 하나 빼기 적 카드 보정
    private int Enemy_Minus_One(int index1, int index2)
    {
        if(hand.cardObjects.Count == 1 || selectCards[0].cardStatus.cardData.cardType == selectCards[1].cardStatus.cardData.cardType)
        {
            if(hand.cardObjects[0].cardStatus.cardData.cardType == CardType.Rock)
            {
                if(enemyCard.enemyCards[index1].cardData.cardType == CardType.Paper)
                    return index1;
                else if(enemyCard.enemyCards[index2].cardData.cardType == CardType.Paper)
                    return index2;
                else if(enemyCard.enemyCards[index2].cardData.cardType == CardType.Rock)
                    return index1;
                else if(enemyCard.enemyCards[index2].cardData.cardType == CardType.Rock)
                    return index2;
                else
                {
                    if(Random.Range(0,2) % 2 == 0)
                        return index1;
                    else
                        return index2;
                }
            }
            else if(hand.cardObjects[0].cardStatus.cardData.cardType == CardType.Scissors)
            {
                if(enemyCard.enemyCards[index1].cardData.cardType == CardType.Rock)
                    return index1;
                else if(enemyCard.enemyCards[index2].cardData.cardType == CardType.Rock)
                    return index2;
                else if(enemyCard.enemyCards[index2].cardData.cardType == CardType.Scissors)
                    return index1;
                else if(enemyCard.enemyCards[index2].cardData.cardType == CardType.Scissors)
                    return index2;
                else
                {
                    if(Random.Range(0,2) % 2 == 0)
                        return index1;
                    else
                        return index2;
                }
            }
            else
            {
                if(enemyCard.enemyCards[index1].cardData.cardType == CardType.Scissors)
                    return index1;
                else if(enemyCard.enemyCards[index2].cardData.cardType == CardType.Scissors)
                    return index2;
                else if(enemyCard.enemyCards[index2].cardData.cardType == CardType.Paper)
                    return index1;
                else if(enemyCard.enemyCards[index2].cardData.cardType == CardType.Paper)
                    return index2;
                else
                {
                    if(Random.Range(0,2) % 2 == 0)
                        return index1;
                    else
                        return index2;
                }
            }
        }
        else
        {
            if(Random.Range(0,2) % 2 == 0)
                return index1;
            else
                return index2;
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
        DualManager.isSequenceRunning = false;
        DualButtonChange();
    }

    // 승리 애니메이션
    IEnumerator Win()
    {
        AudioManager.instance.PlaySfx(AudioManager.instance.sfxClips_DualMode[4]);
        yield return selectedCard.cardImage.GetComponent<RectTransform>().DOAnchorPosY(300f, 0.2f).SetEase(Ease.InQuad).WaitForCompletion();
        enemyCard.transform.GetChild(0).GetChild(1).gameObject.SetActive(true);

        AudioManager.instance.PlaySfx(AudioManager.instance.sfxClips_DualMode[5]);

        if(PlayerPrefs.GetInt("DualVoice", 1) == 1)
            AudioManager.instance.PlaySfx(AudioManager.instance.sfxClips_DualWin[Random.Range(0,AudioManager.instance.sfxClips_DualWin.Length)]);
            
        Tween punchTween = enemyCard.GetComponent<RectTransform>().DOPunchAnchorPos(new Vector2(0, 60), 0.5f, 10, 0.5f);
        Tween posTween = selectedCard.cardImage.GetComponent<RectTransform>().DOAnchorPosY(0, 0.3f).SetEase(Ease.OutQuad);
        Tween shakeTween = GetComponent<RectTransform>().DOShakeAnchorPos(0.5f, 30, 30);
        yield return new WaitWhile(() =>
            punchTween.IsActive() && !punchTween.IsComplete() ||
            posTween.IsActive() && !posTween.IsComplete() ||
            shakeTween.IsActive() && !shakeTween.IsComplete()
        );

    }

    // 비김 애니메이션
    IEnumerator Draw()
    {
        if(PlayerPrefs.GetInt("DualVoice", 1) == 1)
            AudioManager.instance.PlaySfx(AudioManager.instance.sfxClips_CutScene[37]);

        Tween shakeTween_1 = selectedCard.GetComponent<RectTransform>().DOShakeAnchorPos(0.5f, 50, 30);
        Tween shakeTween_2 = enemyCard.GetComponent<RectTransform>().DOShakeAnchorPos(0.5f, 50, 30);
        yield return new WaitWhile(() =>
            shakeTween_1.IsActive() && !shakeTween_1.IsComplete() ||
            shakeTween_2.IsActive() && !shakeTween_2.IsComplete()
        );
    }

    // 패배 애니메이션
    IEnumerator Lose()
    {
        AudioManager.instance.PlaySfx(AudioManager.instance.sfxClips_DualMode[4]);
        yield return enemyCard.GetComponent<RectTransform>().DOAnchorPosY(0f, 0.2f).SetEase(Ease.InQuad).WaitForCompletion();
        selectedCard.transform.GetChild(1).gameObject.SetActive(true);

        AudioManager.instance.PlaySfx(AudioManager.instance.sfxClips_DualMode[5]);

        if(PlayerPrefs.GetInt("DualVoice", 1) == 1)
            AudioManager.instance.PlaySfx(AudioManager.instance.sfxClips_DualLose[Random.Range(0,AudioManager.instance.sfxClips_DualLose.Length)]);

        Tween punchTween = selectedCard.cardImage.GetComponent<RectTransform>().DOPunchAnchorPos(new Vector2(0, 60), 0.5f, 10, 0.5f);
        Tween posTween = enemyCard.GetComponent<RectTransform>().DOAnchorPosY(310f, 0.3f).SetEase(Ease.OutQuad);
        Tween shakeTween = GetComponent<RectTransform>().DOShakeAnchorPos(0.5f, 30, 30);
        yield return new WaitWhile(() =>
            punchTween.IsActive() && !punchTween.IsComplete() ||
            posTween.IsActive() && !posTween.IsComplete() ||
            shakeTween.IsActive() && !shakeTween.IsComplete()
        );

    }

    // 모두 패배 애니메이션
    IEnumerator SimultaneousLose()
    {
        AudioManager.instance.PlaySfx(AudioManager.instance.sfxClips_DualMode[4]);
        Tween posTween_1 = selectedCard.cardImage.GetComponent<RectTransform>().DOAnchorPosY(150f, 0.2f).SetEase(Ease.InQuad);
        Tween posTween_2 = enemyCard.GetComponent<RectTransform>().DOAnchorPosY(150f, 0.2f).SetEase(Ease.InQuad)
            .OnComplete(() =>
            {
                selectedCard.transform.GetChild(1).gameObject.SetActive(true);
                enemyCard.transform.GetChild(0).GetChild(1).gameObject.SetActive(true);
            });

        yield return new WaitWhile(() =>
            posTween_1.IsActive() && !posTween_1.IsComplete() ||
            posTween_2.IsActive() && !posTween_2.IsComplete()
        );

        AudioManager.instance.PlaySfx(AudioManager.instance.sfxClips_DualMode[6]);
        
        if(PlayerPrefs.GetInt("DualVoice", 1) == 1)
            AudioManager.instance.PlaySfx(AudioManager.instance.sfxClips_DualLose[Random.Range(0,AudioManager.instance.sfxClips_DualLose.Length)]);
        
        Tween posTween_3 = selectedCard.cardImage.GetComponent<RectTransform>().DOAnchorPosY(0f, 0.2f).SetEase(Ease.InQuad);
        Tween posTween_4 = enemyCard.GetComponent<RectTransform>().DOAnchorPosY(310f, 0.2f).SetEase(Ease.InQuad);
        Tween shakeTween = GetComponent<RectTransform>().DOShakeAnchorPos(0.5f, 30, 30);
        
        yield return new WaitWhile(() =>
            posTween_3.IsActive() && !posTween_3.IsComplete() ||
            posTween_4.IsActive() && !posTween_4.IsComplete() ||
            shakeTween.IsActive() && !shakeTween.IsComplete()
        );

    }

    // 듀얼 애니메이션
    IEnumerator Dual()
    {
        selectedCard.transform.SetParent(field);

        // 인터페이스 숨김
        yield return StartCoroutine(HideInterface(isTrackerActivated));

        // 선택한 카드를 필드로 이동
        yield return selectedCard.GetComponent<RectTransform>().DOAnchorPos(Vector2.zero, 0.5f);
        yield return selectedCard.transform.DORotate(Vector2.zero, 0.5f).WaitForCompletion();

        // 적 카드 공개
        yield return StartCoroutine(enemyCard.Flip(enemyCard.enemyCards[enemyIndex].cardData.cardSprite));
        tracker.CheckCardReveal(enemyIndex);

        // 스텟 공개
        AudioManager.instance.PlaySfx(AudioManager.instance.sfxClips_CutScene[0]);
        selectedCard.status.ShowStatus();
        enemyCard.status.cardStatus = enemyCard.enemyCards[enemyIndex];
        enemyCard.status.Init();
        enemyCard.status.ShowStatus();
        yield return new WaitForSeconds(1f);
        
        // 듀얼 결과
        int dualResult = DualManager.stage.dualMode.Contains(DualMode.Reverse)? 
            ReverseBattle(selectedCard.cardStatus, enemyCard.enemyCards[enemyIndex]) :
            Battle(selectedCard.cardStatus, enemyCard.enemyCards[enemyIndex]);

        // 전투 결과 애니메이션 재생
        switch(dualResult)
        {
            case -2:
                selectedCard.cardStatus.life--;
                enemyCard.enemyCards[enemyIndex].life--;
                yield return StartCoroutine(SimultaneousLose());
                selectedCard.status.StatusUpdate();
                enemyCard.status.StatusUpdate();
                break;

            case -1:
                selectedCard.cardStatus.life--;
                yield return StartCoroutine(Lose());
                selectedCard.status.StatusUpdate();
                if(enemyCard.enemyCards[enemyIndex].cardData.cardRarity == CardRarity.Null)
                {
                    enemyCard.status.StatusUpdate();
                }
                break;

            case 0:
                yield return StartCoroutine(Draw());
                selectedCard.status.StatusUpdate();
                enemyCard.status.StatusUpdate();
                break;

            case 1:
                enemyCard.enemyCards[enemyIndex].life--;
                yield return StartCoroutine(Win());
                enemyCard.status.StatusUpdate();
                if(selectedCard.cardStatus.cardData.cardRarity == CardRarity.Null)
                {
                    selectedCard.status.StatusUpdate();
                }
                break;
        }
        yield return new WaitForSeconds(0.5f);

        selectedCard.status.HideStatusForDefaultManager();
        enemyCard.status.HideStatusForDefaultManager();
        yield return new WaitForSeconds(1f);
        selectedCard.transform.SetParent(hand.transform);

        if(selectedCard.cardStatus.life == 0)
        {
            yield return StartCoroutine(selectedCard.DestroyCard());
            if(hand.cardObjects.Count == 0)
            {
                // 게임 오버(패배)
                transform.parent.GetComponent<DualManager>().GameOver(false);
            }
        }
        else
        {
            yield return selectedCard.GetComponent<RectTransform>().DOAnchorPosY(0f, 0.5f).WaitForCompletion();
            selectedCard.Deselect();
        }

        if(enemyCard.enemyCards[enemyIndex].life == 0)
        {
            AudioManager.instance.PlaySfx(AudioManager.instance.sfxClips_DualMode[3]);
            yield return StartCoroutine(enemyCard.Disappear((int)enemyCard.enemyCards[enemyIndex].cardData.cardRarity));
            enemyCard.enemyCards.RemoveAt(enemyIndex);
            enemyCard.cardImage.transform.localScale = Vector3.zero;
            enemyCard.cardImage.GetComponent<Image>().material.SetFloat("_Fade", 1);
            enemyCard.cardImage.sprite = enemyCard.backImage;
            tracker.SetGrayTrackerList(enemyIndex);

            if(enemyCard.enemyCards.Count == 0)
            {
                // 게임 오버(승리)
                transform.parent.GetComponent<DualManager>().GameOver(true);
            }
            else
                yield return enemyCard.cardImage.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack).WaitForCompletion();
            
        }
        else
        {
            // 적 카드를 뒷면으로 변경
            yield return StartCoroutine(enemyCard.Flip(enemyCard.backImage));
        }
        // 인터페이스 표시
        yield return ShowInterface(isTrackerActivated);
        DualManager.isSequenceRunning = false;
    }
    
    // 인터페이스 숨김
    IEnumerator HideInterface(bool isTrackerActivated)
    {
        if(!DualManager.isGameOver)
        {
            yield return dualButton.transform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InBack)
                .OnComplete(()=>
                {
                    hand.HideHand();
                }).WaitForCompletion();

            if(isTrackerActivated)
            {
                RectTransform trackerRect = tracker.GetComponent<RectTransform>();
                float posX = trackerRect.anchoredPosition.x >= 0? 1400 : -1400;
                yield return tracker.GetComponent<RectTransform>().DOAnchorPosX(posX, 0.2f).WaitForCompletion();
                tracker.gameObject.SetActive(false);
            }
        }
    }

    // 인터페이스 표시
    IEnumerator ShowInterface(bool isTrackerActivated)
    {
        if(!DualManager.isGameOver)
        {
            yield return dualButton.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack)
                .OnComplete(()=>
                {
                    hand.SetObjectSibling();
                    hand.currentIndex--;
                }).WaitForCompletion();
            yield return StartCoroutine(hand.ChangeHand());

            if(isTrackerActivated)
            {
                tracker.gameObject.SetActive(true);
                yield return tracker.GetComponent<RectTransform>().DOAnchorPos(tracker.initalPosition, 0.2f).WaitForCompletion();;
            }
        }
    }
    
    // 하나 빼기 애니메이션
    public IEnumerator Minus_One()
    {
        timer.localScale = Vector3.zero;
        timer.GetComponent<Image>().fillAmount = 1f;
        timer.gameObject.SetActive(true);

        RectTransform enemy_1 = enemyCard.transform.GetChild(0).GetComponent<RectTransform>();
        RectTransform enemy_2 = enemyCard.transform.GetChild(1).GetComponent<RectTransform>();
        enemy_2.gameObject.SetActive(true);

        yield return StartCoroutine(HideInterface(isTrackerActivated));

        if(selectCards.Count == 1)
        {
            if(!selectCards[0].gameObject.activeSelf)
                selectCards[0].gameObject.SetActive(true);

            selectedCard.transform.SetParent(field);
            yield return selectedCard.rectTransform.DOAnchorPos(Vector2.zero, 0.5f);
            yield return selectedCard.rectTransform.DORotate(Vector2.zero, 0.5f).WaitForCompletion();
        }
        else
        {
            if(!selectCards[0].gameObject.activeSelf)
                selectCards[0].gameObject.SetActive(true);
            if(!selectCards[1].gameObject.activeSelf)
                selectCards[1].gameObject.SetActive(true);

            // 플레이어 카드 이동
            selectCards[0].transform.SetParent(field);
            selectCards[1].transform.SetParent(field);
            yield return selectCards[0].rectTransform.DOAnchorPos(new Vector2(-480f,0), 0.5f);
            yield return selectCards[0].rectTransform.DORotate(Vector2.zero, 0.5f);
            yield return selectCards[1].rectTransform.DOAnchorPos(new Vector2(480f,0), 0.5f);
            yield return selectCards[1].rectTransform.DORotate(Vector2.zero, 0.5f).WaitForCompletion();
        }

        if(enemyCard.enemyCards.Count == 1)
        {
            enemy_2.gameObject.SetActive(false);
            yield return StartCoroutine(enemyCard.Flip(enemyCard.enemyCards[enemyIndex].cardData.cardSprite));
            tracker.CheckCardReveal(enemyIndex);
        }
        else
        {
            // 적 카드 공개
            if(Random.Range(0, 2) % 2 == 0)
            {
                yield return enemy_1.DOAnchorPosX(-480f, 0.5f);
                yield return enemy_2.DOAnchorPosX(480f, 0.5f).WaitForCompletion();
            }
            else
            {
                yield return enemy_1.DOAnchorPosX(480f, 0.5f);
                yield return enemy_2.DOAnchorPosX(-480f, 0.5f).WaitForCompletion();
            }
            
            StartCoroutine(enemyCard.Flip(enemyCard.enemyCards[enemyIndex_MO_1].cardData.cardSprite));
            yield return StartCoroutine(enemyCard.Flip_2(enemyCard.enemyCards[enemyIndex_MO_2].cardData.cardSprite));
            tracker.CheckCardReveal(enemyIndex_MO_1);
            tracker.CheckCardReveal(enemyIndex_MO_2);
        }

        // 스텟 공개
        AudioManager.instance.PlaySfx(AudioManager.instance.sfxClips_CutScene[0]);
        if(selectCards.Count == 1)
        {
            selectedCard.status.ShowStatus();
        }
        else
        {
            selectCards[0].status.ShowStatus();
            selectCards[1].status.ShowStatus();
        }

        enemyCard.status.cardStatus = enemyCard.enemyCards[enemyIndex_MO_1];
        enemyCard.status.Init();
        enemyCard.status.ShowStatus();
        
        if(enemyCard.enemyCards.Count > 1)
        {
            enemyCard.status_2.cardStatus = enemyCard.enemyCards[enemyIndex_MO_2];
            enemyCard.status_2.Init();
            enemyCard.status_2.ShowStatus();
        }
        yield return new WaitForSeconds(1f);

        // 하나 빼기 결과 진행
        if(selectCards.Count == 1)
        {
            StartCoroutine(Minus_One_Result());
        }

        // 플레이어 카드 선택
        else
        {
            yield return timer.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack).WaitForCompletion();
            minusOneRunning = true;

            float elapsed = 0f;  // 경과 시간
            float duration = 1f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;  // 매 프레임마다 경과 시간 증가
                timer.GetComponent<Image>().fillAmount = Mathf.Lerp(1f, 0f, elapsed / duration);  // FillAmount를 서서히 줄임
                yield return null;  // 다음 프레임까지 대기
            }
            minusOneRunning = false;
            selectedCard = Random.Range(0, 2) % 2 == 0 ? selectCards[0] : selectCards[1];
            StartCoroutine(Minus_One_Result());
        }
    }

    // 하나 빼기 결과 애니메이션
    public IEnumerator Minus_One_Result()
    {
        minusOneRunning = false;
        RectTransform enemy_1 = enemyCard.transform.GetChild(0).GetComponent<RectTransform>();
        RectTransform enemy_2 = enemyCard.transform.GetChild(1).GetComponent<RectTransform>();

        yield return timer.DOScale(Vector3.zero, 0.3f).SetEase(Ease.OutBack).WaitForCompletion();
        timer.GetComponent<Image>().fillAmount = 0f;

        selectedCard.transform.SetParent(field);

        // 선택한 카드는 중앙으로, 선택하지 않은 카드는 사라짐
        if(hand.cardObjects.Count > 1 && selectedCard != selectCards[0])
        {
            selectCards[0].status.HideStatus();
            yield return selectCards[0].transform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InBack);

            yield return selectedCard.transform.DOScale(Vector3.one, 0.5f);
            yield return selectedCard.rectTransform.DOAnchorPos(Vector2.zero, 0.5f);
            yield return selectedCard.transform.DORotate(Vector2.zero, 0.5f).WaitForCompletion();

            selectCards[0].transform.SetParent(hand.transform);
            selectCards[0].rectTransform.anchoredPosition = Vector2.zero;
            selectCards[0].transform.localScale = Vector3.one;
        }
        else if(hand.cardObjects.Count > 1 && selectedCard != selectCards[1])
        {
            selectCards[1].status.HideStatus();
            yield return selectCards[1].transform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InBack);

            yield return selectedCard.transform.DOScale(Vector3.one, 0.5f);
            yield return selectedCard.rectTransform.DOAnchorPos(Vector2.zero, 0.5f);
            yield return selectedCard.transform.DORotate(Vector2.zero, 0.5f).WaitForCompletion();

            selectCards[1].transform.SetParent(hand.transform);
            selectCards[1].rectTransform.anchoredPosition = Vector2.zero;
            selectCards[1].transform.localScale = Vector3.one;
        }

        enemyCard.status_2.HideStatus();
        yield return enemy_2.transform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InBack);
        yield return enemy_1.DOAnchorPosX(0f, 0.5f).WaitForCompletion();
        enemy_2.anchoredPosition = new Vector2(0, enemy_2.anchoredPosition.y);

        // 듀얼 결과
        int dualResult = DualManager.stage.dualMode.Contains(DualMode.Reverse)? 
            ReverseBattle(selectedCard.cardStatus, enemyCard.enemyCards[enemyIndex]) :
            Battle(selectedCard.cardStatus, enemyCard.enemyCards[enemyIndex]);

        yield return new WaitForSeconds(1f);

        // 전투 결과 애니메이션 재생
        switch(dualResult)
        {
            case -2:
                selectedCard.cardStatus.life--;
                enemyCard.enemyCards[enemyIndex].life--;
                yield return StartCoroutine(SimultaneousLose());
                selectedCard.status.StatusUpdate();
                enemyCard.status.StatusUpdate();
                break;

            case -1:
                selectedCard.cardStatus.life--;
                yield return StartCoroutine(Lose());
                selectedCard.status.StatusUpdate();
                if(enemyCard.enemyCards[enemyIndex].cardData.cardRarity == CardRarity.Null)
                {
                    enemyCard.status.StatusUpdate();
                }
                break;

            case 0:
                yield return StartCoroutine(Draw());
                selectedCard.status.StatusUpdate();
                enemyCard.status.StatusUpdate();
                break;

            case 1:
                enemyCard.enemyCards[enemyIndex].life--;
                yield return StartCoroutine(Win());
                enemyCard.status.StatusUpdate();
                if(selectedCard.cardStatus.cardData.cardRarity == CardRarity.Null)
                {
                    selectedCard.status.StatusUpdate();
                }
                break;
        }
        yield return new WaitForSeconds(0.5f);

        selectedCard.status.HideStatusForDefaultManager();
        enemyCard.status.HideStatusForDefaultManager();
        yield return new WaitForSeconds(1f);
        selectedCard.transform.SetParent(hand.transform);

        if(selectedCard.cardStatus.life == 0)
        {
            yield return StartCoroutine(selectedCard.DestroyCard());
            if(hand.cardObjects.Count == 0)
            {
                // 게임 오버(패배)
                transform.parent.GetComponent<DualManager>().GameOver(false);
            }
        }
        else
        {
            yield return selectedCard.rectTransform.DOAnchorPosY(0f, 0.5f).WaitForCompletion();
            selectedCard.Deselect();
        }

        if(enemyCard.enemyCards[enemyIndex].life == 0)
        {
            AudioManager.instance.PlaySfx(AudioManager.instance.sfxClips_DualMode[3]);
            yield return StartCoroutine(enemyCard.Disappear((int)enemyCard.enemyCards[enemyIndex].cardData.cardRarity));
            enemyCard.enemyCards.RemoveAt(enemyIndex);
            enemyCard.cardImage.transform.localScale = Vector3.zero;
            enemyCard.cardImage.GetComponent<Image>().material.SetFloat("_Fade", 1);
            enemyCard.cardImage.sprite = enemyCard.backImage;
            tracker.SetGrayTrackerList(enemyIndex);

            if(enemyCard.enemyCards.Count == 0)
            {
                // 게임 오버(승리)
                transform.parent.GetComponent<DualManager>().GameOver(true);
            }
            else
                yield return enemyCard.cardImage.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack).WaitForCompletion();
            
        }
        else
        {
            // 적 카드를 뒷면으로 변경
            yield return StartCoroutine(enemyCard.Flip(enemyCard.backImage));
        }

        enemyCard.rectTransform.anchoredPosition = new Vector2(0f,enemyCard.rectTransform.anchoredPosition.y);
        enemyCard.transform.localScale = Vector3.one;
        enemyCard.cardImage.sprite = enemyCard.backImage;

        enemy_2.anchoredPosition = new Vector2(0f,enemy_2.anchoredPosition.y);
        enemy_2.transform.localScale = Vector3.one;
        enemy_2.GetComponent<Image>().sprite = enemyCard.backImage;
        enemy_2.gameObject.SetActive(false);

        selectedCard = null;

        for(int i=0; i<selectCards.Count; i++)
        {
            selectCards[0].Deselect();
        }

        enemyIndex_MO_1 = -1;
        enemyIndex_MO_2 = -1;

        // 인터페이스 표시
        yield return ShowInterface(isTrackerActivated);
        DualManager.isSequenceRunning = false;
    }
}

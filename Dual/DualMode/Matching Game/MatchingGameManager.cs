using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class MatchingGameManager : MonoBehaviour
{
    public GameObject cardPrefab;   // 카드 프리팹

    [HideInInspector] public bool playerTurn = true;    // 턴 체크

    private Transform cardParent;   // 카드 오브젝트의 parent
    private Transform texts;        // 텍스트 오브젝트의 parent

    private List<MatchingCard> cards = new List<MatchingCard>();    // 카드 리스트
    private List<MatchingCard> memory = new List<MatchingCard>();   // 플레이 한 카드 저장

    private MatchingCard selectCard_1;  // 선택한 카드 1
    private MatchingCard selectCard_2;  // 선택한 카드 2

    private float[] pos_X = new float[6]{-625f, -375f, -125f, 125f, 375f, 625f};    // 카드 오브젝트 X 좌표
    private float[] pos_Y = new float[3]{300f, 0f, -300f};                          // 카드 오브젝트 Y 좌표
    
    private int playerMatchCount = 0;   // 플레이어가 맞춘 카드 수
    private int enemyMatchCount = 0;    // 적이 맞춘 카드 수

    private void Start()
    {
        DualManager.isSequenceRunning = true;
        cardParent = transform.GetChild(0);
        texts = transform.GetChild(1);
        texts.localScale = Vector3.one;
        texts.GetChild(0).localScale = Vector3.zero;
        Init();
    }

    // 카드 세팅
    private void Init()
    {
        List<int> indexs = new List<int>();
        for(int i = 0; i < 9; i++)
        {
            int index = Random.Range(0, 30);
            while(index == 11 || indexs.Contains(index))
            {
                index = Random.Range(0, 30);
            }

            indexs.Add(index);
            indexs.Add(index);
        }

        for(int i = 0; i < indexs.Count; i++)
        {
            int randomIndex = Random.Range(0, indexs.Count);
            int temp = indexs[i];
            indexs[i] = indexs[randomIndex];
            indexs[randomIndex] = temp;
        }

        cardParent.localScale = Vector3.zero;

        for(int i = 0; i < indexs.Count; i++)
        {
            GameObject _card = Instantiate(cardPrefab, cardParent);
            MatchingCard card = _card.GetComponent<MatchingCard>();
            cards.Add(card);
            card.Init(indexs[i]);
        }
        StartCoroutine(SetCardPosition());
    }

    // 턴 종료
    private void TurnEnd()
    {
        playerTurn = !playerTurn;
        selectCard_1 = null;
        selectCard_2 = null;
        if(playerTurn)
        {
            texts.GetChild(0).GetComponent<TextMeshProUGUI>().text = "나의 턴";
            texts.GetChild(0).DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack)
            .OnComplete(()=>
                texts.GetChild(0).DOScale(Vector3.one, 0.3f)
                .OnComplete(()=>
                    texts.GetChild(0).DOScale(Vector3.zero, 0.3f).SetEase(Ease.InBack)
                )
            );
            
            DualManager.isSequenceRunning = false;
        }
        else
        {
            texts.GetChild(0).GetComponent<TextMeshProUGUI>().text = "상대 턴";
            texts.GetChild(0).DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack)
            .OnComplete(()=>
                texts.GetChild(0).DOScale(Vector3.one, 0.3f)
                .OnComplete(()=>
                    texts.GetChild(0).DOScale(Vector3.zero, 0.3f).SetEase(Ease.InBack)
                )
            );
            
            StartCoroutine(EnemyTurn());
        }
    }

    // 카드 선택 메서드
    public void SelectCard(MatchingCard card)
    {
        AudioManager.instance.PlaySfx(AudioManager.instance.sfxClips_DualMode[10]);
        if(selectCard_1 == null)
        {
            selectCard_1 = card;
            selectCard_1.IsFlipped = true;
            if(!memory.Contains(card))
            {memory.Add(card);}
            StartCoroutine(card.Flip(card.cardSprite));
        }
        else
        {
            selectCard_2 = card;
            selectCard_1.IsFlipped = true;
            if(!memory.Contains(card))
            {memory.Add(card);}
            StartCoroutine(Matching());
        }
    }

    // 게임 오버
    private void GameOver()
    {
        // 승리
        if(playerMatchCount > enemyMatchCount)
        {
            transform.parent.GetComponent<DualManager>().GameOver(true);
        }
        // 패배
        else
        {
            transform.parent.GetComponent<DualManager>().GameOver(false);
        }
    }
    
    // 애니메이션

    // 카드 배치
    private IEnumerator SetCardPosition()
    {
        yield return cardParent.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack).WaitForCompletion();
        for(int i = 0; i < pos_Y.Length; i++)
        {
            for(int j = 0; j < pos_X.Length; j++)
            {
                AudioManager.instance.PlaySfx(AudioManager.instance.sfxClips_Main[8]);
                yield return cards[i*pos_X.Length+j].GetComponent<RectTransform>().DOAnchorPos(new Vector2(pos_X[j],pos_Y[i]), 0.15f).WaitForCompletion();
            }
        }
        texts.GetChild(0).GetComponent<TextMeshProUGUI>().text = "나의 턴";
        yield return texts.GetChild(0).DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack).WaitForCompletion();
        yield return new WaitForSeconds(0.3f);
        yield return texts.GetChild(0).DOScale(Vector3.zero, 0.3f).SetEase(Ease.InBack).WaitForCompletion();
        DualManager.isSequenceRunning = false;
    }

    // 카드 선택 결과
    private IEnumerator Matching()
    {
        DualManager.isSequenceRunning = true;
        yield return StartCoroutine(selectCard_2.Flip(selectCard_2.cardSprite));
        yield return new WaitForSeconds(0.5f);

        // 두 카드가 같은 카드일 경우
        if(selectCard_1.cardID == selectCard_2.cardID)
        {
            AudioManager.instance.PlaySfx(AudioManager.instance.sfxClips_DualMode[3]);
            // 카드 제거
            StartCoroutine(selectCard_1.Disappear((int)CardDataManager.instance.GetCardByID(selectCard_1.cardID).cardRarity));
            yield return StartCoroutine(selectCard_2.Disappear((int)CardDataManager.instance.GetCardByID(selectCard_2.cardID).cardRarity));
            cards.Remove(selectCard_1);
            cards.Remove(selectCard_2);
            memory.Remove(selectCard_1);
            memory.Remove(selectCard_2);
            Destroy(selectCard_1.gameObject);
            Destroy(selectCard_2.gameObject);

            // 매치 카운트 증가
            if(playerTurn)
            {
                playerMatchCount++;
                texts.GetChild(1).GetComponent<TextMeshProUGUI>().text = playerMatchCount.ToString();
                
                AudioManager.instance.PlaySfx(AudioManager.instance.sfxClips_Main[2]);
                yield return texts.GetChild(1).DOShakePosition(0.3f, 10f).WaitForCompletion();

                if(cards.Count == 0)
                {
                    GameOver();
                }
                else
                {
                    DualManager.isSequenceRunning = false;
                }
            }
            else
            {
                enemyMatchCount++;
                texts.GetChild(2).GetComponent<TextMeshProUGUI>().text = enemyMatchCount.ToString();

                AudioManager.instance.PlaySfx(AudioManager.instance.sfxClips_Main[1]);
                yield return texts.GetChild(2).DOShakePosition(0.3f, 10f).WaitForCompletion();
                
                if(cards.Count == 0)
                {
                    GameOver();
                }
                else
                {
                    StartCoroutine(EnemyTurn());
                }
            }
        }
        
        // 두 카드가 다른 카드인 경우, 턴 넘김
        else
        {
            StartCoroutine(selectCard_1.Flip(selectCard_1.backImage));
            StartCoroutine(selectCard_2.Flip(selectCard_2.backImage));
            selectCard_1.IsFlipped = false;
            selectCard_2.IsFlipped = false;
            yield return new WaitForSeconds(1f);
            TurnEnd();
        }
    }

    // 적 카드 선택
    private IEnumerator EnemyTurn()
    {
        yield return new WaitForSeconds(1f);

        // memory에서 같은 cardID를 가진 카드 쌍을 탐색
        bool foundPair = false;
        for(int i = 0; i < memory.Count; i++)
        {
            for(int j = i + 1; j < memory.Count; j++)
            {
                if(memory[i].cardID == memory[j].cardID && !memory[i].IsFlipped && !memory[j].IsFlipped)
                {
                    // 같은 cardID의 카드 쌍을 발견한 경우
                    SelectCard(memory[i]);
                    yield return new WaitForSeconds(1f);
                    SelectCard(memory[j]);
                    foundPair = true;
                    break;
                }
            }
            if(foundPair) 
                break;
        }

        // 같은 cardID 쌍이 memory에 없다면 무작위 카드를 뒤집음
        if(!foundPair)
        {
            // 무작위로 첫 번째 카드 선택
            MatchingCard firstCard = null;
            while(firstCard == null)
            {
                int randomIndex = Random.Range(0, cards.Count);
                if (!cards[randomIndex].IsFlipped)
                {
                    firstCard = cards[randomIndex];
                    SelectCard(firstCard);
                    yield return new WaitForSeconds(1f);
                }
            }
            // yield return new WaitForSeconds(1f);
            
            // memory에서 첫 번째 카드와 같은 cardID의 카드가 있는지 확인
            MatchingCard secondCard = memory.FirstOrDefault(card => card.cardID == firstCard.cardID && card != firstCard && !card.IsFlipped);

            // 같은 ID의 카드가 memory에 없으면 무작위로 두 번째 카드 선택
            if(secondCard == null)
            {
                while(secondCard == null)
                {
                    int randomIndex = Random.Range(0, cards.Count);
                    if(!cards[randomIndex].IsFlipped && cards[randomIndex] != firstCard)
                    {
                        secondCard = cards[randomIndex];
                        SelectCard(secondCard);
                    }
                }
            }
            else
            {
                // 같은 ID의 카드가 memory에 있으면 선택
                SelectCard(secondCard);
            }
        }
    }
}

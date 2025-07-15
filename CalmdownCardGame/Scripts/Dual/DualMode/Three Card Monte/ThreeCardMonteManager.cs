using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ThreeCardMonteManager : MonoBehaviour
{
    public List<GameObject> roundObjects = new List<GameObject>();  // 라운드 오브젝트 리스트
    public List<Card_TCM> cards = new List<Card_TCM>();             // 카드 오브젝트 리스트
    public Transform tmp;   // 침바오 카드는? 텍스트
    public Transform chanceObject;    // 기회 오브젝트

    private int currentRound = -1;
    private int chance = 3;
    private float duration = 0.4f;

    // *Awake 변경
    private void Start()
    {
        DualManager.isSequenceRunning = true;
        tmp.localScale = Vector3.zero;
        NextRound();
    }

    // 다음 라운드
    private void NextRound()
    {
        foreach(GameObject round in roundObjects)
        {
            round.SetActive(false);
        }
        currentRound++;

        roundObjects[currentRound].SetActive(true);
        cards.Clear();
        foreach(Transform _card in roundObjects[currentRound].transform)
        {
            Card_TCM card = _card.GetComponent<Card_TCM>();
            cards.Add(card);
            card.correct = false;
            card.fake = false;
        }
        
        cards[Random.Range(0, cards.Count)].correct = true;
        if(currentRound == roundObjects.Count - 1)
        {
            int fakeIndex = Random.Range(0, cards.Count);
            while(cards[fakeIndex].correct)
            {
                fakeIndex = Random.Range(0, cards.Count);
            }
            cards[fakeIndex].fake = true;
        }
        
        StartCoroutine(RoundStart());
    }

    // 기회 오브젝트 업데이트
    private void ChanceUpdate()
    {
        for(int i = 0; i < 3; i++)
        {
            if(i < chance)
            {
                chanceObject.GetChild(i).gameObject.SetActive(true);
            }
            else
            {
                chanceObject.GetChild(i).gameObject.SetActive(false);
            }
        }
        AudioManager.instance.PlaySfx(AudioManager.instance.sfxClips_DualMode[6]);
        chanceObject.DOShakePosition(0.3f, 10);
    }
    // 애니메이션

    // 카드 공개
    private IEnumerator OpenCard()
    {
        yield return new WaitForSeconds(1f);
        AudioManager.instance.PlaySfx(AudioManager.instance.sfxClips_DualMode[10]);
        if(currentRound == roundObjects.Count - 1)
        {
            foreach(Card_TCM card in cards)
            {
                if(card.correct)
                    StartCoroutine(card.Fake_Flip(CardDataManager.instance.GetCardByID(27).cardSprite, CardDataManager.instance.GetCardByID(14).cardSprite));
                
                else if(card.fake)
                    StartCoroutine(card.Fake_Flip(CardDataManager.instance.GetCardByID(14).cardSprite, CardDataManager.instance.GetCardByID(27).cardSprite));
                
                else
                    StartCoroutine(card.Flip(CardDataManager.instance.GetCardByID(14).cardSprite));
            }
        }
        else
        {
            foreach(Card_TCM card in cards)
            {
                if(card.correct)
                    StartCoroutine(card.Flip(CardDataManager.instance.GetCardByID(27).cardSprite));

                else
                    StartCoroutine(card.Flip(CardDataManager.instance.GetCardByID(14).cardSprite));

            }
        }
        yield return new WaitForSeconds(1f);
    }

    // 카드 숨김
    private IEnumerator HideCard()
    {
        yield return new WaitForSeconds(1f);
        AudioManager.instance.PlaySfx(AudioManager.instance.sfxClips_DualMode[10]);
        if(currentRound == roundObjects.Count - 1)
        {
            foreach(Card_TCM card in cards)
            {
                StartCoroutine(card.Fake_Flip(card.backImage, card.backImage));
            }
            yield return new WaitForSeconds(1f);

            foreach(Card_TCM card in cards)
            {
                card.cardImage.transform.GetChild(0).gameObject.SetActive(false);
            }
        }
        else
        {
            foreach(Card_TCM card in cards)
            {
                StartCoroutine(card.Flip(card.backImage));
            }
            yield return new WaitForSeconds(1f);
        }
    }

    // 라운드 시작
    private IEnumerator RoundStart()
    {
        yield return StartCoroutine(OpenCard());
        yield return new WaitForSeconds(1f);
        yield return StartCoroutine(HideCard());

        duration = 0.5f;

        // 라운드가 올라갈수록 섞는 횟수 증가
        int shuffleCount = currentRound==roundObjects.Count-1?
        5 : 5 + currentRound * 5;
        //int previous = -1;
        int randomEffect;
        for(int i=0; i<shuffleCount; i++)
        {
            // 0: ChangeCard, 1: turnClockwise, 2: turnCounterclockwise
            /*
            do {randomEffect = Random.Range(0, 3);}
            while (previous == randomEffect);
            previous = randomEffect;
            */
            
            randomEffect = currentRound > 0 && currentRound < roundObjects.Count - 1 ? 
            Random.Range(0, 4) : Random.Range(0, 3);
            AudioManager.instance.PlaySfx(AudioManager.instance.sfxClips_DualMode[11]);
            switch(randomEffect)
            {
                case 0:
                case 1:
                    int index1 = Random.Range(0, cards.Count);
                    int index2;
                    do {index2 = Random.Range(0, cards.Count);}
                    while (index2 == index1);
                    yield return StartCoroutine(ChangeCard(index1, index2));
                    break;

                case 2:
                    if(Random.Range(0,1) % 2 == 0)
                    {
                        yield return StartCoroutine(TurnClockwise());
                    }
                    else
                    {
                        yield return StartCoroutine(TurnCounterclockwise());
                    }
                    break;

                case 3:
                    yield return StartCoroutine(ChangeCards());
                    break;
            }
            yield return new WaitForSeconds(0.1f);
            duration -= 0.03f;
        }

        yield return tmp.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack).WaitForCompletion();
        DualManager.isSequenceRunning = false;
    }

    // 카드 선택
    public IEnumerator SelectCard(bool correct)
    {   
        DualManager.isSequenceRunning = true;
        AudioManager.instance.PlaySfx(AudioManager.instance.sfxClips_DualMode[7]);
        foreach(Card_TCM card in cards)
        {
            yield return card.cardImage.transform.DOScale(Vector3.one, 0.5f);
        }
        yield return tmp.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InBack).WaitForCompletion();
        yield return new WaitForSeconds(1f);

        if(currentRound != roundObjects.Count - 1)
        {
            yield return StartCoroutine(OpenCard());
            yield return new WaitForSeconds(0.5f);
        }
        
        // 정답
        if(correct)
        {
            if(currentRound == roundObjects.Count-1)
            {
                foreach(Card_TCM card in cards)
                {
                    if(card.correct)
                    {
                        StartCoroutine(card.Fake_Flip(CardDataManager.instance.GetCardByID(27).cardSprite, CardDataManager.instance.GetCardByID(14).cardSprite));
                    }
                    // else if(card.fake)
                    // {
                    //     StartCoroutine(card.Fake_Flip(CardDataManager.instance.GetCardByID(14).cardSprite, CardDataManager.instance.GetCardByID(27).cardSprite));
                    // }
                    else
                    {
                        card.cardImage.transform.GetChild(0).gameObject.SetActive(false);
                        StartCoroutine(card.Flip(CardDataManager.instance.GetCardByID(14).cardSprite));
                    }
                }
                yield return new WaitForSeconds(1.5f);

                AudioManager.instance.PlaySfx(AudioManager.instance.sfxClips_Main[8]);
                foreach(Card_TCM card in cards)
                {
                    if(card.correct)
                    {
                        card.cardImage.transform.GetChild(0).GetComponent<RectTransform>().DOAnchorPosY(-800f,0.5f).WaitForCompletion();
                    }
                    // else if(card.fake)
                    // {
                    //     card.cardImage.transform.GetChild(0).GetComponent<RectTransform>().DOAnchorPosY(-800f,0.5f).WaitForCompletion();
                    // }
                }
                yield return new WaitForSeconds(0.5f);

                AudioManager.instance.PlaySfx(AudioManager.instance.sfxClips_DualMode[8]);
                yield return new WaitForSeconds(1f);
                // 게임 종료(승리)
                transform.parent.GetComponent<DualManager>().GameOver(true);
            }
            else
            {
                AudioManager.instance.PlaySfx(AudioManager.instance.sfxClips_DualMode[8]);
                yield return new WaitForSeconds(0.5f);

                foreach(Card_TCM card in cards)
                {
                    yield return card.transform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InBack);
                }
                yield return new WaitForSeconds(0.5f);
                NextRound();
            }
        }
        // 오답
        else
        {
            if(currentRound == roundObjects.Count-1)
            {
                foreach(Card_TCM card in cards)
                {
                    if(card.correct)
                    {
                        StartCoroutine(card.Fake_Flip(CardDataManager.instance.GetCardByID(27).cardSprite, CardDataManager.instance.GetCardByID(14).cardSprite));
                    }
                    // else if(card.fake)
                    // {
                    //     StartCoroutine(card.Fake_Flip(CardDataManager.instance.GetCardByID(14).cardSprite, CardDataManager.instance.GetCardByID(27).cardSprite));
                    // }
                    else
                    {
                        card.cardImage.transform.GetChild(0).gameObject.SetActive(false);
                        StartCoroutine(card.Flip(CardDataManager.instance.GetCardByID(14).cardSprite));
                    }
                }
                yield return new WaitForSeconds(1.5f);
                AudioManager.instance.PlaySfx(AudioManager.instance.sfxClips_Main[8]);
                foreach(Card_TCM card in cards)
                {
                    if(card.correct)
                    {
                        yield return card.cardImage.transform.GetChild(0).GetComponent<RectTransform>().DOAnchorPosY(-800f,0.5f)
                            .OnComplete(()=>card.cardImage.transform.GetChild(0).gameObject.SetActive(false)).WaitForCompletion();
                    }
                    // else if(card.fake)
                    // {
                    //     card.cardImage.transform.GetChild(0).GetComponent<RectTransform>().DOAnchorPosY(-800f,0.5f).WaitForCompletion();
                    // }
                }
                // yield return new WaitForSeconds(0.5f);

                AudioManager.instance.PlaySfx(AudioManager.instance.sfxClips_DualMode[9]);
                yield return new WaitForSeconds(0.5f);
                foreach(Card_TCM card in cards)
                {
                    yield return card.transform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InBack);
                }
                yield return new WaitForSeconds(0.5f);
            }
            else
            {
                AudioManager.instance.PlaySfx(AudioManager.instance.sfxClips_DualMode[9]);
                yield return new WaitForSeconds(0.5f);

                foreach(Card_TCM card in cards)
                {
                    yield return card.transform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InBack);
                }
                yield return new WaitForSeconds(0.5f);
            }

            chance--;
            ChanceUpdate();

            yield return new WaitForSeconds(0.3f);
            if(chance == 0)
            {
                // 게임 오버
                transform.parent.GetComponent<DualManager>().GameOver(false);
            }
            else
            {
                currentRound--;
                NextRound();
            }
            
        }
        
    }

    // 두 카드 위치 변경
    private IEnumerator ChangeCard(int index_1, int index_2)
    {
        Vector2 position_1 = cards[index_1].rectTransform.anchoredPosition;
        Vector2 position_2 = cards[index_2].rectTransform.anchoredPosition;

        yield return cards[index_1].rectTransform.DOAnchorPos(position_2, duration);
        yield return cards[index_2].rectTransform.DOAnchorPos(position_1, duration).WaitForCompletion();

        cards[index_1].rectTransform.anchoredPosition = position_2;
        cards[index_2].rectTransform.anchoredPosition = position_1;

        Card_TCM temp = cards[index_1];
        cards[index_1] = cards[index_2];
        cards[index_2] = temp;
    }

    // 시계 방향을 회전
    private IEnumerator TurnClockwise()
    {
        List<Vector2> positions = new List<Vector2>();

        foreach(Card_TCM card in cards)
        {
            positions.Add(card.rectTransform.anchoredPosition);
        }

        // 각 카드의 위치를 시계 방향으로 이동
        for (int i = 0; i < cards.Count; i++)
        {
            int newIndex = (i == cards.Count - 1) ? 0 : i + 1;
            yield return cards[i].rectTransform.DOAnchorPos(positions[newIndex], duration);
        }
        yield return new WaitForSeconds(duration);

        // 카드 리스트를 시계 방향으로 업데이트
        Card_TCM lastCard = cards[cards.Count - 1];
        cards.RemoveAt(cards.Count - 1);
        cards.Insert(0, lastCard);
    }

    // 반시계 방향으로 회전
    private IEnumerator TurnCounterclockwise()
    {
        List<Vector2> positions = new List<Vector2>();

        foreach(Card_TCM card in cards)
        {
            positions.Add(card.rectTransform.anchoredPosition);
        }

        // 카드의 위치를 반시계 방향으로 순환
        for (int i = 0; i < cards.Count; i++)
        {
            int newIndex = (i == 0) ? cards.Count - 1 : i - 1;
            yield return cards[i].rectTransform.DOAnchorPos(positions[newIndex], duration);
            cards[i].rectTransform.anchoredPosition = positions[newIndex];
        }
        yield return new WaitForSeconds(duration);

        // 카드 순서 업데이트
        Card_TCM temp = cards[0];
        for (int i = 0; i < cards.Count - 1; i++)
        {
            cards[i] = cards[i + 1];
        }
        cards[cards.Count - 1] = temp;
    }

    // 두 카드 두 쌍 변경
    private IEnumerator ChangeCards()
    {
        List<int> indexs = new List<int>();
        for(int i = 0; i < cards.Count; i++)
        {
            indexs.Add(i);
        }

        int index_1 = indexs[Random.Range(0,indexs.Count)];
        indexs.Remove(index_1);
        int index_2 = indexs[Random.Range(0,indexs.Count)];
        indexs.Remove(index_2);
        int index_3 = indexs[Random.Range(0,indexs.Count)];
        indexs.Remove(index_3);
        int index_4 = indexs[Random.Range(0,indexs.Count)];
        indexs.Remove(index_4);

        Vector2 position_1 = cards[index_1].rectTransform.anchoredPosition;
        Vector2 position_2 = cards[index_2].rectTransform.anchoredPosition;
        Vector2 position_3 = cards[index_3].rectTransform.anchoredPosition;
        Vector2 position_4 = cards[index_4].rectTransform.anchoredPosition;

        yield return cards[index_1].rectTransform.DOAnchorPos(position_2, duration);
        yield return cards[index_2].rectTransform.DOAnchorPos(position_1, duration);
        yield return cards[index_3].rectTransform.DOAnchorPos(position_4, duration);
        yield return cards[index_4].rectTransform.DOAnchorPos(position_3, duration).WaitForCompletion();

        cards[index_1].rectTransform.anchoredPosition = position_2;
        cards[index_2].rectTransform.anchoredPosition = position_1;
        cards[index_3].rectTransform.anchoredPosition = position_4;
        cards[index_4].rectTransform.anchoredPosition = position_3;

        Card_TCM temp = cards[index_1];
        cards[index_1] = cards[index_2];
        cards[index_2] = temp;

        temp = cards[index_3];
        cards[index_3] = cards[index_4];
        cards[index_4] = temp;
    }
}

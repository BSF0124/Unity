using System.Collections;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class BlackjackManager : MonoBehaviour
{
    public Deck deck;                       // 덱
    public BlackjackField player;           // 플레이어 카드
    public BlackjackField dealer;           // 딜러 카드
    public Transform hitBtn;                // 히트 버튼
    public Transform standBtn;              // 스탠드 버튼
    public Transform winScore;              // 라운드 승리 표시 이미지
    public ScoreText_Blackjack scoreTexts;  // 점수 텍스트

    // 라운드 승리 횟수
    private int playerWin = 0;
    private int dealerWin = 0;

    private bool isPlayerTurn = true;

    // 시작 애니메이션 재생
    private void Start()
    {
        deck.transform.localScale = Vector3.zero;
        hitBtn.localScale = Vector3.zero;
        standBtn.localScale = Vector3.zero;
        scoreTexts.transform.GetChild(1).gameObject.SetActive(false);
        foreach(Transform item in winScore.GetChild(0))
        {
            item.localScale = Vector3.zero;
        }
        foreach(Transform item in winScore.GetChild(1))
        {
            item.localScale = Vector3.zero;
        }

        StartCoroutine(StartAnimation());
    }

    // 히트 버튼 동작
    public void HitButton()
    {
        if(!DualManager.isSequenceRunning && isPlayerTurn)
        {
            AudioManager.instance.PlaySfx(AudioManager.instance.sfxClips_Main[1]);
            DualManager.isSequenceRunning = true;
            hitBtn.GetChild(1).gameObject.SetActive(true);
            StartCoroutine(deck.Hit(true));
        }
    }

    // 스탠드 버튼 동작
    public void StandButton()
    {
        if(!DualManager.isSequenceRunning && isPlayerTurn)
        {
            DualManager.isSequenceRunning = true;
            standBtn.GetChild(1).gameObject.SetActive(true);
            Stand();
        }
    }

    // 스탠드
    private void Stand()
    {
        AudioManager.instance.PlaySfx(AudioManager.instance.sfxClips_Main[2]);
        if(isPlayerTurn)
        {
            DualManager.isSequenceRunning = true;
            isPlayerTurn = false;
            StartCoroutine(DealerTurn());
        }

        else
        {
            StartCoroutine(Result());
        }
    }

    // 애니메이션

    // 딜러 턴 진행
    public IEnumerator DealerTurn()
    {
        if(!scoreTexts.transform.GetChild(1).gameObject.activeSelf)
        {
            scoreTexts.transform.GetChild(1).gameObject.SetActive(true);
            yield return StartCoroutine(dealer.transform.GetChild(1).GetComponent<BlackjackCard>().Flip(CardDataManager.instance.GetCardByID(dealer.transform.GetChild(1).GetComponent<BlackjackCard>().cardID).cardSprite));
            yield return new WaitForSeconds(1f);
        }

        // 10 이하일 경우 히트
        if(dealer.score <= 10)
        {
            AudioManager.instance.PlaySfx(AudioManager.instance.sfxClips_Main[1]);
            hitBtn.GetChild(1).gameObject.SetActive(true);
            yield return StartCoroutine(deck.Hit(false));
            yield return new WaitForSeconds(1f);
            StartCoroutine(DealerTurn());
        }

        // 11일 경우 히트 또는 스탠드
        else if(dealer.score == 11)
        {
            if(Random.Range(0,2) % 2 == 0)
            {
                AudioManager.instance.PlaySfx(AudioManager.instance.sfxClips_Main[1]);
                hitBtn.GetChild(1).gameObject.SetActive(true);
                yield return StartCoroutine(deck.Hit(false));
            }
            else
            {
                standBtn.GetChild(1).gameObject.SetActive(true);
                Stand();
            }

        }

        // 12 ~ 15면 스탠드
        else if(dealer.score >= 12 && dealer.score <= 15)
        {
            standBtn.GetChild(1).gameObject.SetActive(true);
            Stand();
        }
    }

    // 시작 애니메이션
    private IEnumerator StartAnimation()
    {
        DualManager.isSequenceRunning = true;
        yield return deck.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack).WaitForCompletion();
        yield return hitBtn.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack).WaitForCompletion();
        yield return standBtn.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack).WaitForCompletion();
        StartCoroutine(Reset());
    }

    // 듀얼 결과
    private IEnumerator Result()
    {
        DualManager.isSequenceRunning = true;

        yield return new WaitForSeconds(0.5f);

        // 승리
        if(player.score > dealer.score)
        {
            hitBtn.GetChild(0).GetComponent<TextMeshProUGUI>().text = "승리";
            standBtn.GetChild(0).GetComponent<TextMeshProUGUI>().text = "승리";
            StartCoroutine(UpdateWinScoreImage(true));
        }
            
        // 패배
        else if(player.score < dealer.score)
        {
            hitBtn.GetChild(0).GetComponent<TextMeshProUGUI>().text = "패배";
            standBtn.GetChild(0).GetComponent<TextMeshProUGUI>().text = "패배";
            StartCoroutine(UpdateWinScoreImage(false));
        }

        // 무승부
        else
        {
            hitBtn.GetChild(0).GetComponent<TextMeshProUGUI>().text = "무승부";
            standBtn.GetChild(0).GetComponent<TextMeshProUGUI>().text = "무승부";
        }
        AudioManager.instance.PlaySfx(AudioManager.instance.sfxClips_DualMode[12]);
        yield return hitBtn.GetComponent<RectTransform>().DOShakeAnchorPos(0.5f, 30, 30);
        yield return standBtn.GetComponent<RectTransform>().DOShakeAnchorPos(0.5f, 30, 30).WaitForCompletion();
        yield return new WaitForSeconds(1.5f);
        StartCoroutine(Reset());
    }

    // 버스트
    public IEnumerator Bust()
    {
        DualManager.isSequenceRunning = true;
        yield return new WaitForSeconds(0.5f);

        // 텍스트 변경
        hitBtn.GetChild(0).GetComponent<TextMeshProUGUI>().text = "버스트!";
        standBtn.GetChild(0).GetComponent<TextMeshProUGUI>().text = "버스트!";
        AudioManager.instance.PlaySfx(AudioManager.instance.sfxClips_DualMode[12]);

        // 버튼 진동
        yield return hitBtn.GetComponent<RectTransform>().DOShakeAnchorPos(0.5f, 30, 30);
        yield return standBtn.GetComponent<RectTransform>().DOShakeAnchorPos(0.5f, 30, 30).WaitForCompletion();

        // 라운드 승리 증가
        StartCoroutine(UpdateWinScoreImage(!isPlayerTurn));
        yield return new WaitForSeconds(1.5f);

        StartCoroutine(Reset());
    }

    // 라운드 승리 이미지 업데이트
    private IEnumerator UpdateWinScoreImage(bool player)
    {
        if(player)
        {
            AudioManager.instance.PlaySfx(AudioManager.instance.sfxClips_DualMode[8]);
            yield return winScore.GetChild(0).GetChild(playerWin).DOScale(Vector3.one, 1f).SetEase(Ease.OutBack).WaitForCompletion();
            playerWin++;
        }
        else
        {
            AudioManager.instance.PlaySfx(AudioManager.instance.sfxClips_DualMode[9]);
            yield return winScore.GetChild(1).GetChild(dealerWin).DOScale(Vector3.one, 1f).SetEase(Ease.OutBack).WaitForCompletion();
            dealerWin++;
        }
    }

    // 초기화
    private IEnumerator Reset()
    {
        DualManager.isSequenceRunning = true;

        // 게임 오버(승리)
        if(playerWin >= 5)
        {
            transform.parent.GetComponent<DualManager>().GameOver(true);
        }
        // 게임 오버(패배)
        else if(dealerWin >= 5)
        {
            transform.parent.GetComponent<DualManager>().GameOver(false);
        }

        else
        {
            if(player.transform.childCount > 0)
            {
                AudioManager.instance.PlaySfx(AudioManager.instance.sfxClips_DualMode[3]);
            }
            // 카드 소멸
            foreach(Transform card in player.transform)
            {
                StartCoroutine(card.GetComponent<BlackjackCard>().DestroyCard());
            }
            foreach(Transform card in dealer.transform)
            {
                StartCoroutine(card.GetComponent<BlackjackCard>().DestroyCard());
            }
            yield return new WaitForSeconds(1.5f);

            // 점수, 텍스트 초기화
            scoreTexts.transform.GetChild(1).gameObject.SetActive(false);
            player.CalculateScore();
            dealer.CalculateScore();
            hitBtn.GetChild(0).GetComponent<TextMeshProUGUI>().text = "히트";
            standBtn.GetChild(0).GetComponent<TextMeshProUGUI>().text = "스탠드";

            // 카드 배분
            yield return StartCoroutine(deck.Hit(true));
            yield return StartCoroutine(deck.Hit(false));
            yield return StartCoroutine(deck.Hit(true));
            yield return StartCoroutine(deck.Hit(false));
            isPlayerTurn = true;
            DualManager.isSequenceRunning = false;
        }
    }
}

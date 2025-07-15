using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using System.Collections.Generic;

public class DualWinPanel : MonoBehaviour
{
    public RawImage background;             // 배경화면 이미지(색 변경에 사용)
    public GameObject cardPrefab;           // 카드 프리팹 
    public GameObject cardPackPrefab;           // 카드 프리팹 

    private Color backgroundColor = new Color(0.5f, 0.9f, 1f);  // 배경색
    private bool isSkipped = false;                             // 애니메이션스킵 여부 확인
    
    // 오브젝트
    private RectTransform resultPanel;       // 게임 결과 패널
    private RectTransform dualWinText;       // 듀얼 승리 텍스트
    private RectTransform acquisitionText;   // 획득 텍스트
    private HorizontalLayoutGroup cardList;  // 획득한 카드 목록 레이아웃
    private TextMeshProUGUI clickText;       // 클릭 텍스트

    // 획득 카드 관련 변수
    public float duration = 0.3f;   // 카드 생성 속도
    private bool getCardPack = false;

    private void Awake()
    {
        PauseManager.instance.canPause = false;

        if(AudioManager.instance.bgmPlayer.isPlaying)
        {
            AudioManager.instance.bgmPlayer.DOFade(0, 0.5f)
            .OnComplete(()=>AudioManager.instance.StopBgm());
        }

        resultPanel = transform.GetChild(0).GetComponent<RectTransform>();
        dualWinText = resultPanel.transform.GetChild(0).GetComponent<RectTransform>();
        acquisitionText = resultPanel.transform.GetChild(1).GetComponent<RectTransform>();
        cardList = resultPanel.transform.GetChild(2).GetComponent<HorizontalLayoutGroup>();
        clickText = transform.GetChild(1).GetComponent<TextMeshProUGUI>();

        Reset();
        List<int> cardPackStage = new List<int>(){2,7,12,17};
        if(cardPackStage.Contains(DualManager.stage.stageID))
        {
            getCardPack = true;
            if(!PlayerDataManager.instance.playerData.stage[DualManager.stage.stageID].stageClear)
            {
                for(int i=0; i<5; i++)
                {
                    GameObject cardPack = Instantiate(cardPackPrefab, cardList.transform);
                    cardPack.transform.localScale = Vector3.zero;
                }
            }
        }
        else
        {
            for(int i=0; i<DualManager.enemyDeckList.Count; i++)
            {
                GameObject card = Instantiate(cardPrefab, cardList.transform);

                int index = DualManager.stage.stageID == 18? DualManager.stage.deckList[i] : DualManager.enemyDeckList[i];
                card.GetComponent<Card>().cardImage.sprite = CardDataManager.instance.GetCardByID(index).cardSprite;
                card.transform.localScale = Vector3.zero;
            }
        }

        PlayerDataManager.instance.playerData.stage[DualManager.stage.stageID].stageClear = true;
        PlayerDataManager.instance.SaveData();

        SetLayoutGroup();
        StartCoroutine(DualWin());
    }

    // ESC를 눌러 애니메이션 스킵
    private void Update()
    {
        if(PauseManager.instance.pausePanel.activeSelf)
        {
            if(Input.GetKeyDown(KeyCode.Escape))
                PauseManager.instance.pausePanel.SetActive(false);
        }
        else
        {
            if(!isSkipped)
            {
                if(Input.GetKeyDown(KeyCode.Escape))
                {
                    AnimationSkip();
                }
            }

            else
            {
                if(Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButton(0))
                {
                    GameManager.instance.deckList.Clear();

                    List<int> indexs = new List<int>(){0,2,4,5,6,9,10,15,18};
                    if(GameManager.instance != null)
                    {
                        if(indexs.Contains(GameManager.instance.current_Stage))
                            StartCoroutine(SceneLoader.instance.LoadScene(3, 1));

                        else
                            StartCoroutine(SceneLoader.instance.LoadScene(3, 2));
                    }
                }
            }
        }
    }

    // 듀얼 승리 효과
    private IEnumerator DualWin()
    {
        yield return new WaitForSeconds(0.5f);
        AudioManager.instance.PlaySfx(AudioManager.instance.sfxClips_DualMode[0]);
        
        // 배경색이 바뀌면서 패널이 등장
        Tween colorTween = background.DOColor(backgroundColor, 0.3f);
        Tween posTween_1 = resultPanel.DOAnchorPosX(0, 0.3f).SetEase(Ease.InOutQuad);
        yield return new WaitWhile(() => 
            colorTween.IsActive() && !colorTween.IsComplete() || 
            posTween_1.IsActive() && !posTween_1.IsComplete()
        );

        // dualWinText가 왼쪽으로 움직임
        yield return dualWinText.DOAnchorPosX(-600, 0.5f).SetEase(Ease.OutBack).WaitForCompletion();
        
        yield return new WaitForSeconds(0.5f);

        // 패널의 크기 증가, dualWinText가 위로 상승
        Tween posTween_2 = resultPanel.DOSizeDelta(new Vector2(1920, 500), 0.5f);
        Tween posTween_3 = dualWinText.DOAnchorPosY(40, 0.5f);
        yield return new WaitWhile(() => 
            posTween_2.IsActive() && !posTween_2.IsComplete() || 
            posTween_3.IsActive() && !posTween_3.IsComplete()
        );

        // 획득 텍스트 등장
        yield return acquisitionText.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack).WaitForCompletion();
        // 카드 생성 효과 재생
        StartCoroutine(CreateCard());
    }

    // 카드 생성 효과
    IEnumerator CreateCard()
    {
        yield return new WaitForSeconds(0.5f);

        foreach(Transform card in cardList.transform)
        {
            AudioManager.instance.PlaySfx(AudioManager.instance.sfxClips_Main[2]);
            yield return card.DOScale(Vector3.one, duration).SetEase(Ease.OutBack).WaitForCompletion();
        }
        yield return new WaitForSeconds(0.5f);
        ClickTextFadeEffect(true);
    }

    // clickText의 Fade 루프 효과
    private void ClickTextFadeEffect(bool fadeIn)
    {
        if(!isSkipped)
            isSkipped = true;

        if(fadeIn)
        {
            clickText.DOFade(1, 1).OnComplete(()=> ClickTextFadeEffect(false));

        }
        else
        {
            clickText.DOFade(0, 1).OnComplete(()=> ClickTextFadeEffect(true));
        }
    }

    // 애니메이션 스킵 메서드
    private void AnimationSkip()
    {
        isSkipped = true;
        StopAllCoroutines();

        background.color = backgroundColor;
        resultPanel.anchoredPosition = Vector2.zero;
        resultPanel.sizeDelta = new Vector2(1920, 500);
        dualWinText.anchoredPosition = new Vector2(-600, 40);
        acquisitionText.localScale = Vector3.one;

        foreach(Transform card in cardList.transform)
        {
            card.localScale = Vector3.one;
        }

        ClickTextFadeEffect(true);
    }


    // 오브젝트들의 초기 설정 메서드
    private void Reset()
    {
        resultPanel.anchoredPosition = new Vector2(-3840,0);
        resultPanel.sizeDelta = new Vector2(1920, 100);
        dualWinText.anchoredPosition = new Vector2(-200, -40);
        acquisitionText.localScale = Vector3.zero;
        clickText.color = new Color(1,1,1,0);
    }

    // 카드 생성에 사용되는 변수 설정 메서드
    private void SetLayoutGroup()
    {
        int count = DualManager.enemyDeckList.Count;

        if(getCardPack)
        {
            cardList.spacing = 0;
            duration = 0.3f;
        }

        else
        {
            // 레이아웃 간격 조절
            if(count >= 16)
                cardList.spacing = -200;
            else if(count >= 11)
                cardList.spacing = -150;
            else if(count >= 8)
                cardList.spacing = -100;
            else if(count == 7)
                cardList.spacing = 0;
            else
                cardList.spacing = 50;

            // 카드 등장 속도 조절 
            if(count >= 15)
                duration = 0.1f;
            else if(count >= 7)
                duration = 0.2f;
            else
                duration = 0.3f;
        }
    }
}

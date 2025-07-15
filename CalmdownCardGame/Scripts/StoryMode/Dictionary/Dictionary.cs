using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using DG.Tweening;
using System.Collections.Generic;

public class Dictionary : MonoBehaviour
{
    public StoryMode storyMode;
    public DeckBuilder deckBuilder;
    public GameObject cardPrefab;           // 카드 프리팹(Displayed Card)
    public GameObject nextBtn;
    public GameObject prevBtn;
    public Image[] rarityButtons;           // 버튼 활성화 표시를 위한 희귀 버튼 이미지 배열
    public Image[] typeButtons;             // 버튼 활성화 표시를 위한 타입 버튼 이미지 배열
    public Image holographicPanel;          // 홀로그래픽 카드 효과를 위한 홀로그래픽 패널
    public HoloGraphicCard holographicCard; // 홀로그래픽 카드 효과를 위한 홀로그래픽 패널

    [HideInInspector] public CardRarity cardRarityOption;   
    [HideInInspector] public CardType cardTypeOption;       
    [HideInInspector] public DisplayedCard selectedCard;
    
    private UniversalRenderPipelineAsset urpAsset;  // 현재 사용 중인 URP Asset
    private Vector2 initialPosition;                // 클릭한 카드의 초기 좌표
    private float duration = 0.2f;                  // 홀로그래픽 효과 전환 시간

    private List<Transform> filterCards = new List<Transform>();   // 필터링된 카드 리스트
    private int currentPage = 0;                // 현재 페이지
    private int cardsPerPage = 6;               // 한 페이지에 보여질 카드 수

    private void Start()
    {
        // 현재 활성화된 RenderPipelineAsset을 UniversalRenderPipelineAsset으로 캐스팅
        urpAsset = (UniversalRenderPipelineAsset)GraphicsSettings.defaultRenderPipeline;
        cardRarityOption = CardRarity.Null;
        cardTypeOption = CardType.All;

        SetCards();
        FilterCards();
        UpdateCardList();
        EnableHDR();
    }

    // 초기 카드 생성
    void SetCards()
    {
        for(int i=0; i<30; i++)
        {
            GameObject card = Instantiate(cardPrefab, transform);
            DisplayedCard displayedCard = card.GetComponent<DisplayedCard>();
            displayedCard.Init(i);
        }
    }

    // 카드 필터링
    void FilterCards()
    {
        if(PlayerDataManager.instance != null)
        {
            filterCards.Clear();

            foreach(Transform item in transform)
            {
                DisplayedCard card = item.GetComponent<DisplayedCard>();

                if(PlayerDataManager.instance.playerData.cardOwnerships[card.cardData.cardID].quantity != 0)
                {
                    if(cardRarityOption != CardRarity.Null && card.cardData.cardRarity != cardRarityOption)
                    {
                        continue;
                    }
                    else if(cardTypeOption != CardType.All  && card.cardData.cardType == CardType.All)
                    {
                        filterCards.Add(item);
                    }
                    else if(cardTypeOption != CardType.All && card.cardData.cardType != cardTypeOption)
                    {
                        continue;
                    }
                    else
                    {
                        filterCards.Add(item);
                    }
                }
            }
        }
    }

    // 카드 목록 갱신
    void UpdateCardList()
    {
        foreach(Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }

        int startIndex = currentPage * cardsPerPage;
        int endIndex = Mathf.Min(startIndex + cardsPerPage, filterCards.Count);

        for(int i = startIndex; i < endIndex; i++)
        {
            filterCards[i].gameObject.SetActive(true);
        }

        prevBtn.SetActive(currentPage > 0);
        nextBtn.SetActive(endIndex < filterCards.Count);
    }

    public void NextButton()
    {
        if(PlayerDataManager.instance != null)
        {
            if((currentPage + 1) * cardsPerPage < filterCards.Count)
            {
                currentPage++;
                UpdateCardList();
            }
        }
        AudioManager.instance.PlaySfx(AudioManager.instance.sfxClips_Main[2]);
    }
    public void PreviousButton()
    {
        if(PlayerDataManager.instance != null)
        {
            if(currentPage > 0)
            {
                currentPage--;
                UpdateCardList();
            }
        }
        AudioManager.instance.PlaySfx(AudioManager.instance.sfxClips_Main[1]);
    }

    public void RefreshCardList()
    {
        // 카드 목록을 다시 필터링
        FilterCards();

        // 현재 페이지가 유효한지 확인 (카드 개수가 줄어든 경우 마지막 페이지에서 카드가 없을 수 있음)
        currentPage = Mathf.Clamp(currentPage, 0, (filterCards.Count - 1) / cardsPerPage);

        // 카드 목록을 다시 갱신
        UpdateCardList();
    }

    // 희귀도 옵션 설정
    public void SetRarity(int num)
    {
        if(cardRarityOption == (CardRarity)num)
            cardRarityOption = CardRarity.Null;
        else
            cardRarityOption = (CardRarity)num;
        
        SetButtonColor();
        RefreshCardList();
        AudioManager.instance.PlaySfx(AudioManager.instance.sfxClips_Main[2]);
    }

    // 타입 옵션 설정
    public void SetType(int num)
    {
        if(cardTypeOption == (CardType)num)
            cardTypeOption = CardType.All;
        else
            cardTypeOption = (CardType)num;
        
        SetButtonColor();
        RefreshCardList();
        AudioManager.instance.PlaySfx(AudioManager.instance.sfxClips_Main[2]);
    }

    // 버튼 색 변경
    private void SetButtonColor()
    {
        if(cardRarityOption == CardRarity.Null)
        {
            foreach(Image button in rarityButtons)
                button.color = new Color(1f, 1f, 1f);
        }
        else
        {
            for(int i=0; i<3; i++)
            {
                if(i == (int)cardRarityOption)
                    rarityButtons[i].color = new Color(0.5f, 1f, 0.5f);
                else
                    rarityButtons[i].color = new Color(1f, 1f, 1f);
            }
        }

        if(cardTypeOption == CardType.All)
        {
            foreach(Image button in typeButtons)
                button.color = new Color(1f, 1f, 1f);
        }
        else
        {
            for(int i=0; i<3; i++)
            {
                if(i == (int)cardTypeOption)
                    typeButtons[i].color = new Color(0.5f, 1f, 0.5f);
                else
                    typeButtons[i].color = new Color(1f, 1f, 1f);
            }
        }
    }

    // 홀로그래픽 효과 활성화
    public void HoloGraphicActivate()
    {
        if(!GameManager.instance.isSequnceActivate)
        {
            GameManager.instance.isSequnceActivate = true;
            DisableHDR();

            holographicPanel.gameObject.SetActive(true);
            holographicCard.Init(selectedCard.cardData);

            initialPosition = selectedCard.GetComponent<RectTransform>().position;
            holographicCard.rectTransform.position = initialPosition;
            holographicCard.image.color = new Color(1f,1f,1f,1f);

            selectedCard.Hide();

            Sequence sequence = DOTween.Sequence();
            sequence.Append(holographicPanel.DOFade(0.995f, duration))
                    .Join(holographicCard.GetComponent<RectTransform>().DOAnchorPos(Vector2.zero, duration))
                    .Join(holographicCard.transform.DOScale(Vector3.one*2, duration))
                    .OnComplete(()=> GameManager.instance.isSequnceActivate = false);
        }
    }

    // 홀로그래픽 효과 비활성화
    public void HoloGraphicDeactivate()
    {
        if(!GameManager.instance.isSequnceActivate)
        {
            GameManager.instance.isSequnceActivate = true;

            Sequence sequence = DOTween.Sequence();

            sequence.Append(holographicCard.GetComponent<RectTransform>().DOMove(initialPosition, duration))
                    .Join(holographicCard.transform.DOScale(Vector3.one, duration))
                    .Join(holographicPanel.DOFade(0, duration))
                    .OnComplete(()=>
                    {
                        selectedCard.Display();
                        holographicCard.image.color = new Color(1f,1f,1f,0f);
                        holographicPanel.gameObject.SetActive(false);
                        GameManager.instance.isSequnceActivate = false;
                        EnableHDR();
                    });
        }
    }

    // HDR 효과 활성화
    public void EnableHDR()
    {
        if (urpAsset != null)
        {
            urpAsset.supportsHDR = true;
        }
    }

    // HDR 효과 비활성화
    public void DisableHDR()
    {
        if (urpAsset != null)
        {
            urpAsset.supportsHDR = false;
        }
    }
}

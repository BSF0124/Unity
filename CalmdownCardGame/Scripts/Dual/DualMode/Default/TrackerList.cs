using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class TrackerList : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Sprite[] typeSprites;    // 카드 타입 스프라이트
    
    [HideInInspector] public CardStatus cardStatus;   // 카드 스텟
    [HideInInspector] public bool cardReveal = false;
    [HideInInspector] public bool grayScale = false;

    
    private Image thumbnailImage;
    private Tracker tracker;
    private TrackerCardImage trackerCardImage;

    private void Awake()
    {
        tracker = transform.parent.parent.parent.parent.GetComponent<Tracker>();
        trackerCardImage = tracker.trackerCardImage;
        thumbnailImage = transform.GetChild(0).GetChild(0).GetComponent<Image>();
    }

    // 초기 설정
    public void Init(CardStatus cardStatus)
    {
        this.cardStatus = cardStatus;

        Image rarityImage = GetComponent<Image>();
        thumbnailImage.sprite = cardStatus.cardData.thumbnailSprite;
        transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = cardStatus.cardData.cardName;
        Image typeImage = transform.GetChild(2).GetComponent<Image>();

        Color color = Color.white;
        switch(cardStatus.cardData.cardRarity)
        {
            case CardRarity.N:
                color = Color.white;
                break;
            case CardRarity.R:
                ColorUtility.TryParseHtmlString("#0063FF", out color);
                break;
            case CardRarity.SR:
                ColorUtility.TryParseHtmlString("#9400D3", out color);
                break;
            case CardRarity.Null:
                ColorUtility.TryParseHtmlString("#FF7300", out color);
                break;
        }
        rarityImage.color = color;
        typeImage.sprite = typeSprites[(int)cardStatus.cardData.cardType];
    }

    // 트래커 카드 활성화
    public void OnPointerEnter(PointerEventData eventData)
    {
        if(cardReveal)
        {
            // 트래커 카드 위치 설정
            int index = transform.GetSiblingIndex();
            float minHeight = -tracker.GetComponent<RectTransform>().sizeDelta.y/2 + trackerCardImage.GetComponent<RectTransform>().sizeDelta.y/2 + tracker.space;
            float maxHeight = tracker.GetComponent<RectTransform>().sizeDelta.y/2 - trackerCardImage.GetComponent<RectTransform>().sizeDelta.y/2 - tracker.space;
            float height = Mathf.Clamp(maxHeight - (index * (tracker.space+tracker.cardHeight)) + tracker.space, minHeight, maxHeight);

            trackerCardImage.GetComponent<RectTransform>().anchoredPosition = tracker.GetComponent<RectTransform>().anchoredPosition.x >=  0?
            new Vector2(-300f, height) : new Vector2(300f, height);
            trackerCardImage.transform.GetChild(0).GetComponent<RectTransform>().anchoredPosition = tracker.GetComponent<RectTransform>().anchoredPosition.x >=  0?
            new Vector2(-190f, 0f) : new Vector2(190f, 0f);
            
            trackerCardImage.gameObject.SetActive(true);
            trackerCardImage.ShowImage(cardStatus);
        }
    }

    // 트래커 카드 비활성화
    public void OnPointerExit(PointerEventData eventData)
    {
        if(cardReveal)
        {
            trackerCardImage.HideImage();
        }
    }

    public void CardReveal()
    {
        transform.GetChild(3).gameObject.SetActive(false);
        cardReveal = true;
    }

    public void SetGrayScale()
    {
        grayScale = true;
        CardReveal();
        transform.GetChild(2).GetComponent<Image>().sprite = typeSprites[(int)cardStatus.cardData.cardType + 4];
        thumbnailImage.sprite = cardStatus.cardData.grayThumbnailSprite;
        transform.SetAsLastSibling();
    }
}

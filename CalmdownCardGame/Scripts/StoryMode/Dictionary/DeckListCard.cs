using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class DeckListCard : MonoBehaviour, IPointerClickHandler
{
    public Sprite[] typeSprites;
    public int cardID;
    public int count = 1;

    private Image rarityImage;
    private Image typeImage;
    private Image thumbnailImage;
    private TextMeshProUGUI nameText;
    private TextMeshProUGUI countText;

    private void Awake()
    {
        rarityImage = GetComponent<Image>();
        thumbnailImage = transform.GetChild(0).GetChild(0).GetComponent<Image>();
        nameText = transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        typeImage = transform.GetChild(2).GetComponent<Image>();
        countText = transform.GetChild(3).GetComponent<TextMeshProUGUI>();

        transform.localScale = Vector3.zero;
    }

    private void Update()
    {
        if(count > 1)
        {
            countText.gameObject.SetActive(true);
        }
        else
        {
            countText.gameObject.SetActive(false);
        }

        countText.text = count.ToString();
    }

    public void Init(int cardID)
    {
        this.cardID = cardID;
        CardData cardData = CardDataManager.instance.GetCardByID(cardID);
        if(cardData != null)
        {
            nameText.text = cardData.cardName;

            Color color = Color.white;
            switch(cardData.cardRarity)
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
            typeImage.sprite = typeSprites[(int)cardData.cardType];
            thumbnailImage.sprite = cardData.thumbnailSprite;
        }

        transform.localScale = Vector3.one;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(eventData.button == PointerEventData.InputButton.Left)
        {
            transform.parent.parent.parent.parent.GetComponent<DeckBuilder>().RemoveCard(cardID);
            transform.parent.parent.parent.parent.parent.GetChild(0).GetChild(1).GetComponent<Dictionary>().RefreshCardList();
            AudioManager.instance.PlaySfx(AudioManager.instance.sfxClips_Main[1]);
        }
    }
}

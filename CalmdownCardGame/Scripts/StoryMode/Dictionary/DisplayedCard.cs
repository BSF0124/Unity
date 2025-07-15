using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using DG.Tweening;

public class DisplayedCard : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public CardData cardData;
    private TextMeshProUGUI countText;
    private Image cardImage;
    private Dictionary dictionary;

    private void Awake()
    {
        cardImage = transform.GetChild(0).GetComponent<Image>();
        countText = transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        dictionary = transform.parent.GetComponent<Dictionary>();
    }

    private void Update()
    {
        if(cardData != null)
        {
            countText.text = "x" + PlayerDataManager.instance.playerData.cardOwnerships[cardData.cardID].quantity;
        }
    }

    public void Init(int cardID)
    {
        if(CardDataManager.instance != null)
        {
            cardData = CardDataManager.instance.GetCardByID(cardID);

            if(cardData != null)
                cardImage.sprite = cardData.cardSprite;
        }
    }

    public void Hide()
    {
        Color color = cardImage.color;
        color.a = 0;
        cardImage.color = color;
    }

    public void Display()
    {
        Color color = cardImage.color;
        color.a = 1;
        cardImage.color = color;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(eventData.button == PointerEventData.InputButton.Left)
        {
            if(GameManager.instance != null && !GameManager.instance.isSequnceActivate && dictionary.storyMode.currentMenu == 0)
            {
                if(PlayerDataManager.instance.playerData.cardOwnerships[cardData.cardID].quantity > 0)
                {
                    dictionary.deckBuilder.AddCard(cardData.cardID);
                    dictionary.RefreshCardList();
                    AudioManager.instance.PlaySfx(AudioManager.instance.sfxClips_Main[2]);
                }
            }
        }

        else if(eventData.button == PointerEventData.InputButton.Right)
        {
            if(GameManager.instance != null && !GameManager.instance.isSequnceActivate && (dictionary.storyMode.currentMenu == 0 || dictionary.storyMode.currentMenu == 1))
            {
                dictionary.selectedCard = this;
                dictionary.HoloGraphicActivate();
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        cardImage.transform.DOScale(Vector3.one*1.1f, 0.3f).SetEase(Ease.OutBack);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        cardImage.transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.InBack);
    }
}
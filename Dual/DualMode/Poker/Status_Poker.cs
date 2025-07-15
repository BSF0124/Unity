using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class Status_Poker : MonoBehaviour
{
    public Image cardTypeImage;
    public Sprite[] typeSprites;

    [HideInInspector] public CardData cardData;
    [HideInInspector] public int life;

    private Tween fadeTween;

    private void Start()
    {
        GetComponent<CanvasGroup>().alpha = 0f;

        if(transform.parent.parent.GetComponent<PokerCard>() != null)
        {
            cardData = transform.parent.parent.GetComponent<PokerCard>().cardData;
            life = transform.parent.parent.GetComponent<PokerCard>().life;
        }
    }

    public void Init()
    {
        SetTypeSprite(cardData);
        string str = null;

        switch(cardData.cardRarity)
        {
            case CardRarity.N:
                str = "N";
                break;
            case CardRarity.R:
                str = "R";
                break;
            case CardRarity.SR:
                str = "SR";
                break;
            case CardRarity.Null:
                str = "SSR";
                break;
        }
        transform.GetChild(0).GetChild(1).GetChild(1).GetComponent<TextMeshProUGUI>().text = str;
        if(transform.GetChild(0).childCount >= 3)
        {
            if(transform.parent.parent.GetComponent<PokerCard>() != null)
                life = transform.parent.parent.GetComponent<PokerCard>().life;
            transform.GetChild(0).GetChild(2).GetChild(2).GetComponent<TextMeshProUGUI>().text = life.ToString();
            
            Color color;
            switch(life)
            {
                case 3:
                    color = Color.white;
                    break;
                case 2:
                    color = new Color(1,0.66f,0.66f);
                    break;
                case 1:
                    color = new Color(1,0.33f,0.33f);
                    break;
                case 0:
                    color = Color.red;
                    break;
                default:
                    color = Color.white;
                    break;
            }
            transform.GetChild(0).GetChild(2).GetChild(2).GetComponent<TextMeshProUGUI>().color = color;
        }
    }

    public void ShowStatus()
    {
        Init();
        transform.GetChild(0).gameObject.SetActive(true);
        fadeTween = GetComponent<CanvasGroup>().DOFade(1f, 0.5f);
    }

    public void HideStatus()
    {
        fadeTween.Kill();
        GetComponent<CanvasGroup>().alpha = 0f;
        transform.GetChild(0).gameObject.SetActive(false);
    }

    public void HideStatusForPokerManager()
    {
        GetComponent<CanvasGroup>().DOFade(0f, 0.5f)
            .OnComplete(()=> transform.GetChild(0).gameObject.SetActive(false));
    }

    public void StatusUpdate()
    {
        Init();
        transform.DOShakePosition(0.3f, 30, 30);
    }
    
    private void SetTypeSprite(CardData cardData)
    {
        switch(cardData.cardType)
        {
            case CardType.Rock:
                cardTypeImage.sprite = typeSprites[0];
                break;
            case CardType.Scissors:
                cardTypeImage.sprite = typeSprites[1];
                break;
            case CardType.Paper:
                cardTypeImage.sprite = typeSprites[2];
                break;
            case CardType.All:
                break;
        }
    }
}

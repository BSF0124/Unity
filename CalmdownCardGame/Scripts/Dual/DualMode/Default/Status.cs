using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class Status : MonoBehaviour
{
    public Image cardTypeImage;
    public Transform cardTypeImage_SSR;
    public Sprite[] typeSprites;

    public CardStatus cardStatus;
    private Tween fadeTween;

    private void Start()
    {
        GetComponent<CanvasGroup>().alpha = 0f;

        if(transform.parent.parent.GetComponent<PlayerCard_Defualt>() != null)
            cardStatus = transform.parent.parent.GetComponent<PlayerCard_Defualt>().cardStatus;
    }

    // 스테이터스 설정
    public void Init()
    {
        if(cardStatus.cardData.cardRarity == CardRarity.Null)
        {
            SetSSRLifeSprite(cardStatus);
        }
        else
        {
            SetTypeSprite(cardStatus);
            transform.GetChild(0).GetChild(1).GetChild(1).GetComponent<TextMeshProUGUI>().text = cardStatus.cardData.attackPower.ToString();
            transform.GetChild(0).GetChild(2).GetChild(2).GetComponent<TextMeshProUGUI>().text = cardStatus.life.ToString();
            Color color;
            switch(cardStatus.life)
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

    // 스테이터스 표시
    public void ShowStatus()
    {
        Init();
        if(cardStatus.cardData.cardRarity == CardRarity.Null)
        {
            transform.GetChild(0).gameObject.SetActive(false);
            transform.GetChild(1).gameObject.SetActive(true);
        }
        else
        {
            transform.GetChild(0).gameObject.SetActive(true);
            transform.GetChild(1).gameObject.SetActive(false);
        }

        fadeTween = GetComponent<CanvasGroup>().DOFade(1f, 0.5f);
    }

    // 스테이터스 숨기기
    public void HideStatus()
    {
        fadeTween.Kill();
        GetComponent<CanvasGroup>().alpha = 0f;
        if(cardStatus != null)
        {
            if(cardStatus.cardData.cardRarity == CardRarity.Null)
            {
                transform.GetChild(1).gameObject.SetActive(false);
            }
            else
            {
                transform.GetChild(0).gameObject.SetActive(false);
            }
        }
    }

    // DefaultManager에서 사용할 스테이터스 숨기기
    public void HideStatusForDefaultManager()
    {
        GetComponent<CanvasGroup>().DOFade(0f, 0.5f)
        .OnComplete(()=> {
                if(cardStatus.cardData.cardRarity == CardRarity.Null)
                {
                    transform.GetChild(1).gameObject.SetActive(false);
                }
                else
                {
                    transform.GetChild(0).gameObject.SetActive(false);
                }
            });
    }

    // 스텟 업데이트
    public void StatusUpdate()
    {
        Init();
        transform.DOShakePosition(0.3f, 30, 30);
    }

    // 타입 스프라이트 설정
    private void SetTypeSprite(CardStatus cardStatus)
    {
        switch(cardStatus.cardData.cardType)
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
        }
    }
    
    // SSR Life 스프라이트 설정
    private void SetSSRLifeSprite(CardStatus cardStatus)
    {
        for(int i=0; i<3; i++)
        {
            GameObject disableImage = cardTypeImage_SSR.GetChild(i).gameObject;
            if(cardStatus.ssrLife.Contains((CardType)i))
            {
                disableImage.SetActive(true);
            }
            else
            {
                disableImage.SetActive(false);
            }
        }
    }
}

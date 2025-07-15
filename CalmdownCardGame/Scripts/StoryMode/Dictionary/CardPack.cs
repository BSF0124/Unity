using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;
using TMPro;

public class CardPack : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public StoryMode storyMode;
    public Dictionary dictionary;
    public GameObject particleObject;   // 파티클 오브젝트
    public GameObject dissolveCardPack; // 카드팩 효과 담당 오브젝트
    public Image[] cards;               // 얻은 카드 이미지
    public Image dissolve_Panel;

    private bool isDissolving = false;
    private bool isEffectEnd = false;
    private bool getSRcard = false;
    private bool getSSRcard = false;

    private Image image;
    private Image dissolve_image;
    private RectTransform dissolve_Rect;

    private Color[] colors = {
        new Color(0.5f, 3f, 0.5f, 1f), 
        new Color(2.5f, 0.8f, 0.35f, 1f),
        new Color(2f,2f,2f,1f),
        new Color(0f, 1.3f, 4.3f, 1f),
        new Color(2.2f, 0f, 3f, 1f),
        new Color(3f, 0.5f, 0f, 1f)};
    private float fade = 1f;
    private float duration = 2f;

    private List<int> card_N = new List<int>() {3,4,6,13,14,15,17,18,20,21,23,26,28};
    private List<int> card_R = new List<int>() {2,8,9,10,12,19,22,24,27,29};
    private List<int> card_SR = new List<int>() {0,1,5,7,16,25};

    AnimationCurve customCurve = new AnimationCurve(
    new Keyframe(0f, 0f), // 시작: 흔들림 없음
    new Keyframe(0.8f, 0.3f), // 중간: 약한 흔들림
    new Keyframe(1f, 1f)  // 끝: 강한 흔들림
);

    void Start()
    {
        image = GetComponent<Image>();
        dissolve_image = dissolveCardPack.GetComponent<Image>();
        dissolve_Rect = dissolveCardPack.GetComponent<RectTransform>();

        dissolve_image.material = new Material(dissolve_image.material);

        dissolve_image.material.SetFloat("_Fade", 1f);
        dissolve_image.material.SetFloat("_Scale", 20f);
        dissolve_image.material.SetColor("_Color", colors[1]);

        for(int i=0; i<cards.Length; i++)
        {
            cards[i].material = new Material(cards[i].material);
            cards[i].material.SetFloat("_Fade", 0);
            cards[i].material.SetFloat("_Scale", 20);
        }
    }

    void Update()
    {
        if(isDissolving)
        {
            fade -= Time.deltaTime / 2.5f;

            if(fade <= 0f)
            {
                fade = 0f;
                isDissolving = false;
            }

            dissolve_image.material.SetFloat("_Fade", fade);
        }

        if(isEffectEnd && Input.GetKeyDown(KeyCode.Escape))
        {
            Reset();
        }
        if(PlayerDataManager.instance != null)
        {
            if(PlayerDataManager.instance.playerData.cardPack == 0)
            {
                transform.GetComponent<CanvasGroup>().alpha = 0f;
            }
            else
            {
                transform.GetComponent<CanvasGroup>().alpha = 1f;
                transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = $"x{PlayerDataManager.instance.playerData.cardPack}";
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        image.transform.DOScale(Vector3.one*1.1f, 0.3f).SetEase(Ease.OutBack);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        image.transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.InBack);
    }

    public void CardPackOpen()
    {
        if(PlayerDataManager.instance != null && PlayerDataManager.instance.playerData.cardPack > 0 && (storyMode.currentMenu == 0 || storyMode.currentMenu == 1))
        {
            PauseManager.instance.canPause = false;
            dissolve_Panel.gameObject.SetActive(true);
            image.color = new Color(1f,1f,1f,0f);

            PlayerDataManager.instance.playerData.cardPack--;
            int[] cardsID = new int[3] {Gacha(),Gacha(),Gacha()};

            for(int i=0; i<cardsID.Length; i++)
            {
                CardData card = CardDataManager.instance.GetCardByID(cardsID[i]);
                cards[i].sprite = card.cardSprite;
                cards[i].material.SetTexture("_MainTex", HoloGraphicCard.SpriteToTexture2D(card.cardSprite));
                cards[i].material.SetFloat("_Scale", 20f);
                cards[i].material.SetFloat("_Fade", 0);

                if(card.cardRarity == CardRarity.Null)
                {
                    cards[i].material.SetTexture("_Emission", HoloGraphicCard.SpriteToTexture2D(CardDataManager.instance.emissionSprites[1]));
                    cards[i].material.SetColor("_Color", colors[5]);
                }
                else
                {
                    cards[i].material.SetTexture("_Emission", HoloGraphicCard.SpriteToTexture2D(CardDataManager.instance.emissionSprites[0]));
                    
                    switch(card.cardRarity)
                    {
                        case CardRarity.N:
                            cards[i].material.SetColor("_Color", colors[2]);
                            break;
                            case CardRarity.R:
                            cards[i].material.SetColor("_Color", colors[3]);
                            break;
                        case CardRarity.SR:
                            cards[i].material.SetColor("_Color", colors[4]);
                            break;
                    }
                }

                PlayerDataManager.instance.AddCard(cardsID[i], 1);
            }

            StartCoroutine(CardPackEffect());

        }
    }

    private IEnumerator CardPackEffect()
    {
        dissolve_Panel.DOFade(0.995f, 0.5f);
        AudioManager.instance.PlaySfx(AudioManager.instance.sfxClips_Main[7]);
        dissolve_Rect.DOAnchorPos(Vector2.zero, 1f).SetEase(Ease.InOutExpo);
        yield return new WaitForSeconds(1.1f);

        AudioManager.instance.PlaySfx(AudioManager.instance.sfxClips_CardPack[2]);
        isDissolving = true;
        yield return dissolve_Rect.DOScale(1.5f, duration);
        yield return dissolve_Rect.DOShakeAnchorPos(duration, new Vector3(10f,10f,10f), 15).SetEase(customCurve);
        yield return dissolve_Rect.DOShakeRotation(duration, new Vector3(0,0,5f)).SetEase(Ease.InExpo).WaitForCompletion();
        
        if(PlayerPrefs.GetInt("DualVoice", 1) == 1)
            AudioManager.instance.PlaySfx(AudioManager.instance.sfxClips_DualWin[Random.Range(3,6)]);
        particleObject.SetActive(true);
        yield return new WaitForSeconds(0.5f);

        cards[0].gameObject.SetActive(true);
        cards[1].gameObject.SetActive(true);
        cards[2].gameObject.SetActive(true);
        AudioManager.instance.PlaySfx(AudioManager.instance.sfxClips_DualMode[2]);
        StartCoroutine(SetCardFade(cards[0], 0, 1));
        StartCoroutine(SetCardFade(cards[1], 0, 1));
        yield return StartCoroutine(SetCardFade(cards[2], 0, 1));


        if(getSSRcard)
        { 
            AudioManager.instance.PlaySfx(AudioManager.instance.sfxClips_CardPack[1]);
        }
        else if(getSRcard)
        {
            AudioManager.instance.PlaySfx(AudioManager.instance.sfxClips_CardPack[0]);
        }

        isEffectEnd = true;
    }

    private IEnumerator SetCardFade(Image image, float start, float end)
    {
        if(image.material != null)
        {
            float elapsedTime = 0f;

            image.material.SetFloat("_Fade", start);

            while(elapsedTime < 0.5f)
            {
                elapsedTime += Time.deltaTime;

                float _fade = Mathf.Lerp(start, end, elapsedTime / 0.5f);
                float _alpha = Mathf.Lerp(start, end, elapsedTime / 0.5f);
                image.material.SetFloat("_Fade", _fade);
                image.material.SetColor("_Color", new Color(image.material.color.r,image.material.color.g,image.material.color.b,_alpha));
                yield return null;
            }

            image.material.SetFloat("_Fade", end);
        }
    }

    public void Reset()
    {
        if(isEffectEnd)
        {
            isEffectEnd = false;
            isDissolving = false;
            getSRcard = false;
            getSSRcard = false;

            AudioManager.instance.PlaySfx(AudioManager.instance.sfxClips_DualMode[3]);
            StartCoroutine(SetCardFade(cards[0], 1, 0));
            StartCoroutine(SetCardFade(cards[1], 1, 0));
            StartCoroutine(SetCardFade(cards[2], 1, 0));
            dissolve_Panel.DOFade(0, 0.5f).OnComplete(()=> 
            {
                dissolve_Rect.localScale = Vector3.one;
                dissolve_Rect.anchoredPosition = new Vector2(770, 0);
                dissolve_image.material.SetFloat("_Fade", 1);
                fade = 1f;

                for(int i=0; i<cards.Length; i++)
                {
                    cards[i].material.SetFloat("_Fade", 0);
                    cards[i].gameObject.SetActive(false);
                }

                particleObject.SetActive(false);
                dissolve_Panel.gameObject.SetActive(false);
                image.color = new Color(1f,1f,1f,1f);
                dictionary.RefreshCardList();
                GameManager.instance.isSequnceActivate = false;
                PauseManager.instance.canPause = true;
            });
        }
    }

    private int Gacha()
    {
        float rand = Random.Range(0.0f, 100.0f);
        if(rand < 55.0f)
        {
            return card_N[Random.Range(0,card_N.Count)];
        }
        else if(rand < 55.0f + 34.0f)
        {
            return card_R[Random.Range(0,card_R.Count)];
        }
        else if(rand < 55.0f + 34.0f + 10.0f)
        {
            getSRcard = true;
            return card_SR[Random.Range(0,card_SR.Count)];
        }
        else
        {
            getSSRcard = true;
            return 11;
        }
    }
}

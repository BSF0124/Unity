using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class StoryMode : MonoBehaviour
{
    public RawImage background;
    public TextMeshProUGUI explainTMP;
    public int currentMenu = -1; //-1: 메인, 0: 월드맵, 1: 카드 사전, 2: 노노그램
    public float duration = 0.5f;
    public Dictionary dictionary;
    public DeckBuilder deckBuilder;

    [HideInInspector] public int buttonType;

    private string[] explainTexts = new string[3]
    {
        "월드맵\n 다양한 상대와 카드 대결",
        "내 카드\n 보유한 카드 보기 및 카드팩 뽑기",
        "노노그램\n 노노그램을 클리어하여 카드팩 획득!"
    };
    private RectTransform rectTransform;

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();

        if(GameManager.instance.isStageSelected)
        {
            rectTransform.anchoredPosition = new Vector2(0, -1080);
            background.color = new Color(0.4f, 0.7f, 1);
            GameManager.instance.isSequnceActivate = false;
            GameManager.instance.isStageSelected = false;
            currentMenu = 0;
        }
    }

    private void Update()
    {
        explainTMP.text = buttonType!=-1? explainTexts[buttonType] : "";
    }

    public void ButtonClick()
    {
        if(GameManager.instance != null && !GameManager.instance.isSequnceActivate)
        {
            GameManager.instance.isSequnceActivate = true;
            currentMenu = buttonType;
            switch(buttonType)
            {
                case 0:
                    WorldMap();
                    break;

                case 1:
                    GameManager.instance.isStageSelected = false;
                    Dictionary();
                    break;

                case 2:
                    Nonogram();
                    break;
            }
        }
        AudioManager.instance.PlaySfx(AudioManager.instance.sfxClips_Main[2]);
    }

    private void WorldMap()
    {
        Sequence sequence = DOTween.Sequence();
        sequence.Append(rectTransform.DOAnchorPos(new Vector2(0, -1080), duration))
        .Join(background.DOColor(new Color(0.4f, 0.7f, 1),duration))
        .OnComplete(()=> GameManager.instance.isSequnceActivate = false);
    }

    public void Dictionary()
    {
        dictionary.RefreshCardList();
        Sequence sequence = DOTween.Sequence();
        sequence.Append(rectTransform.DOAnchorPos(new Vector2(1920, 0), duration))
        .Join(background.DOColor(new Color(0.4f, 1, 0.7f),duration))
        .OnComplete(()=>GameManager.instance.isSequnceActivate = false);
    }

    private void Nonogram()
    {
        Sequence sequence = DOTween.Sequence();
        sequence.Append(rectTransform.DOAnchorPos(new Vector2(-1920, 0), duration))
        .Join(background.DOColor(new Color(0.7f, 0.4f, 1),duration))
        .OnComplete(()=> GameManager.instance.isSequnceActivate = false);
    }

    public void Back()
    {
        if(GameManager.instance != null && !GameManager.instance.isSequnceActivate)
        {
            GameManager.instance.isSequnceActivate = true;

            if(GameManager.instance.isStageSelected)
            {
                for(int i = GameManager.instance.deckList.Count - 1; i >= 0; i--)
                {
                    deckBuilder.RemoveCard(GameManager.instance.deckList[i]);
                }
                GameManager.instance.isStageSelected = false;
                GameManager.instance.deckList.Clear();
                WorldMap();
            }

            else
            {
                Sequence sequence = DOTween.Sequence();
                sequence.Append(rectTransform.DOAnchorPos(new Vector2(0, 0), duration))
                .Join(background.DOColor(new Color(1, 0.7f, 0.4f),duration))
                .OnComplete(()=> 
                {
                    currentMenu = -1;
                    GameManager.instance.isSequnceActivate = false;
                });
            }
        }
        AudioManager.instance.PlaySfx(AudioManager.instance.sfxClips_Main[1]);
    }
}

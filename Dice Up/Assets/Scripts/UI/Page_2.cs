using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Page_2 : MonoBehaviour
{
    public RectTransform dice; // 이동시킬 이미지의 RectTransform
    public Sprite[] diceSprites;
    public float duration = 0.5f;        // 이동 및 회전 지속 시간
    private Sequence diceSequence;
    private int currentDice = 0;

    private void Start()
    {
        CreateSequence();
    }

    private void CreateSequence()
    {
        diceSequence = DOTween.Sequence();
        diceSequence.Append(dice.DOAnchorPos(new Vector2(350, 300), duration).SetEase(Ease.Linear).OnComplete(ChangeDiceImage));
        diceSequence.Append(dice.DOAnchorPos(new Vector2(0, 650), duration).SetEase(Ease.Linear).OnComplete(ChangeDiceImage));
        diceSequence.Append(dice.DOAnchorPos(new Vector2(-350, 300), duration).SetEase(Ease.Linear).OnComplete(ChangeDiceImage));
        diceSequence.Append(dice.DOAnchorPos(new Vector2(0, -50), duration).SetEase(Ease.Linear).OnComplete(ChangeDiceImage));

        diceSequence.SetLoops(-1, LoopType.Restart)
        .SetAutoKill(false);
    }

    private void OnEnable()
    {
        diceSequence.Restart();
    }

    private void OnDisable()
    {
        diceSequence.Pause();
        objectReset();
    }

    private void ChangeDiceImage()
    {
        while(true)
        {
            int rand = Random.Range(0, diceSprites.Length);

            if(rand == currentDice)
            {
                continue;
            }
            else
            {
                currentDice = rand;
                dice.GetComponent<Image>().sprite = diceSprites[Random.Range(0, diceSprites.Length)];
                break;
            }
        }
    }

    private void objectReset()
    {
        dice.anchoredPosition = new Vector2(0, -50);
        dice.GetComponent<Image>().sprite = diceSprites[0];

    }
}

using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Page_3 : MonoBehaviour
{
    public RectTransform dice_1; // 이동시킬 이미지의 RectTransform
    public RectTransform dice_2; // 이동시킬 이미지의 RectTransform
    public RectTransform dice_3; // 이동시킬 이미지의 RectTransform
    public float duration = 1f;        // 이동 및 회전 지속 시간
    
    private Sequence diceSequence_1;
    private Sequence diceSequence_2;
    private Sequence diceSequence_3;
    private Image diceImage;

    void Start()
    {
        diceImage = dice_3.GetComponent<Image>();
        CreateSequence();
    }

    private void CreateSequence()
    {
        diceSequence_1 = DOTween.Sequence();
        diceSequence_2 = DOTween.Sequence();
        diceSequence_3 = DOTween.Sequence();

        diceSequence_1.Append(dice_1.DOAnchorPos(new Vector2(-270, 200), duration).SetEase(Ease.OutQuad))
        .Append(dice_1.DOAnchorPos(new Vector2(-270, -200), duration).SetEase(Ease.InQuad))
        .AppendInterval(0.75f);
        
        diceSequence_2.Append(dice_2.DOAnchorPos(new Vector2(0, 200), duration).SetEase(Ease.OutQuad))
        .Append(dice_2.DOAnchorPos(new Vector2(0, 600), duration).SetEase(Ease.OutQuad))
        .Append(dice_2.DOAnchorPos(new Vector2(0, -200), duration*1.5f).SetEase(Ease.InQuad));
        
        diceSequence_3.AppendCallback(() => diceImage.color = new Color(diceImage.color.r,diceImage.color.g,diceImage.color.b, 0.5f))
        .Append(dice_3.DOAnchorPos(new Vector2(270, 280), duration).SetEase(Ease.OutQuad))
        .AppendCallback(() => diceImage.color = new Color(diceImage.color.r,diceImage.color.g,diceImage.color.b, 1f))
        .Append(dice_3.DOAnchorPos(new Vector2(270, 100), duration/2).SetEase(Ease.InQuad))
        .AppendInterval(1f)
        .OnComplete(SetAnchoredPosition);

        diceSequence_1.SetLoops(-1, LoopType.Restart)
        .SetAutoKill(false);
        diceSequence_2.SetLoops(-1, LoopType.Restart)
        .SetAutoKill(false);
        diceSequence_3.SetLoops(-1, LoopType.Restart)
        .SetAutoKill(false);
    }

    private void SetAnchoredPosition()
    {
        dice_3.anchoredPosition = new Vector2(270, -200);
    }
    
    private void OnEnable()
    {
        diceSequence_1.Restart();
        diceSequence_2.Restart();
        diceSequence_3.Restart();
    }

    private void OnDisable()
    {
        diceSequence_1.Pause();
        diceSequence_2.Pause();
        diceSequence_3.Pause();
        objectReset();
    }


    private void objectReset()
    {
        dice_1.anchoredPosition = new Vector2(-270, -200);
        dice_2.anchoredPosition = new Vector2(0, -200);
        dice_3.anchoredPosition = new Vector2(270, -200);
        diceImage.color = new Color(diceImage.color.r,diceImage.color.g,diceImage.color.b, 1f);
    }
}

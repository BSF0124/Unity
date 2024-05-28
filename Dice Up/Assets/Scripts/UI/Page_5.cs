using UnityEngine;
using DG.Tweening;

public class Page_5 : MonoBehaviour
{
    public RectTransform dice_5;
    public RectTransform dice_6;
    public float duration = 1f;
    
    private Sequence diceSequence_5;
    private Sequence diceSequence_6;

    void Start()
    {
        CreateSequence();
    }

    private void CreateSequence()
    {
        diceSequence_5 = DOTween.Sequence();
        diceSequence_6 = DOTween.Sequence();

        Vector3 startPos = new Vector3(-150, -50, 0);
        Vector3 endPos = new Vector3(-630, -100, 0);
        Vector2 controlPos = (startPos + endPos) / 2 + Vector3.up * 250;
        Vector3[] path = new Vector3[] {startPos, controlPos, endPos};

        diceSequence_5.Append(dice_5.DOAnchorPos(new Vector2(-150, -50), duration/3).SetEase(Ease.OutQuad))
        .Append(dice_5.DOLocalPath(path, duration/2, PathType.CatmullRom).SetEase(Ease.OutQuad))
        .AppendInterval(1f)
        .OnComplete(SetAnchoredPosition);

        diceSequence_6.Append(dice_6.DOAnchorPos(new Vector2(270, 400), duration).SetEase(Ease.OutQuad))
        .Append(dice_6.DOAnchorPos(new Vector2(270, -200), duration).SetEase(Ease.InQuad))
        .AppendInterval(0.5f);

        diceSequence_5.SetLoops(-1, LoopType.Restart)
        .SetAutoKill(false);
        diceSequence_6.SetLoops(-1, LoopType.Restart)
        .SetAutoKill(false);
    }

    private void SetAnchoredPosition()
    {
        dice_5.anchoredPosition = new Vector2(-270, -200);
    }

    private void OnEnable()
    {
        diceSequence_5.Restart();
        diceSequence_6.Restart();
    }

    private void OnDisable()
    {
        diceSequence_5.Pause();
        diceSequence_6.Pause();
        objectReset();
    }

    private void objectReset()
    {
        dice_5.anchoredPosition = new Vector2(-270, -200);
        dice_6.anchoredPosition = new Vector2(270, -200);
    }
}

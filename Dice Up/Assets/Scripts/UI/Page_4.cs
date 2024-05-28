using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class Page_4 : MonoBehaviour
{
    public RectTransform arrow;
    public Image fill;
    public float duration = 1f;
    
    private Sequence arrowSequence;
    private Sequence fillSequence;

    void Start()
    {
        CreateSequence();
    }

    private void CreateSequence()
    {
        arrowSequence = DOTween.Sequence();
        fillSequence = DOTween.Sequence();

        arrowSequence.Append(arrow.DOAnchorPos(new Vector2(-100, 100), duration).SetEase(Ease.Linear))
        .Join(arrow.DORotate(new Vector3(0, 0, 45), duration).SetEase(Ease.Linear))
        .Append(arrow.DOAnchorPos(new Vector2(0, 150), duration).SetEase(Ease.Linear))
        .Join(arrow.DORotate(new Vector3(0, 0, 0), duration).SetEase(Ease.Linear))
        .Append(arrow.DOAnchorPos(new Vector2(100, 100), duration).SetEase(Ease.Linear))
        .Join(arrow.DORotate(new Vector3(0, 0, -45), duration).SetEase(Ease.Linear))
        .Append(arrow.DOAnchorPos(new Vector2(150, 0), duration).SetEase(Ease.Linear))
        .Join(arrow.DORotate(new Vector3(0, 0, -90), duration).SetEase(Ease.Linear))
        .Append(arrow.DOAnchorPos(new Vector2(100, 100), duration).SetEase(Ease.Linear))
        .Join(arrow.DORotate(new Vector3(0, 0, -45), duration).SetEase(Ease.Linear))
        .Append(arrow.DOAnchorPos(new Vector2(0, 150), duration).SetEase(Ease.Linear))
        .Join(arrow.DORotate(new Vector3(0, 0, 0), duration).SetEase(Ease.Linear))
        .Append(arrow.DOAnchorPos(new Vector2(-100, 100), duration).SetEase(Ease.Linear))
        .Join(arrow.DORotate(new Vector3(0, 0, 45), duration).SetEase(Ease.Linear))
        .Append(arrow.DOAnchorPos(new Vector2(-150, 0), duration).SetEase(Ease.Linear))
        .Join(arrow.DORotate(new Vector3(0, 0, 90), duration).SetEase(Ease.Linear));

        fillSequence.Append(fill.DOFillAmount(1f, duration*1.5f).SetEase(Ease.Linear))
        .Append(fill.DOFillAmount(0, duration*1.5f).SetEase(Ease.Linear));

        arrowSequence.SetLoops(-1, LoopType.Restart)
        .SetAutoKill(false);
        fillSequence.SetLoops(-1, LoopType.Restart)
        .SetAutoKill(false);
    }

    private void OnEnable()
    {
        arrowSequence.Restart();
    }

    private void OnDisable()
    {
        arrowSequence.Pause();
        objectReset();
    }

    private void objectReset()
    {
        arrow.anchoredPosition = new Vector2(-150, 0);
        arrow.rotation = Quaternion.Euler(new Vector3(0, 0, 90));
        fill.fillAmount = 0;
    }
}

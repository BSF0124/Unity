using UnityEngine;
using DG.Tweening;

public class Page_1 : MonoBehaviour
{
    public RectTransform imageTransform; // 이동시킬 이미지의 RectTransform
    public float duration = 0.5f;        // 이동 및 회전 지속 시간
    private Sequence mySequence;

    private void Start()
    {
        CreateSequence();
    }

    private void CreateSequence()
    {
        mySequence = DOTween.Sequence();

        mySequence.Append(imageTransform.DOAnchorPos(new Vector2(-200, 200), duration).SetEase(Ease.Linear))
        .Join(imageTransform.DORotate(new Vector3(0, 0, 45), duration).SetEase(Ease.Linear))
        .Append(imageTransform.DOAnchorPos(new Vector2(0, 300), duration).SetEase(Ease.Linear))
        .Join(imageTransform.DORotate(new Vector3(0, 0, 0), duration).SetEase(Ease.Linear))
        .Append(imageTransform.DOAnchorPos(new Vector2(200, 200), duration).SetEase(Ease.Linear))
        .Join(imageTransform.DORotate(new Vector3(0, 0, -45), duration).SetEase(Ease.Linear))
        .Append(imageTransform.DOAnchorPos(new Vector2(300, 0), duration).SetEase(Ease.Linear))
        .Join(imageTransform.DORotate(new Vector3(0, 0, -90), duration).SetEase(Ease.Linear))
        .Append(imageTransform.DOAnchorPos(new Vector2(200, 200), duration).SetEase(Ease.Linear))
        .Join(imageTransform.DORotate(new Vector3(0, 0, -45), duration).SetEase(Ease.Linear))
        .Append(imageTransform.DOAnchorPos(new Vector2(0, 300), duration).SetEase(Ease.Linear))
        .Join(imageTransform.DORotate(new Vector3(0, 0, 0), duration).SetEase(Ease.Linear))
        .Append(imageTransform.DOAnchorPos(new Vector2(-200, 200), duration).SetEase(Ease.Linear))
        .Join(imageTransform.DORotate(new Vector3(0, 0, 45), duration).SetEase(Ease.Linear))
        .Append(imageTransform.DOAnchorPos(new Vector2(-300, 0), duration).SetEase(Ease.Linear))
        .Join(imageTransform.DORotate(new Vector3(0, 0, 90), duration).SetEase(Ease.Linear));

        // 시퀀스 연결 및 무한 반복
        mySequence.SetLoops(-1, LoopType.Restart) // 무한 반복
        .SetAutoKill(false); // 시퀀스 완료 후 제거되지 않도록 설정
    }

    private void OnEnable()
    {
        mySequence.Restart();
    }

    private void OnDisable()
    {
        mySequence.Pause();
        objectReset();
    }

    private void objectReset()
    {
        imageTransform.anchoredPosition = new Vector2(-300, 0);
        imageTransform.rotation = Quaternion.Euler(new Vector3(0, 0, 90));
    }
}

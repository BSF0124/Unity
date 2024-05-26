using UnityEngine;
using DG.Tweening;

public class Page_1 : MonoBehaviour
{
    public RectTransform imageTransform; // 이동시킬 이미지의 RectTransform
    public float duration = 0.5f;        // 이동 및 회전 지속 시간
    private Sequence fullSequence;

    void Start()
    {
        CreateSequence();
    }

    void CreateSequence()
    {
        // 오른쪽 이동 시퀀스
        Sequence moveRightSequence = DOTween.Sequence();
        moveRightSequence.Append(imageTransform.DOAnchorPos(new Vector3(-200, 200, 0), duration).SetEase(Ease.Linear));
        moveRightSequence.Join(imageTransform.DORotate(new Vector3(0, 0, 45), duration).SetEase(Ease.Linear));

        moveRightSequence.Append(imageTransform.DOAnchorPos(new Vector3(0, 300, 0), duration).SetEase(Ease.Linear));
        moveRightSequence.Join(imageTransform.DORotate(new Vector3(0, 0, 0), duration).SetEase(Ease.Linear));

        moveRightSequence.Append(imageTransform.DOAnchorPos(new Vector3(200, 200, 0), duration).SetEase(Ease.Linear));
        moveRightSequence.Join(imageTransform.DORotate(new Vector3(0, 0, -45), duration).SetEase(Ease.Linear));

        moveRightSequence.Append(imageTransform.DOAnchorPos(new Vector3(300, 0, 0), duration).SetEase(Ease.Linear));
        moveRightSequence.Join(imageTransform.DORotate(new Vector3(0, 0, -90), duration).SetEase(Ease.Linear));

        // 왼쪽 이동 시퀀스
        Sequence moveLeftSequence = DOTween.Sequence();
        moveLeftSequence.Append(imageTransform.DOAnchorPos(new Vector3(200, 200, 0), duration).SetEase(Ease.Linear));
        moveLeftSequence.Join(imageTransform.DORotate(new Vector3(0, 0, -45), duration).SetEase(Ease.Linear));

        moveLeftSequence.Append(imageTransform.DOAnchorPos(new Vector3(0, 300, 0), duration).SetEase(Ease.Linear));
        moveLeftSequence.Join(imageTransform.DORotate(new Vector3(0, 0, 0), duration).SetEase(Ease.Linear));

        moveLeftSequence.Append(imageTransform.DOAnchorPos(new Vector3(-200, 200, 0), duration).SetEase(Ease.Linear));
        moveLeftSequence.Join(imageTransform.DORotate(new Vector3(0, 0, 45), duration).SetEase(Ease.Linear));

        moveLeftSequence.Append(imageTransform.DOAnchorPos(new Vector3(-300, 0, 0), duration).SetEase(Ease.Linear));
        moveLeftSequence.Join(imageTransform.DORotate(new Vector3(0, 0, 90), duration).SetEase(Ease.Linear));

        // 시퀀스 연결 및 무한 반복
        fullSequence = DOTween.Sequence();
        fullSequence.Append(moveRightSequence);
        fullSequence.Append(moveLeftSequence);
        fullSequence.SetLoops(-1, LoopType.Restart) // 무한 반복
                   .SetAutoKill(false); // 시퀀스 완료 후 제거되지 않도록 설정
    }

    void OnEnable()
    {
        fullSequence.Play();
    }

    void OnDisable()
    {
        fullSequence.Pause();
    }
}

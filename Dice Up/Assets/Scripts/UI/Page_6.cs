using UnityEngine;
using DG.Tweening;

public class Page_6 : MonoBehaviour
{
    public RectTransform cloneDice;
    public float duration = 1f;
    
    private Sequence cloneDiceSequence;

    void Start()
    {
        CreateSequence();
    }

    private void CreateSequence()
    {
        cloneDiceSequence = DOTween.Sequence();

        Vector3 startPos = new Vector3(-300, -200, 0);
        Vector3 endPos = new Vector3(300, -200, 0);
        Vector2 controlPos = (startPos + endPos) / 2 + Vector3.up * 250;
        Vector3[] path = new Vector3[] {startPos, controlPos, endPos};

        cloneDiceSequence.Append(cloneDice.DOLocalPath(path, duration, PathType.CatmullRom).SetEase(Ease.Linear))
        .AppendInterval(0.5f)
        .OnComplete(objectReset);

        cloneDiceSequence.SetLoops(-1, LoopType.Restart)
        .SetAutoKill(false);
    }

    private void OnEnable()
    {
        cloneDiceSequence.Restart();
    }

    private void OnDisable()
    {
        cloneDiceSequence.Pause();
        objectReset();
    }

    private void objectReset()
    {
        cloneDice.anchoredPosition = new Vector2(-300, -200);
    }
}

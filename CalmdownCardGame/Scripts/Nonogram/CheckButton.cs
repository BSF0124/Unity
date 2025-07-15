using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;

public class CheckButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject panel;

    private Tween tween;

    public void OnPointerExit(PointerEventData eventData)
    {
        tween.Kill();
        tween = panel.GetComponent<CanvasGroup>().DOFade(0f, 0.3f).OnComplete(()=>panel.gameObject.SetActive(false));
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        panel.gameObject.SetActive(true);
        tween.Kill();
        tween = panel.GetComponent<CanvasGroup>().DOFade(1f, 0.3f);
    }

}
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

public class NNG_Diff : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Image image;

    private void Awake()
    {
        image = transform.GetChild(0).GetComponent<Image>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.DOScale(Vector3.one*1.1f, 0.3f).SetEase(Ease.OutBack);
        image.color = new Color(0.5f, 1f, 0.5f);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack);
        image.color = Color.white;
    }
}

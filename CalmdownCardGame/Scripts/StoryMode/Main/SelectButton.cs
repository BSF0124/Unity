using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class SelectButton : MonoBehaviour
{
    public Sprite[] sprites;

    private StoryMode storyMode;
    private Image image;

    private void Start()
    {
        storyMode = transform.parent.parent.GetComponent<StoryMode>();
        image = GetComponent<Image>();
    }

    public void PointerEnter(int buttonType)
    {
        storyMode.buttonType = buttonType;
        image.sprite = sprites[1];
        transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack);
    }

    public void PointerExit()
    {
        storyMode.buttonType = -1;
        image.sprite = sprites[0];
        transform.DOScale(Vector3.one*0.9f, 0.3f).SetEase(Ease.InBack);
    }
}

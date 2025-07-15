using UnityEngine;
using UnityEngine.UI;

public class TrackerCardImage : MonoBehaviour
{
    private Image image;
    private Status status;

    private void Awake()
    {
        image = GetComponent<Image>();
        status = transform.GetChild(0).GetComponent<Status>();
    }

    public void ShowImage(CardStatus cardStatus)
    {
        image.sprite = cardStatus.cardData.cardSprite;
        status.cardStatus = cardStatus;
        gameObject.SetActive(true);
        status.ShowStatus();
    }

    public void HideImage()
    {
        status.HideStatus();
        gameObject.SetActive(false);
    }
}

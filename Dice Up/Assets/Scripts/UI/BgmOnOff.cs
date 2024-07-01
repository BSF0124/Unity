using UnityEngine;
using UnityEngine.UI;

public class BgmOnOff : MonoBehaviour
{
    public Sprite[] soundImages;
    private Image image;

    void Awake()
    {
        image = GetComponent<Image>();
        SetSprite();
    }

    public void _BgmOnOff()
    {
        AudioManager.instance.BgmOnOff();
        SetSprite();
    }

    private void SetSprite()
    {
        if(AudioManager.instance.isBgmOn)
            image.sprite = soundImages[0];

        else
            image.sprite = soundImages[1];
    }
}

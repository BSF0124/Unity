using UnityEngine;
using UnityEngine.UI;

public class SfxOnOff : MonoBehaviour
{
    public Sprite[] soundImages;
    private Image image;

    void Awake()
    {
        image = GetComponent<Image>();
        SetSprite();
    }

    public void _SfxOnOff()
    {
        AudioManager.instance.SfxOnOff();
        SetSprite();
    }

    private void SetSprite()
    {
        if(AudioManager.instance.isSfxOn)
            image.sprite = soundImages[0];

        else
            image.sprite = soundImages[1];
    }
}

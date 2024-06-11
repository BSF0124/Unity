using UnityEngine;
using UnityEngine.UI;

public class SoundButton : MonoBehaviour
{
    public Sprite[] soundImages;
    private Image image;
    private bool isSoundOn = true;

    void Awake()
    {
        image = GetComponent<Image>();
    }

    public void SoundOnOff()
    {
        if(isSoundOn)
        {
            isSoundOn = false;
            image.sprite = soundImages[1];
        }
        else
        {
            isSoundOn = true;
            image.sprite = soundImages[0];
        }
    }
}

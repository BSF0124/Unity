using UnityEngine;
using UnityEngine.UI;

public class ButtonEffect : MonoBehaviour
{
    [HideInInspector] public bool isSelected = false;
    [HideInInspector] public bool isClicked = false;
    private Image image;
    public Sprite[] sprites;

    private void Awake()
    {
        image = GetComponent<Image>();
        image.sprite = sprites[0];
    }

    private void Update()
    {
        if(isClicked)
        {
            image.sprite = sprites[2];
        }
        
        else if(isSelected)
        {
            image.sprite = sprites[1];
        }

        else
        {
            image.sprite = sprites[0];
        }
    }
}

using UnityEngine;
using UnityEngine.UI;

public class HowToPlay : MonoBehaviour
{
    public GameObject[] pages;
    public Image[] circles;
    public Sprite[] sprites;
    private int currentPage = 0;

    private void OnEnable()
    {
        currentPage = 0;
        PageActivate();
        SetSprite();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if(currentPage > 0)
                currentPage--;
            else
                currentPage = pages.Length-1;
                
            PageActivate();
            SetSprite();
            AudioManager.instance.PlaySfx(AudioManager.Sfx.SelectButton);
        }

        if(Input.GetKeyDown(KeyCode.RightArrow))
        {
            if(currentPage < pages.Length - 1)
                currentPage++;
            else
                currentPage = 0;

            PageActivate();
            SetSprite();
            AudioManager.instance.PlaySfx(AudioManager.Sfx.SelectButton);
        }
    }

    private void SetSprite()
    {
        for(int i=0; i<circles.Length; i++)
        {
            if(i == currentPage)
            {
                circles[i].sprite = sprites[1];
            }
            else
            {
                circles[i].sprite = sprites[0];
            }
        }
    }

    private void PageActivate()
    {
        for(int i=0; i<pages.Length; i++)
        {
            if(i == currentPage)
            {
                pages[i].SetActive(true);
            }
            else
            {
                pages[i].SetActive(false);
            }
        }
    }
}

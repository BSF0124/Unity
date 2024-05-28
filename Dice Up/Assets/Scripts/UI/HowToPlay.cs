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
        if(currentPage > 0 && Input.GetKeyDown(KeyCode.LeftArrow))
        {
            currentPage--;
            PageActivate();
            SetSprite();
        }

        if(currentPage < circles.Length && Input.GetKeyDown(KeyCode.RightArrow))
        {
            currentPage++;
            PageActivate();
            SetSprite();
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

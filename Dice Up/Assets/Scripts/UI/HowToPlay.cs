using UnityEngine;
using UnityEngine.UI;

public class HowToPlay : MonoBehaviour
{
    public Image[] circles;
    public Sprite[] sprites;
    private int currentPage = 0;

    private void Update()
    {
        if(currentPage > 0 && Input.GetKeyDown(KeyCode.LeftArrow))
        {
            currentPage--;
            SetSprite();
        }

        if(currentPage < circles.Length && Input.GetKeyDown(KeyCode.RightArrow))
        {
            currentPage++;
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
}

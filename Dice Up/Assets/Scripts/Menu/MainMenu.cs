using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public Button[] buttons;
    public Image[] images;
    public Sprite[] sprites;
    // public GameObject optionPanel;
    // public GameObject howtoplayPanel;
    private int currentButton;
    private float imagePosition_y;

    void Awake()
    {
        imagePosition_y = images[0].transform.localPosition.y;
        currentButton = -1;
        RefreshSprite();
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.UpArrow) && currentButton != 0)
        {
            currentButton--;
            RefreshSprite();
        }
        if(Input.GetKeyDown(KeyCode.DownArrow) && currentButton < 2)
        {
            currentButton++;
            RefreshSprite();
        }
        if(Input.GetKeyDown(KeyCode.LeftArrow) && currentButton == 3)
        {
            currentButton = 2;
            RefreshSprite();
        }
        if(Input.GetKeyDown(KeyCode.RightArrow) && currentButton == 2)
        {
            currentButton = 3;
            RefreshSprite();
        }
        if(Input.GetKeyDown(KeyCode.Return))
        {
            StartCoroutine(ButtonEffect());
            ButtonClick();
        }
    }

    private void RefreshSprite()
    {
        for(int i=0; i<4; i++)
        {
            if(i == currentButton)
            {
                buttons[i].GetComponent<Image>().sprite = sprites[1];
            }

            else
            {
                buttons[i].GetComponent<Image>().sprite = sprites[0];
            }
        }
    }

    public void ButtonEnter(int button)
    {
        currentButton = button;
        RefreshSprite();
    }

    public void ButtonExit()
    {
        if(currentButton != 3)
        {
            images[currentButton].transform.localPosition = new Vector3 (images[currentButton].transform.localPosition.x, 
            imagePosition_y, images[currentButton].transform.localPosition.z);
        }
        currentButton = -1;
        RefreshSprite();
    }

    public void ButtonDown()
    {
        if(currentButton != -1)
        {
            buttons[currentButton].GetComponent<Image>().sprite = sprites[2];
            if(currentButton != 3)
            {
                images[currentButton].transform.localPosition = new Vector3 (images[currentButton].transform.localPosition.x, 
                0, images[currentButton].transform.localPosition.z);
            }
        }
    }

    public void ButtonUp()
    {
        if(currentButton != -1)
        {
            buttons[currentButton].GetComponent<Image>().sprite = sprites[1];
            if(currentButton != 3)
            {
                images[currentButton].transform.localPosition = new Vector3 (images[currentButton].transform.localPosition.x, 
                imagePosition_y, images[currentButton].transform.localPosition.z);
            }
        }
    }

    public void ButtonClick()
    {
        switch(currentButton)
            {
                case 0:
                    print(0);
                    break;
                case 1:
                    print(1);
                    break;
                case 2:
                    Application.Quit();
                    break;
                case 3:
                    print(3);
                    break;
            }
    }

    IEnumerator ButtonEffect()
    {
        buttons[currentButton].GetComponent<Image>().sprite = sprites[2];
        if(currentButton != 3)
        {
            images[currentButton].transform.localPosition = new Vector3 (images[currentButton].transform.localPosition.x, 
            0, images[currentButton].transform.localPosition.z);
        }
        yield return new WaitForSeconds(0.1f);

        buttons[currentButton].GetComponent<Image>().sprite = sprites[1];
        if(currentButton != 3)
        {
            images[currentButton].transform.localPosition = new Vector3 (images[currentButton].transform.localPosition.x, 
            imagePosition_y, images[currentButton].transform.localPosition.z);
        }
    }
}

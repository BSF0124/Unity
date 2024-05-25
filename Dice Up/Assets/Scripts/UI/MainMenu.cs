using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [HideInInspector] public int currentButton;
    public ButtonEffect[] buttons;
    public GameObject howtoplayPanel;

    void Awake()
    {
        currentButton = -1;
        RefreshSprite();
    }

    void Update()
    {
        if(howtoplayPanel.activeSelf)
        {
            if(Input.GetKeyDown(KeyCode.LeftArrow))
            {

            }
            if(Input.GetKeyDown(KeyCode.RightArrow))
            {
                
            }
            if(Input.GetKeyDown(KeyCode.Escape))
            {
                howtoplayPanel.SetActive(false);
            }
        }

        else
        {
            if(Input.GetKeyDown(KeyCode.UpArrow) && currentButton > 0)
            {
                currentButton--;
                RefreshSprite();
            }
            if(Input.GetKeyDown(KeyCode.DownArrow) && currentButton < 2)
            {
                currentButton++;
                RefreshSprite();
            }
            if(Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
            {
                StartCoroutine(ButtonEffect());
                ButtonClick();
            }
            if(Input.GetKeyDown(KeyCode.Escape))
            {
                Application.Quit();
            }
        }
    }

    private void RefreshSprite()
    {
        for(int i=0; i<3; i++)
        {
            if(i == currentButton)
            {
                buttons[i].isSelected = true;
            }
            else
            {
                buttons[i].isSelected = false;
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
            }
    }

    IEnumerator ButtonEffect()
    {
        if(currentButton != -1)
        {
            int temp = currentButton;
            
            buttons[temp].isClicked = true;
            yield return new WaitForSeconds(0.1f);
            buttons[temp].isClicked = false;
            yield return new WaitForSeconds(0.1f);
            RefreshSprite();
        }
    }
}

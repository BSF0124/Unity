using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class MainMenu : MonoBehaviour
{
    public RectTransform howtoplayPanel;
    public ButtonEffect[] buttons;
    
    [HideInInspector] public int currentButton;
    private bool isCoroutineRun = false;

    void Awake()
    {
        currentButton = -1;
        RefreshSprite();
    }

    void Update()
    {
        if(isCoroutineRun)
        {return;}

        if(howtoplayPanel.gameObject.activeSelf)
        {
            if(Input.GetKeyDown(KeyCode.Escape))
            {
                StartCoroutine(HowToPlayPanelEffect(true));
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
                    StartCoroutine(SceneLoader.Instance.LoadScene("Game", LoadSceneMode.Additive));
                    break;
                case 1:
                    StartCoroutine(HowToPlayPanelEffect(false));
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

    IEnumerator HowToPlayPanelEffect(bool isActivated)
    {
        isCoroutineRun = true;
        if(isActivated)
        {
            howtoplayPanel.DOScale(0, 0.5f).SetEase(Ease.InBack);
            yield return new WaitForSeconds(0.6f);
            howtoplayPanel.gameObject.SetActive(false);
            howtoplayPanel.localScale = Vector3.zero;
        }

        else
        {
            howtoplayPanel.gameObject.SetActive(true);
            howtoplayPanel.localScale = Vector3.zero;
            howtoplayPanel.DOScale(1, 0.5f).SetEase(Ease.OutBack);
            yield return new WaitForSeconds(0.6f);
        }
        isCoroutineRun = false;
    }
}

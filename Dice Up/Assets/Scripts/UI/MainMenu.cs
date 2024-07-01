using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class MainMenu : MonoBehaviour
{
    public RectTransform howtoplayPanel;
    public RectTransform licensePanel;
    public ButtonEffect[] buttons;
    
    [HideInInspector] public int currentButton;
    private bool isCoroutineRun = false;

    void Awake()
    {
        currentButton = -1;
        RefreshSprite();
        AudioManager.instance.PlayBgm(false);
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

        else if(licensePanel.gameObject.activeSelf)
        {
            if(Input.GetKeyDown(KeyCode.Escape))
            {
                StartCoroutine(LicensePanelEffect(true));
            }
        }

        else
        {
            if(Input.GetKeyDown(KeyCode.UpArrow) && currentButton > 0)
            {
                AudioManager.instance.PlaySfx(AudioManager.Sfx.SelectButton);
                currentButton--;
                RefreshSprite();
            }
            if(Input.GetKeyDown(KeyCode.DownArrow) && currentButton < 2)
            {
                AudioManager.instance.PlaySfx(AudioManager.Sfx.SelectButton);
                currentButton++;
                RefreshSprite();
            }
            if(Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
            {
                StartCoroutine(ButtonEffect());
                ButtonClick();
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
                    StartCoroutine(SceneLoader.instance.LoadScene("Game", LoadSceneMode.Additive));
                    AudioManager.instance.PlaySfx(AudioManager.Sfx.PressButton);
                    break;
                case 1:
                    StartCoroutine(HowToPlayPanelEffect(false));
                    AudioManager.instance.PlaySfx(AudioManager.Sfx.PressButton);
                    break;
                case 2:
                    Application.Quit();
                    break;
            }
    }

    private IEnumerator ButtonEffect()
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

    private IEnumerator HowToPlayPanelEffect(bool isActivated)
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

    public void ClickLicenseButton()
    {
        StartCoroutine(LicensePanelEffect(false));
    }

    private IEnumerator LicensePanelEffect(bool isActivated)
    {
        isCoroutineRun = true;
        if(isActivated)
        {
            licensePanel.DOScale(0, 0.5f).SetEase(Ease.InBack);
            yield return new WaitForSeconds(0.6f);
            licensePanel.gameObject.SetActive(false);
            licensePanel.localScale = Vector3.zero;
        }

        else
        {
            licensePanel.gameObject.SetActive(true);
            licensePanel.localScale = Vector3.zero;
            licensePanel.DOScale(1, 0.5f).SetEase(Ease.OutBack);
            yield return new WaitForSeconds(0.6f);
        }
        isCoroutineRun = false;
    }
}

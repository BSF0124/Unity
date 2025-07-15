using UnityEngine;
using DG.Tweening;

public class Tutorial : MonoBehaviour
{
    public GameObject[] buttons;
    private int page = 0;

    private void Awake()
    {
        GetComponent<CanvasGroup>().alpha = 0f;
        PageUpdate();
        GetComponent<CanvasGroup>().DOFade(1f, 1f);
    }

    private void PageUpdate()
    {
        for(int i = 0; i < 3; i++)
        {
            if(page == i)
                transform.GetChild(i).gameObject.SetActive(true);
            else
                transform.GetChild(i).gameObject.SetActive(false);

            if(page <= 0)
            {
                buttons[0].SetActive(false);
                buttons[1].SetActive(true);
                buttons[2].SetActive(false);
            }
            else if(page == 1)
            {
                buttons[0].SetActive(true);
                buttons[1].SetActive(true);
                buttons[2].SetActive(false);
            }
            else
            {
                buttons[0].SetActive(true);
                buttons[1].SetActive(false);
                buttons[2].SetActive(true);
            }
        }
    }

    public void LeftButton()
    {
        if(page > 0)
        {
            page--;
        }
        PageUpdate();
        AudioManager.instance.PlaySfx(AudioManager.instance.sfxClips_Main[1]);
    }

    public void RightButton()
    {
        if(page < 2)
        {
            page++;
        }
        PageUpdate();
        AudioManager.instance.PlaySfx(AudioManager.instance.sfxClips_Main[2]);
    }

    public void CloseButton()
    {
        GetComponent<CanvasGroup>().DOFade(0f, 1f)
        .OnComplete(()=> 
            gameObject.SetActive(false)
        );
        AudioManager.instance.PlaySfx(AudioManager.instance.sfxClips_Main[2]);
    }
}

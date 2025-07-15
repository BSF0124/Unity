using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class CutSceneManager : MonoBehaviour
{
    public Image fadeImage;

    private void Awake()
    {
        if(GameManager.instance != null)
        {
            switch(GameManager.instance.current_Stage)
            {
                case 0:
                    transform.GetChild(0).gameObject.SetActive(true);
                    break;

                case 1:
                    transform.GetChild(1).gameObject.SetActive(true);
                    break;

                case 3:
                    transform.GetChild(2).gameObject.SetActive(true);
                    break;

                case 4:
                        transform.GetChild(3).gameObject.SetActive(true);
                    break;

                case 5:
                    transform.GetChild(4).gameObject.SetActive(true);
                    break;

                case 6:
                    transform.GetChild(5).gameObject.SetActive(true);
                    break;

                case 7:
                    if(!GameManager.instance.isStageClear)
                        transform.GetChild(6).gameObject.SetActive(true);
                    else
                        transform.GetChild(7).gameObject.SetActive(true);
                    break;

                case 8:
                    if(!GameManager.instance.isStageClear)
                        transform.GetChild(8).gameObject.SetActive(true);
                    else
                        transform.GetChild(9).gameObject.SetActive(true);
                    break;

                case 9:
                    transform.GetChild(10).gameObject.SetActive(true);
                    break;

                case 10:
                    transform.GetChild(11).gameObject.SetActive(true);
                    break;

                case 11:
                    if(!GameManager.instance.isStageClear)
                        transform.GetChild(12).gameObject.SetActive(true);
                    else
                        transform.GetChild(13).gameObject.SetActive(true);
                    break;

                case 12:
                    if(!GameManager.instance.isStageClear)
                        transform.GetChild(14).gameObject.SetActive(true);
                    else
                        transform.GetChild(15).gameObject.SetActive(true);
                    break;

                case 13:
                    if(!GameManager.instance.isStageClear)
                        transform.GetChild(16).gameObject.SetActive(true);
                    else
                        transform.GetChild(17).gameObject.SetActive(true);
                    break;

                case 14:
                    if(!GameManager.instance.isStageClear)
                        transform.GetChild(18).gameObject.SetActive(true);
                    else
                        transform.GetChild(19).gameObject.SetActive(true);
                    break;

                case 15:
                    transform.GetChild(20).gameObject.SetActive(true);
                    break;

                case 16:
                    if(!GameManager.instance.isStageClear)
                        transform.GetChild(21).gameObject.SetActive(true);
                    else
                        transform.GetChild(22).gameObject.SetActive(true);
                    break;

                case 17:
                    if(!GameManager.instance.isStageClear)
                        transform.GetChild(23).gameObject.SetActive(true);
                    else
                        transform.GetChild(24).gameObject.SetActive(true);
                    break;

                case 18:
                    transform.GetChild(25).gameObject.SetActive(true);
                    break;

                case 19:
                    if(!GameManager.instance.isStageClear)
                        transform.GetChild(26).gameObject.SetActive(true);
                    else
                        transform.GetChild(27).gameObject.SetActive(true);
                    break;
                    
                case 20:
                    if(!GameManager.instance.isStageClear)
                        transform.GetChild(28).gameObject.SetActive(true);
                    else
                        transform.GetChild(29).gameObject.SetActive(true);
                    break;
            }
        }
    }

    public void SetEpilogue()
    {
        AudioManager.instance.bgmPlayer.DOFade(0f, 0.5f);
        fadeImage.gameObject.SetActive(true);
        fadeImage.DOFade(1f, 1f)
        .OnComplete(()=> 
        {
            AudioManager.instance.StopBgm();
            transform.GetChild(29).gameObject.SetActive(false);
            transform.GetChild(30).gameObject.SetActive(true);
            fadeImage.gameObject.SetActive(false);
        });
    }
}

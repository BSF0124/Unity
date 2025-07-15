using UnityEngine;
using DG.Tweening;
public class Gimmick_Tutorial : MonoBehaviour
{
    public void Awake()
    {
        GetComponent<CanvasGroup>().alpha = 0f;
        GetComponent<CanvasGroup>().DOFade(1f, 1f);
        Invoke("Close", 5f);
    }

    public void Close()
    {
        GetComponent<CanvasGroup>().DOFade(0f, 1f)
        .OnComplete(()=> 
            gameObject.SetActive(false)
        );
    }
}

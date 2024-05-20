using UnityEngine;
using DG.Tweening;

public class Menu : MonoBehaviour
{

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void OpenMenu()
    {
        transform.DOLocalMoveX(0, 1).SetEase(Ease.InOutQuad);
    }

    public void CloseMenu()
    {
        transform.DOLocalMoveX(1080, 1).SetEase(Ease.InOutQuad);
    }
}

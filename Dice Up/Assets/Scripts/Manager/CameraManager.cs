using UnityEngine;
using DG.Tweening;
using System.Collections;

public class CameraManager: MonoBehaviour
{
    [SerializeField] GameObject dice;
    private Dice myDice;
    private Camera mainCamera;
    private Vector2 screenVector;
    private float max_Y = 0;

    private void Awake()
    {
        myDice = dice.GetComponent<Dice>();
        mainCamera = Camera.main;
        screenVector = mainCamera.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height));
        PlayerPrefs.SetFloat("screenLeft", -screenVector.x + 1f);
        PlayerPrefs.SetFloat("screenRight", screenVector.x - 1f);
        PlayerPrefs.SetFloat("CreateLine", transform.position.y + screenVector.y + 1.5f);
        PlayerPrefs.SetFloat("DeadLine", transform.position.y - screenVector.y - 1.5f);
    }
    private void Update()
    {
        if(myDice.setCamera)
        {
            StartCoroutine(SetPosition());
        }
        PlayerPrefs.SetFloat("CreateLine", transform.position.y + screenVector.y + 1.5f);
        PlayerPrefs.SetFloat("DeadLine", transform.position.y - screenVector.y - 1.5f);
    }

    // private void SetPosition()
    // {
    //     if(transform.position.y <= min_Y)
    //     {
    //         transform.position = new Vector3(transform.position.x, min_Y, transform.position.z);
    //     }

    //     if(dice != null && dice.transform.position.y >= max_Y)
    //     {
    //         max_Y = dice.transform.position.y;
    //         transform.position = new Vector3(transform.position.x, max_Y, transform.position.z);
    //     }
    // }

    IEnumerator SetPosition()
    {
        if(dice != null && dice.transform.position.y + 3f > max_Y)
        {
            max_Y = dice.transform.position.y + 3f;
            Vector3 position = new Vector3(transform.position.x, max_Y, transform.position.z);
            yield return transform.DOMove(position, 1f).SetEase(Ease.InOutSine);
        }
    }

    public void CameraShake()
    {
        transform.DOShakePosition(0.5f, 0.5f, 25, 90f);
    } 
}
using UnityEngine;
using DG.Tweening;

public class CameraManager: MonoBehaviour
{
    [SerializeField] GameObject dice;

    private Camera mainCamera;
    private Vector2 screenVector;
    private float min_Y = 0;
    private float max_Y = 0;

    private void Awake()
    {
        mainCamera = Camera.main;
        screenVector = mainCamera.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height));
        PlayerPrefs.SetFloat("screenLeft", -screenVector.x);
        PlayerPrefs.SetFloat("screenRight", screenVector.x);
        PlayerPrefs.SetFloat("CreateLine", transform.position.y + screenVector.y + 1.5f);
        PlayerPrefs.SetFloat("DeadLine", transform.position.y - screenVector.y - 1.5f);
    }
    private void Update()
    {
        SetPosition();
        PlayerPrefs.SetFloat("CreateLine", transform.position.y + screenVector.y + 1.5f);
        PlayerPrefs.SetFloat("DeadLine", transform.position.y - screenVector.y - 1.5f);
    }

    private void SetPosition()
    {
        if(transform.position.y <= min_Y)
        {
            transform.position = new Vector3(transform.position.x, min_Y, transform.position.z);
        }

        if(dice != null && dice.transform.position.y >= max_Y)
        {
            max_Y = dice.transform.position.y;
            transform.position = new Vector3(transform.position.x, max_Y, transform.position.z);
        }
    }

    public void CameraShake()
    {
        transform.DOShakePosition(0.5f, 0.5f, 25, 90f);
    } 
}
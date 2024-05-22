using UnityEngine;
using DG.Tweening;

public class CameraManager: MonoBehaviour
{
    [SerializeField] GameObject dice;
    private float min_Y = 0;
    private float max_Y = 0;

    private void Update()
    {
        SetPosition();
    }

    private void SetPosition()
    {
        if(transform.position.y <= min_Y)
        {
            transform.position = new Vector3(transform.position.x, min_Y, transform.position.z);
        }

        if(dice.transform.position.y >= max_Y)
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
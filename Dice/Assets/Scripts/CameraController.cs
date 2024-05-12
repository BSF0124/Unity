using System.Collections;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Camera mainCamera;

    public GameObject dice;         // 플레이어
    private GameObject diceObject;  // 카메라가 추적할 오브젝트     

    public float minX = 0f;   
    public float minY = 0f;   

    private float start = 7.5f;
    private float end = 15f;
    private float duration = 0.1f;

    private void Awake()
    {
        mainCamera = GetComponent<Camera>();
        diceObject = dice;
    }

    private void Update()
    {
        SetCameraPosition();
    }

    public void SetCameraPosition()
    {
        Vector3 position = new Vector3(diceObject.transform.position.x, diceObject.transform.position.y, -10);
        
        if(position.x <= minX)
        {
            position.x = minX;
        }
        if(position.y <= minY)
        {
            position.y = minY;
        }
        transform.position = position;
    }

    public void SetDiceObject(GameObject newDice)
    {
        diceObject = newDice;
    }

    public IEnumerator SetCameraSize(bool sizeUp)
    {
        if(sizeUp)
        {
            while(mainCamera.orthographicSize <= end)
            {
                mainCamera.orthographicSize += duration;
                yield return null;
            }
            mainCamera.orthographicSize = end;
        }

        else
        {
            while(mainCamera.orthographicSize >= start)
            {
                mainCamera.orthographicSize -= duration;
                yield return null;
            }
            mainCamera.orthographicSize = start;
        }
    }
}

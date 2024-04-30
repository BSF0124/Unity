using System.Collections;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private GameObject dice;
    private Camera mainCamera;

    private float start = 7.5f;
    private float end = 10f;
    private float duration = 0.1f;

    private void Awake()
    {
        mainCamera = GetComponent<Camera>();
    }

    private void Update()
    {
        SetCameraPosition(dice);
    }

    public void SetCameraPosition(GameObject diceObject)
    {
        Vector3 position = new Vector3(diceObject.transform.position.x, diceObject.transform.position.y, -10);
        transform.position = position;
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

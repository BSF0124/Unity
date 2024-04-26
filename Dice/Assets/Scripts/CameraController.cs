using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private GameObject dice;

    private Camera mainCamera;

    private void Awake()
    {
        mainCamera = GetComponent<Camera>();
    }

    private void Update()
    {
        SetupCamera();
    }

    public void SetupCamera()
    {
        Vector3 position = new Vector3(dice.transform.position.x, dice.transform.position.y, -10);
        transform.position = position;
    }
}

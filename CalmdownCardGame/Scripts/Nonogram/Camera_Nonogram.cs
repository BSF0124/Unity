using UnityEngine;

public class Camera_Nonogram : MonoBehaviour
{
    public GridManager gridManager;
    private Camera camera;
    private Vector3 lastMousePosition;

    private float cameraMoveSpeed = 25f;
    private float scrollSpeed = 12000.0f;
    private float maxValue = 100;
    private float minValue = 50;

    private void Start()
    {
        camera = GetComponent<Camera>();
        int x = gridManager.rowHintSize;
        int y = gridManager.columnHintSize;

        switch(gridManager.columns)
        {
            case 10:
                transform.position = new Vector2(24-2*x, 1+5*y);
                camera.fieldOfView = 55f + y * 5;
                maxValue = 100f;
                minValue = 50f;
                break;

            case 15:
                transform.position = new Vector2(42-3*x, 2+5*y);
                camera.fieldOfView = 82.5f + y * 2.5f;
                maxValue = 120f;
                minValue = 60f;
                break;

            case 20:
                transform.position = new Vector2(56-3*x, -1+5.5f*y);
                camera.fieldOfView = 98f + y * 2f;
                maxValue = 140f;
                minValue = 70f;
                break;
        }
    }

    private void Update()
    {
        float scroollWheel = Input.GetAxis("Mouse ScrollWheel");
        float value = camera.fieldOfView - scroollWheel * Time.deltaTime * scrollSpeed;
        camera.fieldOfView = Mathf.Clamp(value, minValue, maxValue);

        if(Input.GetMouseButtonDown(2))
        {
            lastMousePosition = Input.mousePosition;
        }

        if(Input.GetMouseButton(2))
        {
            Vector3 delta = Input.mousePosition - lastMousePosition;
            Vector3 move = new Vector3(-delta.x, -delta.y, 0) * Time.deltaTime * cameraMoveSpeed;
            transform.Translate(move, Space.Self);
            lastMousePosition = Input.mousePosition;
        }

        transform.position = new Vector3(
            Mathf.Clamp(transform.position.x, -100.0f, 100.0f),
            Mathf.Clamp(transform.position.y, -100.0f, 100.0f),
            transform.position.z
        );
    }
}
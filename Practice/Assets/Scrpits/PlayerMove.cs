using TMPro;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    private Vector2 screenLeft;
    private Vector2 screenRight;
    private Vector2 screenTop;
    private Vector2 screenBottom;

    public TextMeshProUGUI leftText;
    public TextMeshProUGUI rightText;
    public TextMeshProUGUI topText;
    public TextMeshProUGUI bottomText;
    public TextMeshProUGUI playerText;

    public float moveSpeed = 5f;
    private float horizontal;
    private float vertical;
    private Rigidbody2D rb;


    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        mainCamera = Camera.main;

        // screenWidth = mainCamera.ScreenToViewportPoint(new Vector3(Screen.width, 0f, 0f)).x
        // - mainCamera.ScreenToViewportPoint(Vector3.zero).x;

        screenRight = mainCamera.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height*0.5f));
        screenLeft = -screenRight;
        screenTop = mainCamera.ScreenToWorldPoint(new Vector2(Screen.width*0.5f, Screen.height));
        screenBottom = -screenTop;

        leftText.text = screenLeft.ToString();
        rightText.text = screenRight.ToString();
        topText.text = screenTop.ToString();
        bottomText.text = screenBottom.ToString();
    }

    private void Update() 
    {
        // WrapScreen();
        textSet();

        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");
        rb.velocity = new Vector2(horizontal * moveSpeed, vertical * moveSpeed);
    }

    private void textSet()
    {
        Vector2 position = transform.position;
        playerText.text = position.ToString();
    }

    private void WrapScreen()
    {
        // Vector3 currentPosition = transform.position;
        // Vector3 viewportPosition = mainCamera.WorldToViewportPoint(currentPosition);

        // if(viewportPosition.x < 0)
        // {
        //     currentPosition.x += screenWidth;
        // }
        // else if(viewportPosition.x > 1)
        // {
        //     currentPosition.x -= screenWidth;
        // }

        // transform.position = currentPosition;
    }
}

using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public float moveSpeed = 5f;            // 이동 속도
    private float horizontal;               // 좌,우 방향키 입력 확인
    private float vertical;                 // 상,하 방향키 입력 확인
    private Rigidbody2D rb2D;               // 플레이어 이동을 위한 Rigidbody2D 컴포넌트
    private PlayerManager playerManager;
    
    // 화면 크기
    public float screenLeft;
    public float screenRight;
    public float screenTop;
    public float screenBottom;

    // 플레이어 오브젝트 크기의 절반값
    public float width;
    public float height;

    private void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();
        width = GetComponent<Collider2D>().bounds.extents.x;
        height = GetComponent<Collider2D>().bounds.extents.y;
        playerManager = transform.parent.GetComponent<PlayerManager>();

    }

    private void Update()
    {
        CheckPosition();

        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");
        rb2D.velocity = new Vector2(horizontal * moveSpeed, vertical * moveSpeed);
    }

    // 위치 확인
    private void CheckPosition()
    {
        // 화면 밖으로 벗어남 (좌,우)
        if(transform.position.x < screenLeft - width || transform.position.x > screenRight + width)
        {
            playerManager.isVertical = false;
            playerManager.objectList.Remove(gameObject);
            Destroy(gameObject);

        }

        // 화면 밖으로 벗어남 (상,하)
        if(transform.position.y < screenBottom - height || transform.position.y > screenTop + height)
        {
            playerManager.isHorizontal = false;
            playerManager.objectList.Remove(gameObject);
            Destroy(gameObject);
        }

        // 오브젝트가 좌,우 모서리에 위치
        if((transform.position.x < screenLeft + width && transform.position.x > screenLeft - width) ||
        (transform.position.x > screenRight - width && transform.position.x < screenRight + width))
        {
            playerManager.CreateVertical(transform.position);
            playerManager.isVertical = true;
        }

        // 오브젝트가 상,하 모서리에 위치
        if((transform.position.y > screenTop - height && transform.position.y < screenTop + height) ||
        (transform.position.y < screenBottom + height && transform.position.y > screenBottom - height))
        {
            playerManager.CreateHorizontal(transform.position);
            playerManager.isHorizontal = true;
        }
    }
}

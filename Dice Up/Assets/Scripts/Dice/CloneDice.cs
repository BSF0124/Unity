using System.Collections; 
using UnityEngine;

public class CloneDice : MonoBehaviour
{
    [SerializeField] private Transform leftwallCheck; // 왼쪽 벽 체크
    [SerializeField] private Transform rightwallCheck; // 오른쪽 벽 체크
    [SerializeField] private LayerMask wallLayer;
    public Sprite[] diceSprites;

    [HideInInspector] public Vector2 jumpDirection;
    [HideInInspector] public float objectWidth;
    [HideInInspector] public float objectHeight;
    [HideInInspector] public bool isDiceRoll = false;
    [HideInInspector] public bool isWallJumping = false; // 벽점프 가능 여부 확인

    private Rigidbody2D rb; // rigidbodt2D 컴포넌트
    private SpriteRenderer diceSprite;

    private float radious = 0.4f;
    private float jumpForce = 700; // 점프 힘
    private float timer = 1f;
    private float currentTime = 0; // 현재 시간
    private bool isGrounded = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        diceSprite = GetComponent<SpriteRenderer>();
        objectWidth = GetComponent<Collider2D>().bounds.extents.x;
        objectHeight = GetComponent<Collider2D>().bounds.extents.y;
    }

    private void Update()
    {
        if(currentTime >= timer)
        {
            Destroy(gameObject);
        }

        CheckPosition();
        if(!GetComponent<BoxCollider2D>().isActiveAndEnabled && rb.velocity.y < 0)
        {
            GetComponent<BoxCollider2D>().enabled = true;
        }

        if(isGrounded)
        {
            currentTime += Time.deltaTime;
        }

    }

    private void CheckPosition()
    {
        // 화면 밖으로 벗어남 (좌,우)
        if(transform.position.x < PlayerPrefs.GetFloat("screenLeft") - objectWidth)
        {
            transform.position = new Vector2(PlayerPrefs.GetFloat("screenRight"), transform.position.y);
        }

        if(transform.position.x > PlayerPrefs.GetFloat("screenRight") + objectWidth)
        {
            transform.position = new Vector2(PlayerPrefs.GetFloat("screenLeft"), transform.position.y);
        }

        if(transform.position.y + objectHeight <= PlayerPrefs.GetFloat("DeadLine") + 1f)
        {
            Destroy(gameObject);
        }
    }

    // 벽점프
    private void OnCollisionEnter2D(Collision2D other) 
    {
        if(IsWalled() && isWallJumping)
        {
            // isWallJumping = false;
            jumpDirection.x *= -1;
            rb.velocity = new Vector2(0,0);
            StartCoroutine(Jump(jumpForce));
        }
    }

    private void OnCollisionStay2D(Collision2D other) {
        isGrounded = true;
    }

    private void OnCollisionExit2D(Collision2D other) {
        isGrounded = false;
    }

    // 벽에 닿았는지 확인하는 메서드(벽에 닿았으면 true 리턴)
    private bool IsWalled()
    {
        return Physics2D.OverlapCircle(leftwallCheck.position, radious, wallLayer) || Physics2D.OverlapCircle(rightwallCheck.position, radious, wallLayer);
    }

    // 점프 메서드
    public IEnumerator DoJump(int jumpType)
    {
        diceSprite.sprite = diceSprites[jumpType];
        switch(jumpType)
        {
            case 0:
                StartCoroutine(Jump(jumpForce));
                break;

            case 1:
                StartCoroutine(DoubleJump());
                break;

            case 2:
                StartCoroutine(Transparent());
                break;

            case 3:
                StartCoroutine(WallJump());
                break;
            
            case 4:
                StartCoroutine(SuperJump());
                break;
        }
        yield return null;
    }

    /// <summary>
    /// 일반 점프 (1눈)
    /// </summary>
    public IEnumerator Jump(float jumpForce)
    {
        rb.AddForce(jumpDirection * jumpForce);
        yield return new WaitForSeconds(0.1f);
    }
    /// <summary>
    /// 더블 점프 (2눈)
    /// </summary>
    public IEnumerator DoubleJump()
    {
        StartCoroutine(Jump(jumpForce));
        yield return new WaitForSeconds(0.5f);
        rb.velocity = Vector3.zero;
        StartCoroutine(Jump(jumpForce));
    }
    /// <summary>
    /// (3눈)
    /// </summary>
    public IEnumerator Transparent()
    {
        GetComponent<BoxCollider2D>().enabled = false;
        StartCoroutine(Jump(jumpForce * 1.2f));
        yield return null;
    }
    /// <summary>
    /// 벽 점프 (5눈)
    /// </summary>
    public IEnumerator WallJump()
    {
        yield return null;
        isWallJumping = true;
        StartCoroutine(Jump(jumpForce));
    }
    /// <summary>
    /// 슈퍼 점프 (6눈)
    /// </summary>
    public IEnumerator SuperJump()
    {
        yield return null;
        StartCoroutine(Jump(jumpForce*1.5f));
    }
}
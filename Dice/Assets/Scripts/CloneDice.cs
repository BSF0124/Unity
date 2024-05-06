using System.Collections;
using UnityEngine;

public class CloneDice : MonoBehaviour
{
    [SerializeField] private Transform leftwallCheck;
    [SerializeField] private Transform rightwallCheck;
    [SerializeField] private float radious = 0.2f;
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private Sprite[] diceImages;
    [HideInInspector] public Vector2 jumpDirection;
    private bool isJumping = true;    // 점프중인지 체크
    private bool isWallJumping = false;
    private float jumpForce = 700;
    private float jumpCoolDownTime = 1f;
    private float lastJumpTime;
    private float currentTime;
    private Rigidbody2D rb;
    private CameraController mainCamera;
    private PlayerController playerController;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        mainCamera = GameObject.Find("Main Camera").GetComponent<CameraController>();
        playerController = GameObject.Find("Dice").GetComponent<PlayerController>();
    }

    private void Update()
    {
        currentTime = Time.time;

        if(!isJumping)
        {
            if(currentTime - lastJumpTime > jumpCoolDownTime)
            {
                StartCoroutine(DestroyClone());
            }
        }
        else
        {
            lastJumpTime = currentTime;
        }
    }

    public void SetSprite(int type)
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = diceImages[type];
    }

    // 벽점프
    private void OnCollisionEnter2D(Collision2D other) 
    {
        if(IsWalled() && isWallJumping)
        {
            isWallJumping = false;
            jumpDirection.x *= -1;
            rb.velocity = new Vector2(0,0);
            StartCoroutine(Jump(jumpForce));
            jumpDirection.x *= -1;
        }
    }

    // 땅 위에 있으면 isJumping = false
    private void OnCollisionStay2D(Collision2D other)
    {
        isJumping = false;
    }

    // 땅에서 벗어나면 isJumping = true
    private void OnCollisionExit2D(Collision2D other) 
    {
        isJumping = true;
    }

    // 벽에 닿았는지 확인하는 메서드(벽에 닿았으면 true 리턴)
    private bool IsWalled()
    {
        return Physics2D.OverlapCircle(leftwallCheck.position, radious, wallLayer) || Physics2D.OverlapCircle(rightwallCheck.position, radious, wallLayer);
    }

    /// <summary>
    /// 점프 메서드
    /// </summary>
    public void DoJump(int jumpType)
    {
        switch(jumpType)
        {
            case 0:
                StartCoroutine(Jump(jumpForce));
                break;

            case 1:
                StartCoroutine(DoubleJump());
                break;

            case 2:
                StartCoroutine(WallJump());
                break;
            
            case 3:
                StartCoroutine(SuperJump());
                break;
        }
    }

    /// <summary>
    /// 일반 점프
    /// </summary>
    private IEnumerator Jump(float jumpForce)
    {
        rb.AddForce(jumpDirection * jumpForce);
        yield return new WaitForSeconds(0.1f);
    }

    /// <summary>
    /// 더블 점프
    /// </summary>
    private IEnumerator DoubleJump()
    {
        jumpForce = 700;
        StartCoroutine(Jump(jumpForce));
        yield return new WaitForSeconds(0.5f);
        rb.velocity = Vector3.zero;
        StartCoroutine(Jump(jumpForce));
    }

    /// <summary>
    /// 벽 점프
    /// </summary>
    private IEnumerator WallJump()
    {
        yield return null;
        isWallJumping = true;
        StartCoroutine(Jump(jumpForce));
    }

    /// <summary>
    /// 슈퍼 점프
    /// </summary>
    private IEnumerator SuperJump()
    {
        yield return null;
        StartCoroutine(Jump(jumpForce*1.5f));
    }

    /// <summary>
    /// 클론 주사위 제거 및 카메라 전환
    /// </summary>
    private IEnumerator DestroyClone()
    {
        mainCamera.SetDiceObject(mainCamera.dice);
        yield return StartCoroutine(mainCamera.SetCameraSize(false));
        playerController.isCloneJumping = false;
        Destroy(gameObject);
    }
}

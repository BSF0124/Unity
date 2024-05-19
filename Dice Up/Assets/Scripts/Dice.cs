using System;
using System.Collections; 
using UnityEngine;

public class Dice : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;       // UIManager 컴포넌트를 가지고 있는 게임 오브젝트
    [SerializeField] private GameObject arrow;          // 점프 방향을 나타내는 이미지 오브젝트
    [SerializeField] private Transform leftwallCheck;   // 왼쪽 벽 체크
    [SerializeField] private Transform rightwallCheck;  // 오른쪽 벽 체크
    [SerializeField] private LayerMask wallLayer;

    // 플레이어 오브젝트의 크기
    [HideInInspector] public float objectWidth;

    private PlayerManager playerManager;
    private Rigidbody2D rb;                             // rigidbodt2D 컴포넌트
    private Vector2 jumpDirection;

    public bool isJumping = false;                     // 점프중인지 체크
    public bool isWallJumping = false;                 // 벽점프 가능 여부 확인
    private bool isCoroutineRun = false;

    private float radious = 0.2f;
    private float jumpForce = 700;                      // 점프 힘
    private float jumpCoolDownTime = 1f;                // 점프 쿨타임
    private float lastJumpTime;                         // 마지막으로 점프한 시간
    private float currentTime;                          // 현재 시간

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        playerManager = transform.parent.GetComponent<PlayerManager>();
        objectWidth = GetComponent<Collider2D>().bounds.extents.x;
        jumpDirection = playerManager.jumpDirection;
    }

    private void Update()
    {
        jumpDirection = playerManager.jumpDirection;
        CheckPosition();
        SetArrowTransform();
        currentTime = Time.time;

        if(isCoroutineRun) {return;}

        if(!isJumping && !playerManager.isDiceRoll)
        {
            if(currentTime - lastJumpTime > jumpCoolDownTime)
            {
                isWallJumping = false;
                playerManager.jump = true;
            }
        }
        else
        {
            lastJumpTime = currentTime;
            playerManager.jump = false;
        }
    }

    private void CheckPosition()
    {
        // 화면 밖으로 벗어남 (좌,우)
        if(transform.position.x < -playerManager.screenSize + objectWidth || transform.position.x > playerManager.screenRight - objectWidth)
        {
            Destroy(gameObject);
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

    // 벽에 닿았는지 확인하는 메서드(벽에 닿았으면 true 리턴)
    private bool IsWalled()
    {
        return Physics2D.OverlapCircle(leftwallCheck.position, radious, wallLayer) || Physics2D.OverlapCircle(rightwallCheck.position, radious, wallLayer);
    }

    /// <summary>
    /// 화살표 위치, 각도 설정
    /// </summary>
    private void SetArrowTransform()
    {
        if(!isJumping && !playerManager.isDiceRoll && currentTime - lastJumpTime > jumpCoolDownTime)
        {
            arrow.SetActive(true);
            
            // 위치
            if(jumpDirection.x >= 0.5f || jumpDirection.x <= -0.5f)
            {
                arrow.transform.localPosition = jumpDirection.x>0? new Vector2(jumpDirection.x+0.5f, (1-Mathf.Abs(jumpDirection.x))*2) : new Vector2(jumpDirection.x-0.5f, (1-Mathf.Abs(jumpDirection.x))*2);
            }
            else
            {
                arrow.transform.localPosition = new Vector2(jumpDirection.x*2, 1.5f-Mathf.Abs(jumpDirection.x));
            }

            // 각도
            arrow.transform.rotation = Quaternion.Euler(0,0,jumpDirection.x * -90f);
        }
        else
        {
            arrow.SetActive(false);
        }
    }

    /// <summary>
    /// 일반 점프 (1눈)
    /// </summary>
    public IEnumerator Jump(float jumpForce)
    {
        isJumping = true;
        lastJumpTime = Time.time;
        rb.AddForce(jumpDirection * jumpForce);
        yield return new WaitForSeconds(0.1f);
    }
    /// <summary>
    /// 더블 점프 (2눈)
    /// </summary>
    public IEnumerator DoubleJump()
    {
        jumpForce = 700;
        StartCoroutine(Jump(jumpForce));
        yield return new WaitForSeconds(0.5f);
        rb.velocity = Vector3.zero;
        StartCoroutine(Jump(jumpForce));
    }
    /// <summary>
    /// 클론 생성 (3눈)
    /// </summary>
    public IEnumerator Clone()
    {
        yield return null;
    }
    /// <summary>
    /// 랜덤 점프 (4눈)
    /// </summary>
    public IEnumerator RandomJump()
    {
        isCoroutineRun = true;
        yield return StartCoroutine(ArrowBackandForth());
        yield return StartCoroutine(ArrowBackandForth());

        float direction = UnityEngine.Random.Range(-0.8f, 0.8f);
        jumpForce = UnityEngine.Random.Range(500, 1000);

        yield return StartCoroutine(SetArrowCoroutine(direction));

        StartCoroutine(Jump(jumpForce));
        isCoroutineRun = false;
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

    // 화살표 이동 코루틴
    private IEnumerator SetArrowCoroutine(float direction)
    {
        while(jumpDirection.x >= direction)
        {
            jumpDirection.x -= 0.1f;
            yield return null;
        }
        jumpDirection.x = direction;

        yield return new WaitForSeconds(0.5f);
    }
    private IEnumerator ArrowBackandForth()
    {
        while(jumpDirection.x > -0.8f)
        {
            jumpDirection.x -= 0.1f;
            yield return null;
        }
        jumpDirection.x = -0.8f;

        while(jumpDirection.x < 0.8f)
        {
            jumpDirection.x += 0.1f;
            yield return null;
        }
        jumpDirection.x = 0.8f;
    }
}
using System;
using System.Collections; 
using UnityEngine;
using UnityEngine.SceneManagement;

public class Dice : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private GameManager gameManager;       // UIManager 컴포넌트를 가지고 있는 게임 오브젝트
    [SerializeField] private GameObject arrow;          // 점프 방향을 나타내는 이미지 오브젝트
    [SerializeField] private GameObject landingEffect;
    [SerializeField] private Transform leftwallCheck;   // 왼쪽 벽 체크
    [SerializeField] private Transform rightwallCheck;  // 오른쪽 벽 체크
    [SerializeField] private LayerMask wallLayer;

    [HideInInspector] public float objectWidth;
    [HideInInspector] public bool isDiceRoll = false;

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
    private float deathLimitY = -10;                    // y좌표 제한

    private Vector2 screenVector;
    private float screenLeft;
    private float screenRight;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        objectWidth = GetComponent<Collider2D>().bounds.extents.x;
        jumpDirection = new Vector2(0, 1);

        mainCamera = Camera.main;
        screenVector = mainCamera.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height));
        screenLeft = -screenVector.x;
        screenRight = screenVector.x;
    }

    private void Update()
    {
        SetArrowTransform();
        CheckPosition();

        if(transform.position.y <= deathLimitY)
            SceneManager.LoadScene(0);

        currentTime = Time.time;

        if(isCoroutineRun) {return;}

        if(!isJumping && !isDiceRoll)
        {
            if(currentTime - lastJumpTime > jumpCoolDownTime)
            {
                SetJumpDirection();
                isWallJumping = false;
                if(Input.GetKeyDown(KeyCode.Space))
                {
                    gameManager.rollDicePanelOnOff();
                }
            }
        }
        else
        {
            lastJumpTime = currentTime;
        }
    }

    private void CheckPosition()
    {
        // 화면 밖으로 벗어남 (좌,우)
        if(transform.position.x < screenLeft - objectWidth)
        {
            transform.position = new Vector2(screenRight, transform.position.y);
        }

        if(transform.position.x > screenRight + objectWidth)
        {
            transform.position = new Vector2(screenLeft, transform.position.y);
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
        else
        {
            Instantiate(landingEffect, transform.position, Quaternion.identity);
            mainCamera.GetComponent<CameraManager>().CameraShake();

        }
    }

    // 벽에 닿았는지 확인하는 메서드(벽에 닿았으면 true 리턴)
    private bool IsWalled()
    {
        return Physics2D.OverlapCircle(leftwallCheck.position, radious, wallLayer) || Physics2D.OverlapCircle(rightwallCheck.position, radious, wallLayer);
    }

    /// <summary>
    /// 점프 방향 변경
    /// </summary>
    private void SetJumpDirection()
    {
        if(Input.GetKey(KeyCode.LeftArrow) && jumpDirection.x > -0.8)
        {
            jumpDirection.x += -0.01f;
            jumpDirection.y = (float)Math.Sqrt(1 - (jumpDirection.x * jumpDirection.x));
        }
        if(Input.GetKey(KeyCode.RightArrow) && jumpDirection.x < 0.8)
        {
            jumpDirection.x += 0.01f;
            jumpDirection.y = (float)Math.Sqrt(1 - (jumpDirection.x * jumpDirection.x));
        }
        // 점프 방향 보정
        if(jumpDirection.x > 0.8f)
        {
            jumpDirection.x = 0.8f;;
        }
        if(jumpDirection.x < -0.8f)
        {
            jumpDirection.x = -0.8f;
        }
    }

    /// <summary>
    /// 화살표 위치, 각도 설정
    /// </summary>
    private void SetArrowTransform()
    {
        if(!isJumping && !isDiceRoll && currentTime - lastJumpTime > jumpCoolDownTime)
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

    // 점프 메서드
    public IEnumerator DoJump(int jumpType)
    {
        yield return new WaitForSeconds(1f);
        switch(jumpType)
        {
            case 1:
                StartCoroutine(Jump(jumpForce));
                break;

            case 2:
                StartCoroutine(DoubleJump());
                break;

            case 3:
                StartCoroutine(Clone());
                break;

            case 4:
                StartCoroutine(RandomJump());
                break;

            case 5:
                StartCoroutine(WallJump());
                break;
            
            case 6:
                StartCoroutine(SuperJump());
                break;
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
    /// (3눈)
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
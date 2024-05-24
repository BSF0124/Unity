using System;
using System.Collections; 
using UnityEngine;
using TMPro;

public class Dice : MonoBehaviour
{
    [SerializeField] private CameraManager mainCamera;
    [SerializeField] private GameManager gameManager;       // UIManager 컴포넌트를 가지고 있는 게임 오브젝트
    [SerializeField] private GameObject arrow;          // 점프 방향을 나타내는 이미지 오브젝트
    [SerializeField] private GameObject landingEffect;
    [SerializeField] private Transform leftwallCheck;   // 왼쪽 벽 체크
    [SerializeField] private Transform rightwallCheck;  // 오른쪽 벽 체크
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] TextMeshProUGUI scoreText;


    [HideInInspector] public float objectWidth;
    [HideInInspector] public float objectHeight;
    [HideInInspector] public bool isDiceRoll = false;

    private Rigidbody2D rb;                             // rigidbodt2D 컴포넌트
    private Vector2 jumpDirection;

    public bool isJumping = false;                     // 점프중인지 체크
    public bool isWallJumping = false;                 // 벽점프 가능 여부 확인
    private bool isCoroutineRun = false;
    private bool scoreCheck = false;

    private float radious = 0.2f;
    private float jumpForce = 700;                      // 점프 힘
    private float jumpCoolDownTime = 0.5f;                // 점프 쿨타임
    private float lastJumpTime;                         // 마지막으로 점프한 시간
    private float currentTime;                          // 현재 시간

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        objectWidth = GetComponent<Collider2D>().bounds.extents.x;
        objectHeight = GetComponent<Collider2D>().bounds.extents.y;
        jumpDirection = new Vector2(0, 1);
    }

    private void Update()
    {
        SetArrowTransform();
        CheckPosition();
        currentTime = Time.time;

        if(isCoroutineRun)
        {return;}
        
        if(!GetComponent<BoxCollider2D>().isActiveAndEnabled && rb.velocity.y < 0)
        {GetComponent<BoxCollider2D>().enabled = true;}

        if(!isJumping && !isDiceRoll)
        {
            if(currentTime - lastJumpTime > jumpCoolDownTime)
            {
                SetJumpDirection();
                isWallJumping = false;
                scoreCheck = true;
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
            GameManager.isGameOver = true;
            Destroy(gameObject);
        }
    }

    // 땅 위에 있으면
    private void OnCollisionStay2D(Collision2D other)
    {
        isJumping = false;

        if(scoreCheck)
        {
            int score = int.Parse(other.transform.name);
            if(score > PlayerPrefs.GetInt("Score"))
            {
                PlayerPrefs.SetInt("Score", score);
                scoreText.text = PlayerPrefs.GetInt("Score").ToString();
            }
        }
    }

    // 땅에서 벗어나면 isJumping = true
    private void OnCollisionExit2D(Collision2D other) 
    {
        isJumping = true;
        scoreCheck = false;
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
            mainCamera.CameraShake();
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
        GetComponent<BoxCollider2D>().enabled = false;
        StartCoroutine(Jump(jumpForce * 1.2f));
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
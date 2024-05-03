using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private UIManager uiManager;
    [SerializeField] private GameObject arrow;      // 점프 방향을 나타내는 이미지 오브젝트

    [HideInInspector]
    public Vector2 jumpDirection;              // 점프 방향
    private float jumpForce = 700;             // 점프 힘

    public bool isJumping = false;             // 점프중인지 체크

    private float jumpCoolDownTime = 3f;         // 점프 쿨타임
    private float lastJumpTime;
    float currentTime;

    private float deathLimitY = -10;           // y좌표 제한
    
    private Rigidbody2D rb;               // rigidbodt2D 컴포넌트
    public TextMeshProUGUI text;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        jumpDirection = new Vector2(0, 1);
    }

    private void Update()
    {
        text.text = rb.velocity.ToString();
        SetArrowTransform();

        if(transform.position.y <= deathLimitY)
            SceneManager.LoadScene(0);

        currentTime = Time.time;

        if(!isJumping)
        {
            if(currentTime - lastJumpTime > jumpCoolDownTime)
            {
                SetJumpDirection();

                if(Input.GetKeyDown(KeyCode.Space))
                {
                    DoJump();
                }
            }
        }
        else
        {
            lastJumpTime = currentTime;
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        isJumping = false;
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
        if(currentTime - lastJumpTime > jumpCoolDownTime)
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
    public void DoJump()
    {
        rb.AddForce(jumpDirection * jumpForce);
        isJumping = true;
        lastJumpTime = Time.time;
    }

    /// <summary>
    /// 일반 점프 (1눈)
    /// </summary>
    private IEnumerator Jump(float jumpForce)
    {
        rb.AddForce(jumpDirection * jumpForce);
        yield return new WaitForSeconds(0.1f);
    }

    /// <summary>
    /// 더블 점프 (2눈)
    /// </summary>
    private IEnumerator DoubleJump()
    {
        jumpForce = 700;
        StartCoroutine(Jump(jumpForce));
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(Jump(jumpForce));
    }

    /// <summary>
    /// 클론 생성 (3눈)
    /// </summary>
    private IEnumerator Clone()
    {
        isJumping = true;
        yield return null;
        uiManager.cloneDicePanelOnOff();

    }

    /// <summary>
    /// 랜덤 점프 (4눈)
    /// </summary>
    private IEnumerator RandomJump()
    {
        yield return null;
        jumpDirection.x = UnityEngine.Random.Range(-0.8f, 0.8f);
        jumpForce = UnityEngine.Random.Range(500, 1000);
        StartCoroutine(Jump(jumpForce));
    }

    /// <summary>
    /// 벽 점프 (5눈)
    /// </summary>
    private IEnumerator WallJump()
    {
        yield return null;
        StartCoroutine(Jump(jumpForce));
        isWallJumping = true;
    }

    /// <summary>
    /// 슈퍼 점프 (6눈)
    /// </summary>
    private IEnumerator SuperJump()
    {
        yield return null;
        StartCoroutine(Jump(jumpForce*2));
    }
}

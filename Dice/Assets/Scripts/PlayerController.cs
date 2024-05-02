using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private UIManager uiManager;
    [SerializeField] private GameObject arrow;      // 점프 방향을 나타내는 이미지 오브젝트

    [SerializeField] private Transform groundCheck;
    [SerializeField] private float radious;
    [SerializeField] private LayerMask groundLayer; // LayerMask

    [HideInInspector]
    public Vector2 jumpDirection;              // 점프 방향
    private float jumpForce = 700;              // 점프 힘

    public bool isJumping = false;             // 점프중인지 체크
    private bool wallJump = false;              // 벽점프 가능 여부 확인

    private float jumpCoolDown = 0.5f;         // 점프 쿨타임
    private float jumpCoolDownCounter;

    private float deathLimitY = -10;            // y좌표 제한

    private bool isFacingRight = true;
    
    private Rigidbody2D rb;                     // rigidbodt2D 컴포넌트
    private Collider2D coll;              // collider2D 컴포넌트

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<Collider2D>();
        jumpDirection = new Vector2(0, 1);
    }

    private void Update()
    {
        SetArrowTransform();
        RaycastHorizontal();

        if(transform.position.y <= deathLimitY)
            SceneManager.LoadScene(0);

        if(!isJumping)
        {
            jumpCoolDownCounter -= Time.deltaTime;
        }
        else
        {
            jumpCoolDownCounter = jumpCoolDown;
        }

        if(jumpCoolDownCounter <= 0f)
        {
            SetJumpDirection();

            if(Input.GetKeyDown(KeyCode.Space))
            {
                DoJump();
            }

            if(Input.GetKeyDown(KeyCode.Alpha1))
            {
                StartCoroutine(Jump(jumpForce));
            }
            if(Input.GetKeyDown(KeyCode.Alpha2))
            {
                StartCoroutine(DoubleJump());;
            }
            if(Input.GetKeyDown(KeyCode.Alpha3))
            {
                StartCoroutine(Clone());
            }
            if(Input.GetKeyDown(KeyCode.Alpha4))
            {
                StartCoroutine(RandomJump());
            }
            if(Input.GetKeyDown(KeyCode.Alpha5))
            {
                StartCoroutine(WallJump());
            }
            if(Input.GetKeyDown(KeyCode.Alpha6))
            {
                StartCoroutine(SuperJump());
            }
        }
    }
    
    private void OnCollisionStay2D(Collision2D other) 
    {
        if(rb.velocity.x <= 0f && rb.velocity.y <= 0f)
        {
            isJumping = false;
        }
    }

    private void OnCollisionExit2D(Collision2D other) 
    {
        isJumping = true;
    }

    /// <summary>
    /// 벽 충돌 감지
    /// </summary>
    private void RaycastHorizontal()
    {
        float distance = 0.9f;
        Vector2 direction = Vector2.up;
        Bounds bounds = coll.bounds;

        Vector2 leftRayPosition;
        Vector2 rightRayPosition;

        RaycastHit2D leftHit;
        RaycastHit2D rightHit;

        leftRayPosition = new Vector2(bounds.min.x, bounds.min.y+0.05f);
        leftHit = Physics2D.Raycast(leftRayPosition, direction, distance, groundLayer);
        Debug.DrawRay(leftRayPosition, direction*distance, Color.blue);

        rightRayPosition = new Vector2(bounds.max.x, bounds.min.y+0.05f);
        rightHit = Physics2D.Raycast(rightRayPosition, direction, distance, groundLayer);
        Debug.DrawRay(rightRayPosition, direction*distance, Color.red);

        if((leftHit || rightHit) && wallJump)
        {
            wallJump = false;
            jumpDirection.x *= -1;
            rb.velocity = new Vector2(0,0);
            StartCoroutine(Jump(jumpForce));
            jumpDirection.x *= -1;
        }
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
        if(jumpCoolDownCounter <= 0f)
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

    private void Flip()
    {
        // if(isFacingRight && rb.velocity)
    }

    // 점프 메서드
    public void DoJump()
    {
        arrow.SetActive(false);
        int jumpType = UnityEngine.Random.Range(1,7);

        switch(jumpType)
        {
            case 1:
                print($"{jumpType} : Jump");
                StartCoroutine(Jump(jumpForce));
                break;

            case 2:
                print($"{jumpType} : Double Jump");
                StartCoroutine(DoubleJump());
                break;

            case 3:
                print($"{jumpType} : Clone");
                StartCoroutine(Clone());
                break;

            case 4:
                print($"{jumpType} : Random Jump");
                StartCoroutine(RandomJump());
                break;

            case 5:
                print($"{jumpType} : Wall Jump");
                StartCoroutine(WallJump());
                break;
            
            case 6:
                print($"{jumpType} : Super Jump");
                StartCoroutine(SuperJump());
                break;
        }
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
        wallJump = true;
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

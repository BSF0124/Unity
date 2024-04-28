using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private GameObject arrow;                   // 점프 방향을 나타내는 이미지 오브젝트

    private Vector2 jumpDirection;              // 점프 방향
    private float jumpForce = 700;              // 점프 힘
    
    private float deathLimitY = -10;            // y좌표 제한

    private bool isGrounded = true;             // 착지 여부 확인    
    private bool isJumping = false;             // 점프 가능 여부 확인
    private float jumpCoolDown = 1;             // 점프 쿨타임

    private bool wallJump = false;              // 벽점프 가능 여부 확인

    private Rigidbody2D rb;                     // rigidbodt2D 컴포넌트
    private Collider2D collider2D;              // collider2D 컴포넌트
    public LayerMask collisionLayer;            // LayerMask

    public TextMeshProUGUI text;

    // 컴포넌트 초기화
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        collider2D = GetComponent<Collider2D>();
        jumpDirection = new Vector2(0, 1);
    }

    private void Update()
    {
        SetArrowTransform();
        text.text = rb.velocity.ToString();

        if(transform.position.y <= deathLimitY)
            SceneManager.LoadScene(0);

        if(!isGrounded)
        {
            RaycastVertical();
        }

        if(wallJump)
        {
            RaycastHorizontal();
        }
        if(!isJumping)
        {
            SetJumpDirection();

            if(Input.GetKeyDown(KeyCode.Space))
            {
                DoJump();
            }
        }
    }

    /// <summary>
    /// 바닥 충돌 감지
    /// </summary>
    private void RaycastVertical()
    {
        Vector2 rayPosition;
        Vector2 direction = Vector2.right;
        float distance = 0.9f;
        RaycastHit2D hit;

        Bounds bounds = collider2D.bounds;
        // bounds.Expand(0.015f * -2);

        rayPosition = new Vector2(bounds.min.x+0.05f, bounds.min.y-0.1f);
        hit = Physics2D.Raycast(rayPosition, direction, distance, collisionLayer);
        Debug.DrawRay(rayPosition, direction*distance, Color.red);

        if(hit)
        {
            isGrounded = true;
            StartCoroutine(JumpCoolDown());
        }
    }

    /// <summary>
    /// 벽 충돌 감지
    /// </summary>
    private void RaycastHorizontal()
    {
        float distance = 0.9f;
        Vector2 direction = Vector2.up;
        Bounds bounds = collider2D.bounds;

        Vector2 leftRayPosition;
        Vector2 rightRayPosition;

        RaycastHit2D leftHit;
        RaycastHit2D rightHit;

        leftRayPosition = new Vector2(bounds.min.x-0.1f, bounds.min.y+0.05f);
        leftHit = Physics2D.Raycast(leftRayPosition, direction, distance, collisionLayer);
        Debug.DrawRay(leftRayPosition, direction*distance, Color.red);

        rightRayPosition = new Vector2(bounds.max.x+0.1f, bounds.min.y+0.05f);
        rightHit = Physics2D.Raycast(rightRayPosition, direction, distance, collisionLayer);
        Debug.DrawRay(rightRayPosition, direction*distance, Color.red);

        if(leftHit || rightHit)
        {
            wallJump = false;
            jumpDirection.x *= -1;
            rb.velocity = new Vector2(0,0);
            StartCoroutine(Jump(jumpForce));
        }
    }

    /// <summary>
    /// 점프 방향 변경
    /// </summary>
    private void SetJumpDirection()
    {
        if(Input.GetKey(KeyCode.LeftArrow) && jumpDirection.x > -1)
        {
            jumpDirection.x += -0.01f;
            jumpDirection.y = (float)Math.Sqrt(1 - (jumpDirection.x * jumpDirection.x));
        }
        if(Input.GetKey(KeyCode.RightArrow) && jumpDirection.x < 1)
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

    // 점프 메서드
    private void DoJump()
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
                print($"{jumpType} : Long Jump");
                StartCoroutine(LongJump());
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
    
    // 점프 쿨타임 측정 코루틴
    IEnumerator JumpCoolDown()
    {
        yield return new WaitForSeconds(jumpCoolDown);
        arrow.SetActive(true);
        isJumping = false;
    }

    /// <summary>
    /// 일반 점프 (1눈)
    /// </summary>
    private IEnumerator Jump(float jumpForce)
    {
        rb.AddForce(jumpDirection * jumpForce);
        yield return new WaitForSeconds(0.1f);
        isJumping = true;
        isGrounded = false;
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
        yield return null;
        isJumping = true;
        isGrounded = false;
    }

    /// <summary>
    /// 롱 점프 (3눈)
    /// </summary>
    private IEnumerator LongJump()
    {
        yield return null;
        StartCoroutine(Jump(jumpForce*2));
        isJumping = true;
        isGrounded = false;
    }

    /// <summary>
    /// 랜덤 점프 (4눈)
    /// </summary>
    private IEnumerator RandomJump()
    {
        jumpDirection.x = UnityEngine.Random.Range(-0.8f, 0.8f);
        jumpForce = UnityEngine.Random.Range(500, 1000);
        StartCoroutine(Jump(jumpForce));
        yield return null;
        isJumping = true;
        isGrounded = false;
    }

    /// <summary>
    /// 벽 점프 (5눈)
    /// </summary>
    private IEnumerator WallJump()
    {
        StartCoroutine(Jump(jumpForce));
        wallJump = true;
        yield return null;
        isJumping = true;
        isGrounded = false;
    }

    /// <summary>
    /// 슈퍼 점프 (6눈)
    /// </summary>
    private IEnumerator SuperJump()
    {
        yield return null;
        StartCoroutine(Jump(jumpForce*3));
        isJumping = true;
        isGrounded = false;
    }
}

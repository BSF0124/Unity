using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private GameObject arrow;                   // 점프 방향을 나타내는 이미지 오브젝트
    [HideInInspector]
    public Vector2 jumpDirection;               // 점프 방향
    public float jumpForce = 700;
    private bool isGrounded = true;             // 점프 조건 체크
    private bool wallJumpOn = false;
    private Rigidbody2D rb;                     // rigidbodt2D 컴포넌트
    private Collider2D collider2D;              // collider2D 컴포넌트
    public LayerMask collisionLayer;
    public TextMeshProUGUI text;

    private float deathLimitY = -10;
    
    // 컴포넌트 초기화
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        collider2D = GetComponent<Collider2D>();
        jumpDirection = new Vector2(0, 1);
    }

    private void Update()
    {
        RaycastVertical();
        SetArrowTransform();
        text.text = rb.velocity.ToString();

        if(transform.position.y <= deathLimitY)
            SceneManager.LoadScene(0);

        if(wallJumpOn)
        {
            RaycastHorizontal();
        }
        if(isGrounded)
        {
            SetJumpDirection();

            if(Input.GetKeyDown(KeyCode.A))
                StartCoroutine(Jump(700));
            if(Input.GetKeyDown(KeyCode.S))
                StartCoroutine(DoubleJump());
            if(Input.GetKeyDown(KeyCode.D))
                StartCoroutine(LongJump());
            if(Input.GetKeyDown(KeyCode.Q))
                StartCoroutine(RandomJump());
            if(Input.GetKeyDown(KeyCode.W))
                StartCoroutine(WallJump());
            if(Input.GetKeyDown(KeyCode.E))
                StartCoroutine(SuperJump());
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
            wallJumpOn = false;
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
            wallJumpOn = false;
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

    /// <summary>
    /// 점프 (1눈)
    /// </summary>
    private IEnumerator Jump(float jumpForce)
    {
        isGrounded = false;
        rb.AddForce(jumpDirection * jumpForce);
        yield return null;
    }

    /// <summary>
    /// 더블 점프 (2눈)
    /// </summary>
    private IEnumerator DoubleJump()
    {
        isGrounded = false;
        jumpForce = 700;
        StartCoroutine(Jump(jumpForce));
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(Jump(jumpForce));
        yield return null;
    }

    /// <summary>
    /// 롱 점프 (3눈)
    /// </summary>
    private IEnumerator LongJump()
    {
        isGrounded = false;
        yield return null;
    }

    /// <summary>
    /// 랜덤 점프 (4눈)
    /// </summary>
    private IEnumerator RandomJump()
    {
        isGrounded = false;
        jumpDirection.x = UnityEngine.Random.Range(-0.8f, 0.8f);
        jumpForce = UnityEngine.Random.Range(500, 1000);
        StartCoroutine(Jump(jumpForce));
        yield return null;
    }

    /// <summary>
    /// 벽 점프 (5눈)
    /// </summary>
    private IEnumerator WallJump()
    {
        isGrounded = false;
        StartCoroutine(Jump(jumpForce));
        wallJumpOn = true;
        yield return null;
    }

    /// <summary>
    /// 슈퍼 점프 (6눈)
    /// </summary>
    private IEnumerator SuperJump()
    {
        isGrounded = false;
        yield return null;
    }
}

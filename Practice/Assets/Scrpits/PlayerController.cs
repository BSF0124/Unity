using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private float horizontal;               // 이동 방향
    private float speed = 8f;               // 이동 속도
    private float jumpingPower = 16f;       // 점프 힘
    private bool isFacingRight = true;      // 바라보는 방향 체크

    private bool isJumping;                 // 점프중인지 체크

    private float coyoteTime = 0.2f;        // 코요테 타임 시간
    private float coyoteTimeCounter;        // 코요테 타임 측정

    private float jumpBufferTime = 0.2f;    // 점프 버퍼링 시간
    private float jumpBufferCounter;        // 점프 버퍼링 측정

    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;

    private void Update()
    {
        horizontal = Input.GetAxisRaw("Horizontal");    // 좌,우 이동

        // 플레이어가 땅 위에 있으면 코요테 타임 카운터를 설정하고, 공중에 있으면 초 읽기 시작
        if(IsGrounded())
        {
            coyoteTimeCounter = coyoteTime;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }

        // 점프 버튼을 누르면 점프 커퍼링 카운터 설정 및 초 읽기 시작
        if(Input.GetButtonDown("Jump"))
        {
            jumpBufferCounter = jumpBufferTime;
        }
        else
        {
            jumpBufferCounter -= Time.deltaTime;
        }

        // 
        if(coyoteTimeCounter > 0f && jumpBufferCounter > 0f && !isJumping)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpingPower); // 점프
            jumpBufferCounter = 0f;                                 // 점프 버퍼링 카운터를 0으로 설정
            StartCoroutine(JumpCooldown());                         // 점프 후 0.4초 동안 점프 불가
        }

        // 점프 버튼을 떼고 점프 속도가 0보다 크면 점프 속도를 절반으로 줄임
        if(Input.GetButtonUp("Jump") && rb.velocity.y > 0f)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);

            // 코요테 타임 카운터를 0으로 설정
            coyoteTimeCounter = 0f;
        }

        // 플레이어가 바라보는 방향 조정
        Flip();
    }

    // 좌, 우로 이동
    private void FixedUpdate() 
    {
        rb.velocity = new Vector2(horizontal * speed, rb.velocity.y);
    }

    // 플레이어가 땅을 밟고있는지 확인하는 메서드
    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }

    // 바라보는 방향으로 오브젝트를 뒤집음
    private void Flip()
    {
        if(isFacingRight && horizontal < 0f || !isFacingRight && horizontal > 0f)
        {
            Vector3 localScale = transform.localScale;
            isFacingRight = !isFacingRight;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }
    }

    // 점프 시전 쿨타임 코루틴
    private IEnumerator JumpCooldown()
    {
        isJumping = true;
        yield return new WaitForSeconds(0.4f);
        isJumping = false;
    }
}

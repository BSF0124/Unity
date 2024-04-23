using UnityEngine;
using TMPro;
using System;

public class Dice : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI jumpDirectionText;  // 점프 방향을 표시하는 텍스트
    [SerializeField]
    private TextMeshProUGUI jumpForceText;      // 점프 힘을 표시하는 텍스트
    [SerializeField]
    private GameObject arrow;                   // 점프 방향을 나타내는 이미지 오브젝트

    private Vector2 jumpDirection;              // 점프 방향
    private float jumpForce = 500f;             // 점프 힘
    private float gravity;

    private Rigidbody2D rb;

    
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        jumpDirection = new Vector2(0, 1);
    }

    private void Update()
    {
        SetText();
        SetJumpDirection();
        SetArrowTransform();
        SetJumpForce();
        GroundCheck();
        Jump();
    }

    // 텍스트 설정
    private void SetText()
    {
        jumpDirectionText.text = $"Direction : {jumpDirection}";
        jumpForceText.text = $"JumpForce : {jumpForce}";
    }

    // 점프 방향 변경
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

        if(jumpDirection.x > 1)
        {
            jumpDirection = new Vector2(1, 0);
        }
        if(jumpDirection.x < -1)
        {
            jumpDirection = new Vector2(-1, 0);
        }
    }

    // 점프 힘 변경
    private void SetJumpForce()
    {
        if(Input.GetKeyDown(KeyCode.UpArrow) && jumpForce < 1000)
        {
            jumpForce += 100;
        }

        if(Input.GetKeyDown(KeyCode.DownArrow) && jumpForce > 300)
        {
            jumpForce -= 100;
        }
    }

    // 점프
    private void Jump()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            rb.AddForce(jumpDirection * jumpForce);
        }
    }

    // 화살표 위치, 각도 설정
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

    private void GroundCheck()
    {
        Debug.DrawRay(transform.position - new Vector3(0,0.5f,0), new Vector2(0,-0.1f), new Color(1,0,0));
    }
}

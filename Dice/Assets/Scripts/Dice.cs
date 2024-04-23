using UnityEngine;
using TMPro;
using System;

public class Dice : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI jumpDirectionText;
    [SerializeField]
    private TextMeshProUGUI jumpForceText;
    [SerializeField]
    private GameObject arrow;

    private Vector2 jumpDirection;
    private float jumpForce = 500f;
    private Rigidbody2D rb;

    private bool isGround = true;
    public LayerMask layer;

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
        CheckGround();
        Jump();
    }
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

    // 점프 
    private void SetJumpForce()
    {
        if(Input.GetKey(KeyCode.UpArrow) && jumpForce < 500)
        {
            jumpForce += 50;
        }

        if(Input.GetKey(KeyCode.DownArrow) && jumpForce > 100)
        {
            jumpForce -= 50;
        }
    }

    // 점프
    private void Jump()
    {
        if(Input.GetKeyDown(KeyCode.Space) && isGround)
        {
            rb.AddForce(jumpDirection * jumpForce);
        }
    }

    // 
    private void CheckGround()
    {
    }

    // 화살표 위치, 각도 설정
    private void SetArrowTransform()
    {
        // 위치
        if(jumpDirection.x >= 0.5f && jumpDirection.x <= -0.5f)
        {
            if(jumpDirection.x > 0)
            {
                arrow.transform.localPosition = new Vector2(jumpDirection.x+0.5f, (1-Mathf.Abs(jumpDirection.x))*2);
            }
            else
            {
                arrow.transform.localPosition = new Vector2(-(jumpDirection.x+0.5f), (1-Mathf.Abs(jumpDirection.x))*2);
            }
        }
        else
        {
            arrow.transform.localPosition = new Vector2(jumpDirection.x*2, 1.5f-Mathf.Abs(jumpDirection.x));
        }

        // 각도
        arrow.transform.rotation = Quaternion.Euler(0,0,jumpDirection.x * -90f);
    }
}

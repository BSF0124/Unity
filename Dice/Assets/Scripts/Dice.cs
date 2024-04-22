using UnityEngine;
using TMPro;
using System.Collections;
using System;

public class Dice : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI jumpDirectionText;
    [SerializeField]
    private TextMeshProUGUI jumpForceText;

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
        SetJumpForce();
        CheckGround();
        Jump();
    }
    private void SetText()
    {
        jumpDirectionText.text = $"Direction : {jumpDirection}";
        jumpForceText.text = $"JumpForce : {jumpForce}";
    }

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

    private void Jump()
    {
        if(Input.GetKeyDown(KeyCode.Space) && isGround)
        {
            rb.AddForce(jumpDirection * jumpForce);
        }
    }

    private void CheckGround()
    {
    }
}

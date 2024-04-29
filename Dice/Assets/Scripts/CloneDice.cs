using System.Collections;
using UnityEngine;

public class CloneDice : MonoBehaviour
{
    public Vector2 jumpDirection;
    private float jumpForce = 700;
    private bool wallJump = false;
    private Rigidbody2D rb;
    private Collider2D coll;
    public LayerMask collisionLayer;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<Collider2D>();
    }

    private void Update()
    {
        RaycastHorizontal();
    }
    public void DoJump(int jumpType)
    {
        switch(jumpType)
        {
            case 0:
                print($"{jumpType} : Jump");
                StartCoroutine(Jump(jumpForce));
                break;

            case 1:
                print($"{jumpType} : Double Jump");
                StartCoroutine(DoubleJump());
                break;

            case 2:
                print($"{jumpType} : Wall Jump");
                StartCoroutine(WallJump());
                break;
            
            case 3:
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
        yield return null;
    }

    /// <summary>
    /// 벽 점프 (5눈)
    /// </summary>
    private IEnumerator WallJump()
    {
        StartCoroutine(Jump(jumpForce));
        wallJump = true;
        yield return null;
    }

    /// <summary>
    /// 슈퍼 점프 (6눈)
    /// </summary>
    private IEnumerator SuperJump()
    {
        yield return null;
        StartCoroutine(Jump(jumpForce*2));
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
        leftHit = Physics2D.Raycast(leftRayPosition, direction, distance, collisionLayer);
        Debug.DrawRay(leftRayPosition, direction*distance, Color.blue);

        rightRayPosition = new Vector2(bounds.max.x, bounds.min.y+0.05f);
        rightHit = Physics2D.Raycast(rightRayPosition, direction, distance, collisionLayer);
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
}

using System;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private GameManager gameManager;

    [HideInInspector] public Vector2 jumpDirection;     // 점프 방향
    
    public GameObject diceObject;
    private Vector2 screenVector;

    public float screenSize;
    public float screenLeft;
    public float screenRight;

    // public GameObject mainObject;
    public GameObject[] objectArray = new GameObject[3];


    void Awake()
    {
        mainCamera = Camera.main;
        screenVector = mainCamera.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height));
        screenLeft = -screenVector.x;
        screenRight = screenVector.x;
        screenSize = screenVector.x * 2;

        jumpDirection = new Vector2(0, 1);
        objectArray[1] = Instantiate(diceObject,transform);
        gameManager.dice = objectArray[1];
    }

    void Update()
    {
        SetObjectPosition();
    }

    // 오브젝트 확인 및 생성
    public void objectCheck()
    {
        if(objectArray[0] == null)
        {
            objectArray[0] = objectArray[1];
            objectArray[0].GetComponent<BoxCollider2D>().enabled = false;

            objectArray[1] = objectArray[2];
            objectArray[1].GetComponent<BoxCollider2D>().enabled = true;
        }

        else if(objectArray[2] == null)
        {

        }
    }

    // 보조 오브젝트 위치 설정
    private void SetObjectPosition()
    {
        Vector2 position = objectArray[1].transform.position;
        if(objectArray[0] != null)
        {
            objectArray[0].transform.position = new Vector2(position.x, position.y);
        }

        if(objectArray[2] != null)
        {
            objectArray[2].transform.position = objectArray[1].transform.position;
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
}

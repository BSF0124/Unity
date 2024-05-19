using System;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private GameManager gameManager;

    public GameObject[] objectArray = new GameObject[3];
    [HideInInspector] public Vector2 jumpDirection;     // 점프 방향
    [HideInInspector] public bool isDiceRoll = false;
    [HideInInspector] public bool jump = false;
    
    private Vector2 screenVector;
    private Vector2 mainObjectPosition;
    public GameObject diceObject;
    public float screenSize;
    public float screenLeft;
    public float screenRight;

    void Awake()
    {
        mainCamera = Camera.main;
        screenVector = mainCamera.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height));
        screenLeft = -screenVector.x;
        screenRight = screenVector.x;
        screenSize = screenVector.x * 2;

        jumpDirection = new Vector2(0, 1);
        mainObjectPosition = diceObject.transform.position;

        objectArray[0] = Instantiate(diceObject, new Vector2(mainObjectPosition.x - screenSize, mainObjectPosition.y), Quaternion.identity, transform);
        objectArray[1] = Instantiate(diceObject, new Vector2(mainObjectPosition.x, mainObjectPosition.y), Quaternion.identity, transform);
        objectArray[2] = Instantiate(diceObject, new Vector2(mainObjectPosition.x + screenSize, mainObjectPosition.y), Quaternion.identity, transform);
        

        objectArray[0].GetComponent<BoxCollider2D>().enabled = false;
        objectArray[2].GetComponent<BoxCollider2D>().enabled = false;
        objectArray[0].GetComponent<Rigidbody2D>().gravityScale = 0;
        objectArray[2].GetComponent<Rigidbody2D>().gravityScale = 0;

        objectArray[0].SetActive(true);
        objectArray[1].SetActive(true);
        objectArray[2].SetActive(true);
    }

    void Update()
    {
        SetObjectPosition();
        SetJumpDirection();
        objectCheck();
        SetObjectPosition();

        if(jump)
        {
            if(Input.GetKeyDown(KeyCode.Space))
            {
                gameManager.rollDicePanelOnOff();
            }
        }
    }

    // 오브젝트 확인 및 생성
    public void objectCheck()
    {
        if(objectArray[0] == null)
        {
            mainObjectPosition = objectArray[1].transform.position;

            objectArray[0] = objectArray[1];
            objectArray[0].GetComponent<BoxCollider2D>().enabled = false;
            objectArray[0].GetComponent<Rigidbody2D>().gravityScale = 0;

            objectArray[1] = objectArray[2];
            objectArray[1].GetComponent<BoxCollider2D>().enabled = true;

            objectArray[2] = Instantiate(diceObject, new Vector2(mainObjectPosition.x - screenSize, mainObjectPosition.y), Quaternion.identity, transform);
            objectArray[2].GetComponent<BoxCollider2D>().enabled = false; 
            objectArray[2].GetComponent<Rigidbody2D>().gravityScale = 0;
            objectArray[2].SetActive(true);
        }
        if(objectArray[2] == null)
        {
            mainObjectPosition = objectArray[1].transform.position;
            
            objectArray[2] = objectArray[1];
            objectArray[2].GetComponent<BoxCollider2D>().enabled = false;
            objectArray[2].GetComponent<Rigidbody2D>().gravityScale = 0;

            objectArray[1] = objectArray[0];
            objectArray[1].GetComponent<BoxCollider2D>().enabled = true;
            objectArray[1].GetComponent<Rigidbody2D>().gravityScale = 1.5f;

            objectArray[0] = Instantiate(diceObject, new Vector2(mainObjectPosition.x + screenSize, mainObjectPosition.y), Quaternion.identity, transform);
            objectArray[0].GetComponent<BoxCollider2D>().enabled = false;
            objectArray[0].GetComponent<Rigidbody2D>().gravityScale = 0;
            objectArray[0].SetActive(true);
        }
    }

    // 보조 오브젝트 위치 설정
    private void SetObjectPosition()
    {
        Vector2 position = objectArray[1].transform.position;
        if(objectArray[0] != null)
        {
            objectArray[0].transform.position = new Vector2(position.x + screenLeft, position.y);
        }
        if(objectArray[2] != null)
        {
            objectArray[2].transform.position = new Vector2(position.x + screenRight, position.y);
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

    // 점프 메서드
    public void DoJump(int jumpType)
    {
        switch(jumpType)
        {
            case 1:
                StartCoroutine(objectArray[1].GetComponent<Dice>().Jump(700));
                break;

            case 2:
                StartCoroutine(objectArray[1].GetComponent<Dice>().DoubleJump());
                break;

            case 3:
                StartCoroutine(objectArray[1].GetComponent<Dice>().Clone());
                break;

            case 4:
                StartCoroutine(objectArray[1].GetComponent<Dice>().RandomJump());
                break;

            case 5:
                StartCoroutine(objectArray[1].GetComponent<Dice>().WallJump());
                break;
            
            case 6:
                StartCoroutine(objectArray[1].GetComponent<Dice>().SuperJump());
                break;
        }
    }
}

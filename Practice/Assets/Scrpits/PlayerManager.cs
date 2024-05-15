using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    public GameObject playerObject;
    Vector2 screenSize;

    [HideInInspector] public bool isVertical = false;
    [HideInInspector] public bool isHorizontal = false;
    [HideInInspector] public bool isDiagonal = false;

    public GameObject mainObject;
    public List<GameObject> objectList = new List<GameObject>();

    // bool diagonalCreated = false;

    void Start()
    {
        mainCamera = Camera.main;
        screenSize = mainCamera.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height));
        playerObject.GetComponent<PlayerMove>().screenLeft = -screenSize.x;
        playerObject.GetComponent<PlayerMove>().screenRight = screenSize.x;
        playerObject.GetComponent<PlayerMove>().screenTop = screenSize.y;
        playerObject.GetComponent<PlayerMove>().screenBottom = -screenSize.y;

        mainObject = Instantiate(playerObject,transform);
        objectList.Add(mainObject);
        mainObject.SetActive(true);
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Return))
        {
            print(objectList.Count);
        }
        
        if(mainObject == null)
        {
            mainObject = objectList[0];
        }

        if(isVertical && isHorizontal && !isDiagonal) // 수정
        {
            CreateDiagonal(); // 추가
        }
    }

    public void CreateVertical(Vector2 position)
    {
        if(!isVertical)
        {
            Vector2 spawnPosition;

            if(position.x < 0)
            {
                spawnPosition = new Vector2(screenSize.x + playerObject.GetComponent<PlayerMove>().width - (-screenSize.x + playerObject.GetComponent<PlayerMove>().width - position.x), position.y);
            }
            else
            {
                spawnPosition = new Vector2(-screenSize.x - playerObject.GetComponent<PlayerMove>().width - (screenSize.x - playerObject.GetComponent<PlayerMove>().width - position.x), position.y);
            }

            GameObject clone = Instantiate(playerObject, spawnPosition, Quaternion.identity, transform);
            clone.SetActive(true);
            objectList.Add(clone);
            isDiagonal = false; // 대각선 생성 플래그 재설정
        }
    }

    public void CreateHorizontal(Vector2 position)
    {
        if(!isHorizontal)
        {
            Vector2 spawnPosition;
            if(position.y > 0)
            {
                spawnPosition = new Vector2(position.x, -screenSize.y - playerObject.GetComponent<PlayerMove>().height - (screenSize.y - playerObject.GetComponent<PlayerMove>().height - position.y));
            }
            else
            {
                spawnPosition = new Vector2(position.x, screenSize.y + playerObject.GetComponent<PlayerMove>().height - (-screenSize.y + playerObject.GetComponent<PlayerMove>().height - position.y));
            }

            GameObject clone = Instantiate(playerObject, spawnPosition, Quaternion.identity, transform);
            clone.SetActive(true);
            objectList.Add(clone);
            isDiagonal = false; // 대각선 생성 플래그 재설정
        }
    }

    public void CreateDiagonal()
    {
        if(!isDiagonal && objectList.Count == 3)
        {
            Vector2 position = mainObject.transform.position;
            Vector2 spawnPosition;

            // 좌상단
            if(position.x < 0 && position.y > 0)
            {
                spawnPosition = new Vector2(screenSize.x + playerObject.GetComponent<PlayerMove>().width - (-screenSize.x + playerObject.GetComponent<PlayerMove>().width - position.x),
                -screenSize.y - playerObject.GetComponent<PlayerMove>().height - (screenSize.y - playerObject.GetComponent<PlayerMove>().height - position.y));
            }

            // 우상단
            else if(position.x > 0 && position.y > 0)
            {
                spawnPosition = new Vector2(-screenSize.x - playerObject.GetComponent<PlayerMove>().width - (screenSize.x - playerObject.GetComponent<PlayerMove>().width - position.x),
                -screenSize.y - playerObject.GetComponent<PlayerMove>().height - (screenSize.y - playerObject.GetComponent<PlayerMove>().height - position.y));
            }

            // 좌하단
            else if(position.x < 0 && position.y < 0)
            {
                spawnPosition = new Vector2(screenSize.x + playerObject.GetComponent<PlayerMove>().width - (-screenSize.x + playerObject.GetComponent<PlayerMove>().width - position.x),
                screenSize.y + playerObject.GetComponent<PlayerMove>().height - (-screenSize.y + playerObject.GetComponent<PlayerMove>().height - position.y));
            }

            // 우하단
            else
            {
                spawnPosition = new Vector2(screenSize.x + playerObject.GetComponent<PlayerMove>().width - (-screenSize.x + playerObject.GetComponent<PlayerMove>().width - position.x),
                screenSize.y + playerObject.GetComponent<PlayerMove>().height - (-screenSize.y + playerObject.GetComponent<PlayerMove>().height - position.y));
            }

            GameObject clone = Instantiate(playerObject, spawnPosition, Quaternion.identity, transform);
            clone.SetActive(true);
            objectList.Add(clone);

            isDiagonal = true; // 대각선 생성 플래그 설정
        }
    }
}

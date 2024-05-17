using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private GameManager gameManager;
    public GameObject diceObject;
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
        diceObject.GetComponent<Dice>().screenLeft = -screenSize.x;
        diceObject.GetComponent<Dice>().screenRight = screenSize.x;

        mainObject = Instantiate(diceObject,transform);
        objectList.Add(mainObject);
        gameManager.dice = mainObject;
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
    }

    public void CreateVertical(Vector2 position)
    {
        if(!isVertical)
        {
            Vector2 spawnPosition;

            if(position.x < 0)
            {
                spawnPosition = new Vector2(screenSize.x + diceObject.GetComponent<Dice>().objectWidth - (-screenSize.x + diceObject.GetComponent<Dice>().objectWidth - position.x), position.y);
            }
            else
            {
                spawnPosition = new Vector2(-screenSize.x - diceObject.GetComponent<Dice>().objectWidth - (screenSize.x - diceObject.GetComponent<Dice>().objectWidth - position.x), position.y);
            }

            GameObject clone = Instantiate(diceObject, spawnPosition, Quaternion.identity, transform);
            clone.SetActive(true);
            objectList.Add(clone);
        }
    }
}

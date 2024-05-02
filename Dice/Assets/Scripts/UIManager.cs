using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private CameraController mainCamera;
    [SerializeField]
    private PlayerController playerController;
    [SerializeField]
    private GameObject cloneDicePrefab;
    [SerializeField]
    private GameObject rollingDicePanel;        // 주사위 굴리기 패널  
    [SerializeField]
    private GameObject cloneDicePanel;          // 클론 주사위 패널   
    [SerializeField]
    private Image[] cloneDiceIamges = new Image[4];   // 클론 주사위 이미지 배열

    private GameObject cloneDiceObj;
    private int currentDice = 0;                // 선택된 클론 주사위

    private void Awake()
    {
    }

    private void Update()
    {
        if(cloneDicePanel.activeSelf)
        {
            SelectCloneDice();
        }
    }

    // 주사위 굴리기 패널 활성화/비활성화
    public void rollingDicePanelOnOff()
    {
        if(rollingDicePanel.activeSelf)
            rollingDicePanel.SetActive(false);
        else
            rollingDicePanel.SetActive(true);
    }

    // 클론 주사위를 고르는 패널 활성화/비활성화
    public void cloneDicePanelOnOff()
    {
        if(cloneDicePanel.activeSelf)
            cloneDicePanel.SetActive(false);
        else
        {
            cloneDicePanel.SetActive(true);
            refreshImage();
        }
    }

    private void refreshImage()
    {
        for(int i = 0; i < 4; i++)
        {
            if(i == currentDice)
                cloneDiceIamges[i].color = Color.green;

            else
                cloneDiceIamges[i].color = Color.white;
        }
    }

    // 클론 주사위 종류 선택
    public void SelectCloneDice()
    {
        if(Input.GetKeyDown(KeyCode.UpArrow))
        {
            if(currentDice >= 2)
            {
                currentDice -= 2;
                refreshImage();
            }
        }

        if(Input.GetKeyDown(KeyCode.DownArrow))
        {
            if(currentDice <= 1)
            {
                currentDice += 2;
                refreshImage();
            }
        }

        if(Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if(currentDice % 2 == 1)
            {
                currentDice -= 1;
                refreshImage();
            }
        }

        if(Input.GetKeyDown(KeyCode.RightArrow))
        {
            if(currentDice % 2 == 0)
            {
                currentDice += 1;
                refreshImage();
            }
        }

        if(Input.GetKeyDown(KeyCode.Return))
        {
            cloneDicePanelOnOff();
            StartCoroutine(CloneDiceJump());
        }
    }

    private IEnumerator CloneDiceJump()
    {
        yield return StartCoroutine(mainCamera.SetCameraSize(true));

        cloneDiceObj = Instantiate(cloneDicePrefab, 
        playerController.transform.position, Quaternion.identity);
        cloneDiceObj.GetComponent<CloneDice>().jumpDirection = playerController.jumpDirection;
        cloneDiceObj.GetComponent<CloneDice>().DoJump(currentDice);

        yield return new WaitForSeconds(3f);
        Destroy(cloneDiceObj);

        yield return StartCoroutine(mainCamera.SetCameraSize(false));
        
        playerController.isJumping = false;
    }
}

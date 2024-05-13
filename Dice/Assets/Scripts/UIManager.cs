using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private CameraController mainCamera;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private RolltheDice rolltheDice;
    [SerializeField] private GameObject cloneDicePrefab;
    [SerializeField] private GameObject rollDiceObject;
    [SerializeField] private GameObject rollDicePanel;
    [SerializeField] private GameObject cloneDicePanel;          // 클론 주사위 패널
    [SerializeField] private Image[] cloneDiceSelectImages = new Image[4];   // 클론 주사위 선택 이미지

    private GameObject cloneDiceObj;
    private int diceJumpType;
    private int cloneDiceType = 0;                // 선택된 클론 주사위

    private void Update()
    {
        if(rollDiceObject.activeSelf)
        {
            if(rolltheDice.isRollEnd)
            {
                diceJumpType = rolltheDice.currentDice;
                rollDicePanelOnOff();
            }
        }

        if(cloneDicePanel.activeSelf)
        {
            SelectCloneDice();
        }
    }

    // 주사위 굴리기 패널 활성화/비활성화
    public void rollDicePanelOnOff()
    {
        if(rollDiceObject.activeSelf)
        {
            playerController.isDiceRoll = false;
            rollDiceObject.SetActive(false);
            rollDicePanel.SetActive(false);
            StartCoroutine(playerController.DoJump(diceJumpType));
        }

        else
        {
            playerController.isDiceRoll = true;
            rollDicePanel.SetActive(true);
            rollDiceObject.SetActive(true);
        }
    }

    // 클론 주사위를 고르는 패널 활성화/비활성화
    public void cloneDicePanelOnOff()
    {
        if(cloneDicePanel.activeSelf)
        {
            // playerController.isDiceRoll = true;
            cloneDicePanel.SetActive(false);
        }
        else
        {
            // playerController.isDiceRoll = true;
            cloneDicePanel.SetActive(true);
            refreshImage();
        }
    }

    private void refreshImage()
    {
        for(int i = 0; i < 4; i++)
        {
            if(i == cloneDiceType)
                cloneDiceSelectImages[i].gameObject.SetActive(true);

            else
                cloneDiceSelectImages[i].gameObject.SetActive(false);
        }
    }

    // 클론 주사위 종류 선택
    public void SelectCloneDice()
    {
        if(Input.GetKeyDown(KeyCode.UpArrow))
        {
            if(cloneDiceType >= 2)
            {
                cloneDiceType -= 2;
                refreshImage();
            }
        }

        if(Input.GetKeyDown(KeyCode.DownArrow))
        {
            if(cloneDiceType <= 1)
            {
                cloneDiceType += 2;
                refreshImage();
            }
        }

        if(Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if(cloneDiceType % 2 == 1)
            {
                cloneDiceType -= 1;
                refreshImage();
            }
        }

        if(Input.GetKeyDown(KeyCode.RightArrow))
        {
            if(cloneDiceType % 2 == 0)
            {
                cloneDiceType += 1;
                refreshImage();
            }
        }

        if(Input.GetKeyDown(KeyCode.Return))
        {
            cloneDicePanelOnOff();
            StartCoroutine(CreateClone());
        }
    }

    private IEnumerator CreateClone()
    {
        cloneDiceObj = Instantiate(cloneDicePrefab, playerController.transform.position, Quaternion.identity);
        cloneDiceObj.GetComponent<CloneDice>().SetSprite(cloneDiceType);
        cloneDiceObj.GetComponent<CloneDice>().jumpDirection = playerController.jumpDirection;
        yield return StartCoroutine(mainCamera.SetDiceObject(cloneDiceObj));
        cloneDiceObj.GetComponent<CloneDice>().DoJump(cloneDiceType);
    }
}

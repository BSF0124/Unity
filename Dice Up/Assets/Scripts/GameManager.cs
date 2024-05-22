using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Dice dice;
    [SerializeField] private RollDice rolltheDice;
    [SerializeField] private GameObject rollDicePanel;

    private int diceJumpType;

    private void Update()
    {
        if(rollDicePanel.activeSelf)
        {
            if(rolltheDice.isRollEnd)
            {
                diceJumpType = rolltheDice.currentDice;
                rollDicePanelOnOff();
            }
        }
    }

    // 주사위 굴리기 패널 활성화/비활성화
    public void rollDicePanelOnOff()
    {
        if(rollDicePanel.activeSelf)
        {
            dice.isDiceRoll = false;
            rollDicePanel.SetActive(false);
            StartCoroutine(dice.DoJump(diceJumpType));
        }

        else
        {
            dice.isDiceRoll = true;
            rollDicePanel.SetActive(true);
        }
    }
}

using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private PlayerManager playerManager;
    [SerializeField] private RollDice rollDice;
    [SerializeField] private GameObject rollDicePanel;
    [HideInInspector] public GameObject dice;

    private int diceJumpType;

    private void Update()
    {
        if(rollDicePanel.activeSelf)
        {
            if(rollDice.isRollEnd)
            {
                diceJumpType = rollDice.currentDice;
                Invoke("rollDicePanelOnOff", 1f);
            }
        }
    }

    // 주사위 굴리기 패널 활성화/비활성화
    public void rollDicePanelOnOff()
    {
        if(rollDicePanel.activeSelf)
        {
            dice.GetComponent<Dice>().isDiceRoll = false;
            rollDicePanel.SetActive(false);
            rollDicePanel.SetActive(false);
            StartCoroutine(dice.GetComponent<Dice>().DoJump(diceJumpType));
        }

        else
        {
            dice.GetComponent<Dice>().isDiceRoll = true;
            rollDicePanel.SetActive(true);
            rollDicePanel.SetActive(true);
        }
    }
}

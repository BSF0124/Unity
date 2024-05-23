using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static bool isGameOver = false;
    
    [SerializeField] private Dice dice;
    [SerializeField] private RollDice rolltheDice;
    [SerializeField] private GameObject rollDicePanel;
    [SerializeField] TextMeshProUGUI scoreText;

    private int diceJumpType;

    private void Awake()
    {
        PlayerPrefs.SetInt("Score", 0);
    }
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

        if(PlayerPrefs.GetInt("Score") > PlayerPrefs.GetInt("HighScore"))
        {
            PlayerPrefs.SetInt("HighScore", PlayerPrefs.GetInt("Score"));
        }

        scoreText.text = PlayerPrefs.GetInt("Score").ToString();
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

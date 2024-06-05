using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static bool isGameOver = false;
    
    [SerializeField] private Dice dice;
    [SerializeField] private RollDice rolltheDice;
    [SerializeField] private GameObject rollDicePanel;
    [SerializeField] private Image exitCircle;

    private int diceJumpType;
    private float exitTime = 1f;
    private float currentTime;

    private void Awake()
    {
        PlayerPrefs.SetInt("Score", 0);
        currentTime = 0;
    }
    
    private void Update()
    {
        if(isGameOver)
        {
            if(Input.GetKeyDown(KeyCode.Escape))
            {
                isGameOver = false;
                StartCoroutine(SceneLoader.Instance.LoadScene("Menu", LoadSceneMode.Additive));
            }
            else if(Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
            {
                isGameOver = false;
                StartCoroutine(SceneLoader.Instance.LoadScene("Game", LoadSceneMode.Additive));
            }
        }
        else
        {
            if(currentTime >= exitTime)
            {
                StartCoroutine(SceneLoader.Instance.LoadScene("Menu", LoadSceneMode.Additive));
            }

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

            if(Input.GetKey(KeyCode.Escape))
            {
                if(currentTime < exitTime)
                {
                    currentTime += Time.deltaTime;
                }
            }
            else
            {
                if(currentTime != 0)
                {
                    currentTime -= Time.deltaTime;
                }
            }

            exitCircle.fillAmount = currentTime/exitTime;
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

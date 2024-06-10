using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using DG.Tweening;

public class GameManager : MonoBehaviour
{
    public static bool isGameOver = false;
    
    [SerializeField] private Dice dice;
    [SerializeField] private RollDice rolltheDice;
    [SerializeField] private GameObject rollDicePanel;
    [SerializeField] private Image exitCircle;
    [SerializeField] private RectTransform gameoverPanel;
    [SerializeField] private TextMeshProUGUI score;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI bestScoreText;
    [SerializeField] private TextMeshProUGUI gameoverText;

    private int diceJumpType;
    private float exitTime = 1f;
    private float currentTime;
    private float duration = 1f;

    private Coroutine fadeCoroutine;
    private float textDuration = 1f;

    private void Awake()
    {
        PlayerPrefs.SetInt("Score", 0);
        currentTime = 0;
    }
    
    private void Update()
    {
        if(isGameOver)
        {
            if(gameoverPanel.gameObject.activeSelf)
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
                StartCoroutine(GameOverEffect());
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
            score.text = PlayerPrefs.GetInt("Score").ToString();
            exitCircle.fillAmount = currentTime/exitTime;
        }

        if(Input.GetKeyDown(KeyCode.K))
        {
            PlayerPrefs.SetInt("HighScore", 0);
        }
        if(Input.GetKeyDown(KeyCode.O))
        {
            PlayerPrefs.SetInt("Score", PlayerPrefs.GetInt("Score")+1);
        }
        if(Input.GetKeyDown(KeyCode.P))
        {
            PlayerPrefs.SetInt("Score", PlayerPrefs.GetInt("Score")-1);
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

    private IEnumerator GameOverEffect()
    {
        gameoverPanel.localScale = Vector3.zero;
        gameoverPanel.gameObject.SetActive(true);
        bestScoreText.text = PlayerPrefs.GetInt("HighScore").ToString();
        int score = 0;
        yield return new WaitForSeconds(duration);
        gameoverPanel.DOScale(1, duration).SetEase(Ease.OutBack);
        yield return new WaitForSeconds(duration);

        while(score < PlayerPrefs.GetInt("Score"))
        {
            score++;
            scoreText.text = score.ToString();
            if(score > PlayerPrefs.GetInt("HighScore"))
            {
                bestScoreText.text = score.ToString();
            }
            yield return new WaitForSeconds(0.1f);
        }
        if(PlayerPrefs.GetInt("Score") > PlayerPrefs.GetInt("HighScore"))
        {
            PlayerPrefs.SetInt("HighScore", PlayerPrefs.GetInt("Score"));
        }

        yield return new WaitForSeconds(duration);
        StartFadeLoop();
    }

    public void StartFadeLoop()
    {
        if (fadeCoroutine == null)
        {
            fadeCoroutine = StartCoroutine(TextFadeLoop());
        }
    }

    public void StopFadeLoop()
    {
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
            fadeCoroutine = null;
        }
    }

    private IEnumerator TextFadeLoop()
    {
        bool fadeIn = true;
        while (true)
        {
            if (fadeIn)
            {
                gameoverText.DOFade(1, textDuration);
            }

            else
            {
                gameoverText.DOFade(0, textDuration);
            }

            fadeIn = !fadeIn;
            yield return new WaitForSeconds(textDuration);
        }

    }
}

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
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI bestScoreText;
    [SerializeField] private TextMeshProUGUI gameoverText;

    private int diceJumpType;
    private float exitTime = 1f;
    private float currentTime;
    private float duration = 1f;

    private Coroutine fadeCoroutine;
    private float textDuration = 0.5f;

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

    private IEnumerator GameOverEffect()
    {
        gameoverPanel.localScale = Vector3.zero;
        gameoverPanel.gameObject.SetActive(true);
        scoreText.text = PlayerPrefs.GetInt("Score").ToString();
        bestScoreText.text = PlayerPrefs.GetInt("HighScore").ToString();

        yield return new WaitForSeconds(duration);

        gameoverPanel.DOScale(1, duration).SetEase(Ease.OutBack);
        
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

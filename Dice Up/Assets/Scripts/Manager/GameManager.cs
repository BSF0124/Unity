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
    [SerializeField] private TextMeshProUGUI newrecordText;

    private Coroutine fadeCoroutine;
    private int diceJumpType;
    private float exitTime = 1f;
    private float currentTime;
    private float duration = 1f;
    private float textDuration = 1f;
    private bool exitGame = false;

    private void Awake()
    {
        PlayerPrefs.SetInt("Score", 0);
        currentTime = 0;
        // newrecordText.gameObject.SetActive(false);
        AudioManager.instance.SetBgmEffect(Camera.main.GetComponent<AudioHighPassFilter>());
        AudioManager.instance.PlayBgm(true);
    }
    
    private void Update()
    {
        if(!isGameOver && !exitGame && currentTime >= exitTime)
        {
            exitGame = true;
            AudioManager.instance.PlaySfx(AudioManager.Sfx.PressButton);
            StartCoroutine(SceneLoader.instance.LoadScene("Menu", LoadSceneMode.Additive));
            AudioManager.instance.PlayBgm(false);
        }

        else
        {
            // 게임 오버 시
            if(isGameOver)
            {
                if(gameoverPanel.gameObject.activeSelf)
                {
                    // 메인 메뉴로
                    if(Input.GetKeyDown(KeyCode.Escape))
                    {
                        isGameOver = false;
                        StartCoroutine(SceneLoader.instance.LoadScene("Menu", LoadSceneMode.Additive));
                        AudioManager.instance.PlayBgm(false);
                    }

                    // 재시작
                    else if(Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
                    {
                        isGameOver = false;
                        StartCoroutine(SceneLoader.instance.LoadScene("Game", LoadSceneMode.Additive));
                    }
                }

                // 게임 오버 패널 등장
                else
                {
                    StartCoroutine(GameOverEffect());
                }
            }

            else
            {
                // 주사위 굴리기
                if(rollDicePanel.activeSelf)
                {
                    if(rolltheDice.isRollEnd)
                    {
                        diceJumpType = rolltheDice.currentDice;
                        rollDicePanelOnOff();
                    }
                }

                // esc 누름
                if(Input.GetKey(KeyCode.Escape))
                {
                    if(currentTime < exitTime)
                    {
                        currentTime += Time.deltaTime;
                    }
                }
                else
                {
                    if(currentTime > 0)
                    {
                        currentTime -= Time.deltaTime;
                    }
                }
                
                score.text = PlayerPrefs.GetInt("Score").ToString();
                exitCircle.fillAmount = currentTime/exitTime;
            }
        }
    }

    // 주사위 굴리기 패널 활성화/비활성화
    public void rollDicePanelOnOff()
    {
        if(rollDicePanel.activeSelf)
        {
            if(diceJumpType == 3)
                AudioManager.instance.PlaySfx(AudioManager.Sfx.RandomJump);

            dice.isDiceRoll = false;
            rollDicePanel.SetActive(false);
            AudioManager.instance.EffectBgm(false);
            StartCoroutine(dice.DoJump(diceJumpType));
        }

        else
        {
            dice.isDiceRoll = true;
            rollDicePanel.SetActive(true);
            AudioManager.instance.EffectBgm(true);
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
            AudioManager.instance.PlaySfx(AudioManager.Sfx.GameResult);
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
            AudioManager.instance.PlaySfx(AudioManager.Sfx.BestScore);
            PlayerPrefs.SetInt("HighScore", PlayerPrefs.GetInt("Score"));
            NewRecordEffect();
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

    private void NewRecordEffect()
    {
        if(!newrecordText.gameObject.activeSelf)
            newrecordText.gameObject.SetActive(true);

        newrecordText.DOScale(1.5f,duration)
        .OnComplete(() => newrecordText.DOScale(1f, duration)
        .OnComplete(() => NewRecordEffect()));
    }
}

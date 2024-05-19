using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using DG.Tweening;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI gameTitleText;
    [SerializeField] private TextMeshProUGUI gameStartText;
    [SerializeField] private TextMeshProUGUI storyBoardText;
    [SerializeField] private TextMeshProUGUI optionText;
    [SerializeField] private TextMeshProUGUI exitText;
    [SerializeField] private Image fadeImage;
    public float duration = 1f;

    Sequence mySequence;

    private void Awake()
    {
        mySequence = DOTween.Sequence();
        gameTitleText.text = "";
        gameStartText.text = "";
        storyBoardText.text = "";
        optionText.text = "";
        exitText.text = "";
    }

    private void Start()
    {
        // StartCoroutine(Fadeffect(1,0));
        // StartCoroutine(TextEffect2(gameTitleText, "METRO:BOT"));
        // mySequence.Append(gameStartText.DOText("START", 1f));
        // mySequence.Join(storyBoardText.DOText("STORY BOARD", 2f));
        // mySequence.Join(optionText.DOText("OPTION", 1f));
        // mySequence.Join(exitText.DOText("EXIT", 1f));
        StartCoroutine(TitleEffect());
    }

    public void GameStart()
    {
        StartCoroutine(Fadeffect(0,1));
        SceneManager.LoadScene("Game");
    }

    public void StoryBoard()
    {

    }

    public void Option()
    {

    }

    public void Exit()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    IEnumerator TitleEffect()
    {
        yield return StartCoroutine(Fadeffect(1,0));
        yield return StartCoroutine(TextEffect2(gameTitleText, "METRO:BOT"));
        mySequence.Append(gameStartText.DOText("START", 0.5f));
        mySequence.Join(storyBoardText.DOText("STORY BOARD", 0.5f));
        mySequence.Join(optionText.DOText("OPTION", 0.5f));
        mySequence.Join(exitText.DOText("EXIT", 0.5f));
    }

    IEnumerator Fadeffect(float start, float end)
    {
        Color temp = fadeImage.color;
        temp.a = start;
        fadeImage.color = temp;

        fadeImage.gameObject.SetActive(true);
        fadeImage.DOFade(end, duration);
        yield return new WaitForSeconds(duration);

        fadeImage.gameObject.SetActive(false);
    }

    IEnumerator TextEffect(TextMeshProUGUI tmp, string str)
    {
        StringBuilder sb = new StringBuilder();
        int length = str.Length;
        int count = 0;
        tmp.text = "";

        while(true)
        {
            if(count == length)
                break;

            if(str[count] < 65 || str[count] > 122)
            {
                sb.Append(str[count]);
                tmp.text = sb.ToString();
            }

            else
            {
                sb.Append('A');
                tmp.text = sb.ToString();

                while(true)
                {
                    if(sb[count] == str[count])
                    {
                        break;
                    }

                    else
                    {
                        sb[count]++;
                        tmp.text = sb.ToString();
                        yield return new WaitForSeconds(0.03f);
                    }
                }
            }

            count++;
        }
        yield return new WaitForSeconds(0.5f);
    }

    IEnumerator TextEffect2(TextMeshProUGUI tmp, string str)
    {
        StringBuilder sb = new StringBuilder();
        int length = str.Length;
        int count;
        tmp.text = "";

        for(int i=0; i<length; i++)
        {
            if(str[i] < 65 || str[i] > 122)
                sb.Append(str[i]);
            
            else
                sb.Append('A');
        }
        tmp.text = sb.ToString();

        while(true)
        {
            count = 0;
            
            if(sb.Equals(str))
                break;

            else
            {
                while(true)
                {
                    if(count == length)
                        break;
                    
                    else
                    {
                        if(sb[count] != str[count])
                        {
                            sb[count]++;
                        }
                        count++;
                    }
                }
            }
            tmp.text = sb.ToString();
            yield return new WaitForSeconds(0.07f);
        }
        yield return new WaitForSeconds(0.5f);
    }
}
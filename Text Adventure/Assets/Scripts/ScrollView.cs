using System.Collections;
using UnityEngine; 
using UnityEngine.UI;
using TMPro;
using KoreanTyper;
using DG.Tweening;


public class ScrollView : MonoBehaviour
{
    [SerializeField] private Image fadeImage;

    public ScrollRect scrollRect;
    public TextMeshProUGUI tmpPrefab;
    public Image imagePrefab;

    private TextMeshProUGUI currentTMP;
    private string[] dialogue = new string[]{
        "가나다라마바사아자차카타파하 이 글은 텍스트 타이핑 테스트를 위한 문구입니다.",
        "aaaaa  bbbbb  ccccc  ddddd  eeeee",
        "텍스트가 타이핑 되는 도중에 엔터 키를 누르면 타이핑이 스킵이 됩니다.",
        "동해물과 백두산이 마르고 닳도록 하느님이 보우하사 우리 나라 만세 무궁화 삼천리 화려 강산 대한사람 대한으로 길이 보전하세",
    };
    private float duration = 0.025f;
    private bool isTyping = false;
    private int count = 0;

    void Awake()
    {
        StartCoroutine(GameStart());
    }

    void Update()
    {
        if(isTyping && Input.GetKeyDown(KeyCode.Return))
        {
            isTyping = false;
            StopAllCoroutines();
            currentTMP.text = dialogue[count];
            scrollRect.normalizedPosition = new Vector2(0,0);
        }

        else
        {
            if(count != dialogue.Length)
                if(Input.GetKeyDown(KeyCode.Return))
                {
                    count++;
                    AddText();
                }
        }
    }

    private IEnumerator GameStart()
    {
        yield return StartCoroutine(Fadeffect(1, 0));
        AddText();
    }

    public void AddText()
    {
        if(!isTyping)
        {
            currentTMP = Instantiate(tmpPrefab,new Vector3(0,0,0),Quaternion.identity, transform);
            StartCoroutine(Typing());
        }
    }

    public void AddImage()
    {
        Image image = Instantiate(imagePrefab, new Vector3(0,0,0),Quaternion.identity, transform);
        image.color = new Color32((byte)Random.Range(0, 255),(byte)Random.Range(0, 255),(byte)Random.Range(0, 255), 255);
    }
    
    IEnumerator Typing()
    {
        isTyping = true;
        currentTMP.text = null;

        if(dialogue[count].Contains("  "))
        {
            dialogue[count] = dialogue[count].Replace("  ", "\n");
        }

        int typingLength = dialogue[count].GetTypingLength();
        
        for(int index = 0; index <= typingLength; index++)
        {
            if(scrollRect.normalizedPosition.y > 0)
                scrollRect.normalizedPosition = new Vector2(0,0);

            currentTMP.text = dialogue[count].Typing(index);
            yield return new WaitForSeconds(duration);
        }
        isTyping = false;
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
}
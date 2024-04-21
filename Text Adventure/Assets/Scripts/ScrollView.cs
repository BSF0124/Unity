using System.Collections;
using UnityEngine; 
using UnityEngine.UI;
using TMPro;
using KoreanTyper;


public class ScrollView : MonoBehaviour
{
    public ScrollRect scrollRect;
    public TextMeshProUGUI tmpPrefab;
    public Image imagePrefab;

    private TextMeshProUGUI currentTMP;
    private string dialogue;
    private float duration = 0.025f;
    private bool isTyping = false;

    void Start()
    {
        dialogue = "가나다라마바사아자차카타파하가나다라마바사아자차카타파하가나다라마바사아자차카타파하가나다라마바사아자차카타파하";
    }

    void Update()
    {
        if(isTyping && Input.GetKeyDown(KeyCode.Return))
        {
            isTyping = false;
            StopAllCoroutines();
            currentTMP.text = dialogue;
            scrollRect.normalizedPosition = new Vector2(0,0);
        }
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

        if(dialogue.Contains("  "))
        {
            dialogue = dialogue.Replace("  ", "\n");
        }

        int typingLength = dialogue.GetTypingLength();
        
        for(int index = 0; index <= typingLength; index++)
        {
            if(scrollRect.normalizedPosition.y > 0)
                scrollRect.normalizedPosition = new Vector2(0,0);

            currentTMP.text = dialogue.Typing(index);
            yield return new WaitForSeconds(duration);
        }
        isTyping = false;
    }
}
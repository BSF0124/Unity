using System.Collections;
using UnityEngine;
using TMPro;
using KoreanTyper;

public class KoreanTyperAsset : MonoBehaviour
{
    public TextMeshProUGUI tmp; // 텍스트를 출력할 텍스트박스
    public string[] inputText; // 출력할 텍스트
    public float space = 1f;
    private string dialogue;
    private int index = -1;
    private bool isTypingEnd = false; // 엔터 입력 방지

    void Start()
    {
        tmp.lineSpacing = space;

        // 입력한 텍스트가 없을 경우
        if(inputText == null)
        {
            dialogue = "입력된 텍스트가 없습니다.";
            StartCoroutine(Typing(dialogue)); 
        }

        else
        {
            dialogue = "엔터 입력시 텍스트가 출력됩니다.  -Press Enter-";
            StartCoroutine(Typing(dialogue));
        }
    }

    void Update()
    {
        // 엔터 입력 시 다음 대사 출력
        // 다음 대사가 없으면 출력 종료
        if(isTypingEnd && Input.GetKeyDown(KeyCode.Return))
        {
            index++;
            if(index == inputText.Length)
            {
                index = 0;
                // EndTyping();
                // return;
            }
            StartCoroutine(Typing(inputText[index]));
        }
    }

    // 한 문장만 출력하는 경우
    IEnumerator Typing(string talk)
    {
        isTypingEnd = false;

        // 텍스트 박스를 비워둠
        tmp.text = null;

        // 띄어쓰기 두번을 줄바꿈으로 바꿈
        if(talk.Contains("  "))
        {
            talk = talk.Replace("  ", "\n");
        }

        int typingLength = talk.GetTypingLength();
        for(int index = 0; index <= typingLength; index++)
        {
            tmp.text = talk.Typing(index);
            yield return new WaitForSeconds(0.025f);
        }

        // 다음 대사 딜레이
        yield return new WaitForSeconds(1f);
        isTypingEnd = true;
    }

    // 출력 종료
    public void EndTyping()
    {
        dialogue = "텍스트 출력을 종료합니다.";
        StartCoroutine(Typing(dialogue));
        // Application.Quit();
        UnityEditor.EditorApplication.isPlaying = false;
    }
}

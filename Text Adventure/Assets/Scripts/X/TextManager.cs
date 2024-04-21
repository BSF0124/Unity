using UnityEngine;
using UnityEngine.UI;
using TMPro;
using KoreanTyper;
using System.Collections;
using UnityEngine.SceneManagement;


class DialogueBlock
{
    public string dialogue;
    public string option1Text;
    public string option2Text;
    public string option3Text;
    public DialogueBlock nextBlock;
    public DialogueBlock option1Block;
    public DialogueBlock option2Block;
    public DialogueBlock option3Block;

    // 최종 다이얼로그
    public DialogueBlock(string dialogue)
    {
        this.dialogue = dialogue;
        nextBlock = null;
        option1Text = null;
        option2Text = null;
        option3Text = null;
        option1Block = null;
        option2Block = null;
        option3Block = null;
    }

    // 선택지 없는 다이얼로그
    public DialogueBlock(string dialogue, DialogueBlock nextBlock)
    {
        this.dialogue = dialogue;
        this.nextBlock = nextBlock;
        option1Text = null;
        option2Text = null;
        option3Text = null;
        option1Block = null;
        option2Block = null;
        option3Block = null;
    }

    // 선택지 1개 다이얼로그
    public DialogueBlock(string dialogue, string option1Text,DialogueBlock option1Block)
    {
        this.dialogue = dialogue;
        this.option1Text = option1Text;
        this.option1Block = option1Block;
        option2Text = null;
        option3Text = null;
        option2Block = null;
        option3Block = null;
        nextBlock = null;
    }

    // 선택지 2개 다이얼로그
    public DialogueBlock(string dialogue, string option1Text,string option2Text, 
    DialogueBlock option1Block, DialogueBlock option2Block)
    {
        this.dialogue = dialogue;
        this.option1Text = option1Text;
        this.option2Text = option2Text;
        this.option1Block = option1Block;
        this.option2Block = option2Block;
        option3Text = null;
        option3Block = null;
        nextBlock = null;

    }

    // 선택지 3개 다이얼로그
    public DialogueBlock(string dialogue, string option1Text,string option2Text, 
    string option3Text,DialogueBlock option1Block, 
    DialogueBlock option2Block,DialogueBlock option3Block)
    {
        this.dialogue = dialogue;
        this.option1Text = option1Text;
        this.option2Text = option2Text;
        this.option3Text = option3Text;
        this.option1Block = option1Block;
        this.option2Block = option2Block;
        this.option3Block = option3Block;
        nextBlock = null;

    }

}

public class TextManager : MonoBehaviour
{
    public TextMeshProUGUI textPrefab; // 텍스트를 출력할 텍스트박스 프리팹
    private TextMeshProUGUI currentTMP; // 현재 텍스트박스
    public Button buttonPrefab; // 버튼 프리팹
    private Button[] currentButton = new Button[3]; // 현재 버튼 배열
    private DialogueBlock currentBlock; // 현재 블록
    private Transform parent; // 생성될 콘텐츠들의 부모
    private bool nextDialogue = false; // 타이핑 스킵 방지

    static DialogueBlock block8 = new DialogueBlock("여덟번째 블록입니다.");
    static DialogueBlock block7 = new DialogueBlock("일곱번째 블록입니다.");
    static DialogueBlock block6 = new DialogueBlock("여섯번째 블록입니다.");
    static DialogueBlock block5 = new DialogueBlock("다섯번째 블록은 세 개의 선택지가 있습니다.", "1", "2", "3",block6,block7,block8);
    static DialogueBlock block4 = new DialogueBlock("네번째 블록입니다. 선택지가 없습니다.");
    static DialogueBlock block3 = new DialogueBlock("세번째 블록은 두 개의 선택지가 있습니다.", "첫번째 선택", "두번째 선택", block4, block5);
    static DialogueBlock block2 = new DialogueBlock("한 개의 선택지가 존재하는 블록입니다.", "선택 버튼입니다.", block3);
    static DialogueBlock block1 = new DialogueBlock("첫번째 블록입니다.", block2);

    void Start()
    {
        parent = transform;
        DisplayBlock(block1);
    }

    void Update()
    {
        if(nextDialogue && Input.GetMouseButtonDown(0))
        {
            currentBlock = currentBlock.nextBlock;
            DisplayBlock(currentBlock);
        }
        
        if(Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(3);
        }
    }

    void DisplayBlock(DialogueBlock block)
    {
        Reset();

        // 다이얼로그 생성
        currentTMP = Instantiate(textPrefab,new Vector3(0,0,0),Quaternion.identity,parent);
        StartCoroutine(Typing(block.dialogue));

        // 버튼 생성
        // 선택지가 없는 경우
        if(block.option1Block == null)
        {
            nextDialogue = true;
        }

        // 선택지가 1개인 경우
        else if(block.option2Block == null)
        {
            currentButton[0] = Instantiate(buttonPrefab,new Vector3(0,0,0),Quaternion.identity,parent);
            print(currentButton[0].name);
            currentButton[0].GetComponentInChildren<TextMeshProUGUI>().text = block.option1Text;
            
            currentButton[0].onClick.AddListener(Button1Clicked);
        }

        // 선택지가 2개인 경우
        else if(block.option3Block == null)
        {
            currentButton[0] = Instantiate(buttonPrefab,new Vector3(0,0,0),Quaternion.identity,parent);
            currentButton[0].GetComponentInChildren<TextMeshProUGUI>().text = block.option1Text;
            currentButton[1] = Instantiate(buttonPrefab,new Vector3(0,0,0),Quaternion.identity,parent);
            currentButton[1].GetComponentInChildren<TextMeshProUGUI>().text = block.option2Text;
            
            currentButton[0].onClick.AddListener(Button1Clicked);
            currentButton[0].onClick.AddListener(Button2Clicked);

        }

        // 선택지가 3개인 경우
        else
        {
            currentButton[0] = Instantiate(buttonPrefab,new Vector3(0,0,0),Quaternion.identity,parent);
            currentButton[0].GetComponentInChildren<TextMeshProUGUI>().text = block.option1Text;
            currentButton[1] = Instantiate(buttonPrefab,new Vector3(0,0,0),Quaternion.identity,parent);
            currentButton[1].GetComponentInChildren<TextMeshProUGUI>().text = block.option2Text;
            currentButton[2] = Instantiate(buttonPrefab,new Vector3(0,0,0),Quaternion.identity,parent);
            currentButton[2].GetComponentInChildren<TextMeshProUGUI>().text = block.option3Text;
            
            currentButton[0].onClick.AddListener(Button1Clicked);
            currentButton[0].onClick.AddListener(Button2Clicked);
            currentButton[0].onClick.AddListener(Button3Clicked);
        }

        currentBlock = block;
    }

    public void Button1Clicked()
    {
        DisplayBlock(currentBlock.option1Block);
    }

    public void Button2Clicked()
    {
        DisplayBlock(currentBlock.option2Block);
    }

    public void Button3Clicked()
    {
        DisplayBlock(currentBlock.option3Block);
    }

    // 타이핑
    IEnumerator Typing(string dialogue)
    {
        nextDialogue = false;

        // 텍스트 박스를 비워둠
        currentTMP.text = null;

        // 띄어쓰기 두번을 줄바꿈으로 바꿈
        if(dialogue.Contains("  "))
        {
            dialogue = dialogue.Replace("  ", "\n");
        }

        int typingLength = dialogue.GetTypingLength();
        for(int index = 0; index <= typingLength; index++)
        {
            currentTMP.text = dialogue.Typing(index);
            yield return new WaitForSeconds(0.025f);
        }

        // 다음 대사 딜레이
        yield return new WaitForSeconds(1f);
    }

    private void Reset()
    {
        currentTMP = null;
        currentButton[0] = null;
        currentButton[1] = null;
        currentButton[2] = null;
    }
}

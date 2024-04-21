using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

class StoryBlock
{
    public string story;
    public string option1Text;
    public string option2Text;
    public StoryBlock option1Block;
    public StoryBlock option2Block;

    public StoryBlock(string story, string option1Text = "", string option2Text = "", 
    StoryBlock option1Block = null, StoryBlock option2Block = null)
    {
        this.story = story;
        this.option1Text = option1Text;
        this.option2Text = option2Text;
        this.option1Block = option1Block;
        this.option2Block = option2Block;
    }

}

public class GameManager : MonoBehaviour
{
    public TextMeshProUGUI dialogue;
    public Button option1;
    public Button option2;

    StoryBlock currentBlock;

    static StoryBlock block8 = new StoryBlock("게임 오버(Happy Ending - 게임 클리어)");
    static StoryBlock block7 = new StoryBlock("게임 오버(Bad Ending 2 - 과로사)");
    static StoryBlock block6 = new StoryBlock("선택지가 한 개 일수도 있음", "한개지롱", "", block8);
    static StoryBlock block5 = new StoryBlock("다섯번째 블록임", "절대 이 버튼을 누르지마", "눌러", block6, block7);
    static StoryBlock block4 = new StoryBlock("게임 오버(Bad Ending 1 - 고독사)");
    static StoryBlock block3 = new StoryBlock("게임오버(R키를 누르면 재시작)");
    static StoryBlock block2 = new StoryBlock("두번째 블록(선택이 없는 블록이면 버튼 비활성화)", "네번째", "다섯번째", block4, block5);
    static StoryBlock block1 = new StoryBlock("첫번째 블록", "버튼", "클릭", block2, block3);
    
    void Start()
    {
        DisplayBlock(block1);
    }

    void Update()
    {
        if(option1.GetComponentInChildren<TextMeshProUGUI>().text == "")
        {option1.interactable = false;}
        else
        {option1.interactable = true;}

        if(option2.GetComponentInChildren<TextMeshProUGUI>().text == "")
        {option2.interactable = false;}
        else
        {option2.interactable = true;}

        if(Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(2);
        }
    }

    void DisplayBlock(StoryBlock block)
    {
        dialogue.text = block.story;
        option1.GetComponentInChildren<TextMeshProUGUI>().text = block.option1Text;
        option2.GetComponentInChildren<TextMeshProUGUI>().text = block.option2Text;

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
}

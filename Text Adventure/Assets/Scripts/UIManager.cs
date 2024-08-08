using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI storyText;
    public GameObject choiceButtonPrefab;
    public Transform choiceButtonContainer;

    public void DisplayNode(StoryNode node)
    {
        storyText.text = node.text;
        foreach(Transform child in choiceButtonContainer)
        {
            Destroy(child.gameObject);
        }

        foreach(var choice in node.choices)
        {
            var button = Instantiate(choiceButtonPrefab, choiceButtonContainer);
            button.GetComponentInChildren<TextMeshProUGUI>().text = choice.text;
            button.GetComponent<Button>().onClick.AddListener(() => OnChoiceSelected(choice.nextId));
        }
    }

    void OnChoiceSelected(string nextId)
    {
        StoryManager storyManager = Object.FindObjectsByType<StoryManager>(FindObjectsSortMode.None).FirstOrDefault();
        if (storyManager != null)
        {
            storyManager.DisplayNode(nextId);
        }
    }
}

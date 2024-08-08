using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class Choice
{
    public string text;
    public string nextId;
}

[System.Serializable]
public class StoryNode
{
    public string id;
    public string text;
    public List<Choice> choices;
}

public class StoryManager : MonoBehaviour
{
    public TextAsset storyJson;

    private Dictionary<string, StoryNode> storyNodes;

    void Start()
    {
        if (storyJson == null)
        {
            Debug.LogError("Story JSON is not assigned in the inspector");
            return;
        }

        LoadStory();
        DisplayNode("start");
    }

    void LoadStory()
    {
        storyNodes = new Dictionary<string, StoryNode>();
        try
        {
            StoryNode[] nodes = JsonHelper.FromJson<StoryNode>(storyJson.text);
            foreach (var node in nodes)
            {
                storyNodes[node.id] = node;
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Error loading story: " + ex.Message);
        }

        if (storyNodes.Count == 0)
        {
            Debug.LogError("No story nodes loaded. Please check your JSON format.");
        }
    }

    public void DisplayNode(string nodeId)
    {
        if (storyNodes.ContainsKey(nodeId))
        {
            var node = storyNodes[nodeId];
            Object.FindAnyObjectByType<UIManager>().DisplayNode(node);
        }
        else
        {
            Debug.LogError("Story node with ID " + nodeId + " not found.");
        }
    }
}

using TMPro;
using UnityEngine;

public class StageUI : MonoBehaviour
{
    [SerializeField]
    private PlayerController playerController;
    [SerializeField]
    private TextMeshProUGUI jumpDirectionText;  // 점프 방향을 표시하는 텍스트

    void Update()
    {
        SetText();
    }

    private void SetText()
    {
        jumpDirectionText.text = $"Direction : {playerController.jumpDirection}";
    }
}

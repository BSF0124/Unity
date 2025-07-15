using TMPro;
using UnityEngine;

public class ScoreText_Blackjack : MonoBehaviour
{
    public BlackjackField player;
    public BlackjackField dealer;
    private TextMeshProUGUI playerCardCountText;
    private TextMeshProUGUI dealerCardCountText;

    private void Awake()
    {
        playerCardCountText = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        dealerCardCountText = transform.GetChild(1).GetComponent<TextMeshProUGUI>();
    }

    public void UpdateText()
    {
        playerCardCountText.text = player.score + " / " + 15;
        dealerCardCountText.text = dealer.score + " / " + 15;
    }
}

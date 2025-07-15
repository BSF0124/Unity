using TMPro;
using UnityEngine;

public class CardCountText_Poker : MonoBehaviour
{
    public Hand_Poker hand;
    public EnemyCard_Poker enemyCard;
    private TextMeshProUGUI playerCardCountText;
    private TextMeshProUGUI enemyCardCountText;

    private void Awake()
    {
        playerCardCountText = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        enemyCardCountText = transform.GetChild(1).GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        playerCardCountText.text = hand.cardObjects.Count + "/" + DualManager.playerDeckList.Count;
        enemyCardCountText.text = enemyCard.enemyDatas.Count + "/" + DualManager.enemyDeckList.Count;
    }
}

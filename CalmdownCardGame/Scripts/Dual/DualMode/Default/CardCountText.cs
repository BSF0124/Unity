using TMPro;
using UnityEngine;

public class CardCountText : MonoBehaviour
{
    public Hand hand;
    public EnemyCard_Default enemyCard_Default;
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
        enemyCardCountText.text = enemyCard_Default.enemyCards.Count + "/" + DualManager.enemyDeckList.Count;
    }
}

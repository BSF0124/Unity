using UnityEngine;

[CreateAssetMenu(fileName = "NewCard", menuName = "Card")]

public class CardData : ScriptableObject
{
    public int cardID;
    public string cardName;
    public CardRarity cardRarity;
    public CardType cardType;
    public int attackPower;
    public Sprite cardSprite;
    public Sprite maskSprite;
    public Sprite thumbnailSprite;
    public Sprite grayThumbnailSprite;
}

public enum CardRarity
{
    N,
    R,
    SR,
    Null
}

public enum CardType
{
    Rock,
    Scissors,
    Paper,
    All
}
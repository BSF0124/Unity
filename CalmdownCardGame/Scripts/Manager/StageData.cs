using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewStage", menuName = "Stage")]

public class StageData : ScriptableObject
{
    public int stageID;
    public int deckCount;
    public List<int> deckList;
    public DualMode[] dualMode;
}

public enum DualMode
{
    Default, Three_Card_Monte, Matching_Game, Poker, Blackjack,
    Minus_One = 10, Equality, Reverse, No_Duplicate, Random, Replication, Change
}

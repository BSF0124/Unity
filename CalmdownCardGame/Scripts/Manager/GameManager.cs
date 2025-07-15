using System.Collections.Generic;
using UnityEngine;

public enum NNG_Difficulty{Easy, Normal, Hard};

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public NNG_Difficulty current_Difficulty;
    public int current_Stage = 0;
    public bool isStageSelected = false;
    public bool isStageClear = false;
    public bool isSequnceActivate = false;
    public bool isGameStart = true;
    public bool isSkipedCutScene = false;

    public List<int> deckList;

    public Texture2D cursorTex; // 사용자 정의 커서 이미지

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        else
        {
            Destroy(gameObject);
        }

        Cursor.SetCursor(cursorTex, Vector2.zero, CursorMode.ForceSoftware);
    }
}
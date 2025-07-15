using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class DualManager : MonoBehaviour
{
    public static Canvas canvas;
    public static StageData stage;  // 현재 스테이지 데이터
    public static bool isSequenceRunning = false;
    public static bool isDragging = false;
    public static bool isGameOver = false;     // 게임 오버 체크 변수
    public static List<int> playerDeckList = new List<int>();    // 플레이어의 덱 리스트
    public static List<int> enemyDeckList = new List<int>();     // 상대의 덱 리스트

    // 게임 시작, 게임 오버에 사용할 애니메이션 오브젝트 배열
    // 0: 게임 시작 / 1: 게임 오버(승리) / 2: 게임 오버(패배)
    public GameObject[] animationPanels;

    public GameObject tutorial;
    public GameObject gimmick_Tutorial;
    public GameObject gimmick;
    public GameObject gimmickButton;

    private void Awake()
    {
        canvas = transform.parent.GetComponent<Canvas>();
        isGameOver = false;

        // GameManager의 current_Stage 변수를 통해 현재 스테이지 데이터를 불러옴
        if(StageDataManager.instance != null && GameManager.instance != null)
        {
            stage = StageDataManager.instance.GetStageByID(GameManager.instance.current_Stage);
            
            // 만약 현재 스테이지가 덱 구성이 필요한 스테이지일 경우, 플레이어와 상대의 덱 리스트를 저장함.
            // 덱이 필요 없는 스테이지일 경우, deckCount의 값이 0임.
            
            if(stage.deckCount > 0)
            {
                if(GameManager.instance.current_Stage == 1)
                {
                    gimmickButton.SetActive(false);
                    playerDeckList = new List<int>() {2,2,10,10,18,18};
                }
                else if(GameManager.instance.current_Stage == 2)
                {
                    playerDeckList = new List<int>() {1,7,12,19,27};
                }
                else
                {
                    playerDeckList = GameManager.instance.deckList;
                }
                enemyDeckList = stage.deckList;
            }

            // BGM 재생
            if(stage.stageID == 19)
            {
                AudioManager.instance.PlayBgm(AudioManager.instance.bgmClips_Game[14]);
            }
            else if(stage.stageID == 20)
            {
                AudioManager.instance.PlayBgm(AudioManager.instance.bgmClips_Game[15]);
            }
            else
            {
                AudioManager.instance.PlayBgm(AudioManager.instance.bgmClips_Game[Random.Range(1,14)]);
            }
            AudioManager.instance.bgmPlayer.loop = true;
            AudioManager.instance.bgmPlayer.DOFade(AudioManager.instance.bgmVolume, 1f);
        }

        // 게임 시작 애니메이션 재생
        animationPanels[0].SetActive(true);
    }
    
    public void ActivateDualModeObject()
    {
        if(stage.stageID == 1)
        {
            tutorial.SetActive(true);
        }
        else if(stage.stageID == 2)
        {
            gimmick_Tutorial.SetActive(true);
        }

        if(!transform.GetChild((int)stage.dualMode[0]).gameObject.activeSelf)
            transform.GetChild((int)stage.dualMode[0]).gameObject.SetActive(true);
    }

    // 게임 오버 애니메이션 재생
    public void GameOver(bool isPlayerWin)
    {
        StopAllCoroutines();
        isGameOver = true;
        
        if(isPlayerWin)
        {
            if(PlayerDataManager.instance != null)
            {
                if(stage.stageID == 2 && !PlayerDataManager.instance.playerData.stage[2].stageClear)
                {
                    PlayerDataManager.instance.playerData.cardPack += 5;
                }
                else if(stage.stageID == 7 && !PlayerDataManager.instance.playerData.stage[7].stageClear)
                {
                    PlayerDataManager.instance.playerData.cardPack += 5;

                }
                else if(stage.stageID == 12 && !PlayerDataManager.instance.playerData.stage[12].stageClear)
                {
                    PlayerDataManager.instance.playerData.cardPack += 5;

                }
                else if(stage.stageID == 17 && !PlayerDataManager.instance.playerData.stage[17].stageClear)
                {
                    PlayerDataManager.instance.playerData.cardPack += 5;
                }
                else
                {
                    foreach(int card in playerDeckList)
                    {
                        PlayerDataManager.instance.AddCard(card, 1);
                    }
                    foreach(int card in enemyDeckList)
                    {
                        PlayerDataManager.instance.AddCard(card, 1);
                    }
                }
                GameManager.instance.isStageClear = true;
                PlayerDataManager.instance.SaveData();

            }
            animationPanels[1].SetActive(true);
        }
        
        else
            animationPanels[2].SetActive(true);
    }

    public void ShowGimmickPanel()
    {
        PauseManager.instance.canPause = false;
        AudioManager.instance.PlaySfx(AudioManager.instance.sfxClips_Main[2]);
        gimmick.SetActive(true);
    }
}

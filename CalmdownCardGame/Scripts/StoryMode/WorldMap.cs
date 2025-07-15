using UnityEngine;
using TMPro;
using DG.Tweening;

public class WorldMap : MonoBehaviour
{
    public StoryMode storyMode;
    public TextMeshProUGUI worldTMP;
    public GameObject[] worldObjects;
    public GameObject[] stageObjects;
    public GameObject nextWorldButton;
    public GameObject previousWorldButton;
    public GameObject deckBuilder;

    private int worldIndex = 0;
    private int maxWorldIndex = 4;
    private string[] worldText = {
        "월드 1: 금병영 스투디오",
        "월드 2: 배도라운지",
        "월드 3: 천하 제일 카드대회",
        "월드 4: 지하철 1호선",
        "월드 5: OO 학원"
    };

    private void Awake()
    {
        AudioManager.instance.bgmPlayer.loop = true;
        AudioManager.instance.PlayBgm(AudioManager.instance.bgmClips_Game[0]);
        AudioManager.instance.bgmPlayer.DOFade(AudioManager.instance.bgmVolume, 0.7f);
    }

    private void Start()
    {
        worldIndex = 4;
        foreach(var item in PlayerDataManager.instance.playerData.stage)
        {
            if(!item.stageClear)
            {
                if(item.stageID <= 3)
                {
                    worldIndex = 0;
                }
                else if(item.stageID <= 7)
                {
                    worldIndex = 1;
                }
                else if(item.stageID <= 12)
                {
                    worldIndex = 2;
                }
                else if(item.stageID <= 17)
                {
                    worldIndex = 3;
                }
                else
                {
                    worldIndex = 4;
                }
                break;
            }
        }
        SetWorld();
    }

    public void NextWorld()
    {
        worldIndex++;
        SetWorld();
        AudioManager.instance.PlaySfx(AudioManager.instance.sfxClips_Main[2]);
    }

    public void PreviousWorld()
    {
        worldIndex--;
        SetWorld();
        AudioManager.instance.PlaySfx(AudioManager.instance.sfxClips_Main[1]);
    }

    private void SetWorld()
    {
        for(int i=0; i<=maxWorldIndex; i++)
        {
            if(i == worldIndex)
            {
                worldObjects[i].SetActive(true);
                string str = worldText[i];
                switch(worldIndex)
                {
                    case 1:
                        if(!PlayerDataManager.instance.playerData.stage[4].stageClear)
                            str = "월드 2: ???";
                        break;
                    case 2:
                        if(!PlayerDataManager.instance.playerData.stage[8].stageClear)
                            str = "월드 3: ???";
                        break;

                    case 3:
                        if(!PlayerDataManager.instance.playerData.stage[13].stageClear)
                            str = "월드 4: ???";
                        break;

                    case 4:
                        if(!PlayerDataManager.instance.playerData.stage[18].stageClear)
                            str = "월드 5: ???";
                        break;
                }
                worldTMP.text = str;
            }
            else
                worldObjects[i].SetActive(false);
        }

        if(worldIndex == maxWorldIndex)
        {
            nextWorldButton.SetActive(false);
            previousWorldButton.SetActive(true);
        }
        else if(worldIndex == 0)
        {
            nextWorldButton.SetActive(true);
            previousWorldButton.SetActive(false);
        }
        else
        {
            nextWorldButton.SetActive(true);
            previousWorldButton.SetActive(true);
        }
    }

    public void SelectStage(int stageIndex)
    {
        if(GameManager.instance != null)
        {
            GameManager.instance.isSequnceActivate = true;
            GameManager.instance.current_Stage = stageIndex;
            GameManager.instance.isStageSelected = true;
            GameManager.instance.isStageClear = false;

            if(stageIndex == 1 || stageIndex == 2)
            {
                StartCoroutine(SceneLoader.instance.LoadScene(1, 3));
            }
            else if(stageIndex == 7 || stageIndex == 12 || stageIndex == 17)
            {
                StartCoroutine(SceneLoader.instance.LoadScene(1, 2));
            }

            else
            {
                deckBuilder.SetActive(true);
                storyMode.Dictionary();
            }
        }
        AudioManager.instance.PlaySfx(AudioManager.instance.sfxClips_Main[5]);
    }
}

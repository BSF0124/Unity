using UnityEngine;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using TMPro;

public class Gimmick : MonoBehaviour
{
    public GameObject[] buttons;

    public List<GameObject> pageObjects = new List<GameObject>();
    private int currentPage = 0;
    private StringBuilder gimmickList = new StringBuilder("[적용 기믹]\n");

    private void Awake()
    {
        SetPageObjects();
        PageUpdate();
    }

    private void Update()
    {
        if(gameObject.activeSelf && Input.GetKeyDown(KeyCode.Escape))
        {
            PauseManager.instance.canPause = true;
            gameObject.SetActive(false);
        }
    }

    private void SetPageObjects()
    {
        switch(DualManager.stage.dualMode[0])
        {
            case DualMode.Default:
                pageObjects.Add(transform.GetChild(0).gameObject);
                if(DualManager.stage.dualMode.Length > 1)
                {
                    SetGimmickList();
                }
                else
                {
                    gimmickList.AppendLine("•기믹 없음");
                }
                break;

            case DualMode.Three_Card_Monte:
                pageObjects.Add(transform.GetChild(1).gameObject);
                break;

            case DualMode.Matching_Game:
                pageObjects.Add(transform.GetChild(2).gameObject);
                break;

            case DualMode.Poker:
                if(DualManager.stage.dualMode.Length > 1)
                {
                    pageObjects.Add(transform.GetChild(0).gameObject);
                    SetGimmickList();
                }
                pageObjects.Add(transform.GetChild(3).gameObject);
                pageObjects.Add(transform.GetChild(4).gameObject);
                break;

            case DualMode.Blackjack:
                pageObjects.Add(transform.GetChild(5).gameObject);
                pageObjects.Add(transform.GetChild(6).gameObject);
                break;
        }
        transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = gimmickList.ToString();
    }

    private void SetGimmickList()
    {
        if(DualManager.stage.dualMode.Contains(DualMode.Poker))
        {
            gimmickList.AppendLine("• 포커\n");
        }
        if(DualManager.stage.dualMode.Contains(DualMode.Minus_One))
        {
            gimmickList.AppendLine("• 하나 빼기\n   카드 2장을 선택한 뒤, 제한 시간 내에 대결할 카드를 선택한다.\n");
        }
        if(DualManager.stage.dualMode.Contains(DualMode.Equality))
        {
            gimmickList.AppendLine("• 평등\n   카드 등급에 상관없이 목숨이 1로 변경된다.\n");
        }
        if(DualManager.stage.dualMode.Contains(DualMode.Reverse))
        {
            gimmickList.AppendLine("• 역 가위바위보\n   가위바위보의 규칙이 반대로 적용된다.\n");
        }
        if(DualManager.stage.dualMode.Contains(DualMode.No_Duplicate))
        {
            gimmickList.AppendLine("• 중복 금지\n   직전에 플레이한 카드를 연속으로 낼 수 없다.\n");
        }
        if(DualManager.stage.dualMode.Contains(DualMode.Random))
        {
            gimmickList.AppendLine("• 랜덤\n   병거니우스의 덱이 랜덤으로 구성된다.\n");
        }
        if(DualManager.stage.dualMode.Contains(DualMode.Replication))
        {
            gimmickList.AppendLine("• 복제\n   플레이어의 덱과 똑같은 덱을 사용한다.\n");
        }
        if(DualManager.stage.dualMode.Contains(DualMode.Change))
        {
            gimmickList.AppendLine("• 덱 교체\n   플레이어와 덱을 교체한다.\n");
        }
    }

    private void PageUpdate()
    {
        for(int i = 0; i < pageObjects.Count; i++)
        {
            pageObjects[i].gameObject.SetActive(currentPage == i);
        }

        buttons[0].SetActive(currentPage != 0);
        buttons[1].SetActive(currentPage != pageObjects.Count - 1);
    }

    public void LeftButton()
    {
        if(currentPage > 0)
        {
            currentPage--;
        }
        PageUpdate();
        AudioManager.instance.PlaySfx(AudioManager.instance.sfxClips_Main[1]);
    }

    public void RightButton()
    {
        if(currentPage < pageObjects.Count - 1)
        {
            currentPage++;
        }
        PageUpdate();
        AudioManager.instance.PlaySfx(AudioManager.instance.sfxClips_Main[2]);
    }

    public void CloseButton()
    {
        gameObject.SetActive(false);
        PauseManager.instance.canPause = true;
        AudioManager.instance.PlaySfx(AudioManager.instance.sfxClips_Main[1]);
    }
}

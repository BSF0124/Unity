using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Prologue : CutScene
{
    private void Awake()
    {
        dialogues = new string[]{
            "20XX년 X월 X일 금병영 스투디오",
            "하아암...",
            "요즘 할 게임 없어요? 재밌는 게임 추천받습니다.",
            "루카노르 백작 다시 해줘잉~",
            "루카노르는 각자 하시고, 다른 거 추천 있습니까?",
            "침착한 카드 게임 어떰?",
            "침착한 카드 게임?",
            "침착맨은 인터넷에 검색해 본다.",
            "뭐야? 이거 완전 파쿠리 게임이잖아?",
            "ㄴㄴ 아님 이기면 상대방 카드 다 뺏는 어둠의 듀얼임",
            "이기면 상대방 카드을 뺏는다고? 미친 거 아니야?",
            "황급히 '침착한 카드 게임'을 설치하는 침착맨",
            "자! 그러면 <wave a=0.3 f=1.5 w=1>'침착한 카드 게임'</wave> 가보도록 하겠습니다.\n뭔지 자세히는 모르겠는데, 추천이 많이 들어와서 해보려고 합니다."
        };
        if(PlayerDataManager.instance != null)
        {
            PlayerDataManager.instance.playerData.stage[0].stageClear = true;
            PlayerDataManager.instance.SaveData();
            GameManager.instance.isStageClear = true;
        }

        AudioManager.instance.PlayBgm(AudioManager.instance.bgmClips_CutScene[0]);
        AudioManager.instance.bgmPlayer.loop = true;
        AudioManager.instance.bgmPlayer.DOFade(AudioManager.instance.bgmVolume, 0.7f);
    }

    public override void DialogueEffect()
    {
        switch(currentDialogue)
        {
            case 0:
                SetSpeaker(-1);
                dialoguePanel.GetComponent<CanvasGroup>().DOFade(1f, 0.3f)
                    .OnComplete(()=> dialogueTMP.text = dialogues[currentDialogue]);
                break;
            case 1:
                dialogueTMP.text = dialogues[currentDialogue];
                SetSpeaker(0);
                characterObjects[0].GetComponent<Image>().sprite = characterSprites[0];
                characterObjects[0].GetComponent<Image>().SetNativeSize();
                characterObjects[0].transform.localScale = Vector3.one * 2f;
                characterObjects[0].SetActive(true);
                Effects(0, 0);
                break;
            case 2:
                dialogueTMP.text = dialogues[currentDialogue];
                SetSpeaker(0);
                characterObjects[0].GetComponent<Image>().sprite = characterSprites[1];
                characterObjects[0].GetComponent<Image>().SetNativeSize();
                characterObjects[0].transform.localScale = Vector3.one * 0.8f;
                Effects(0, 0);
                AudioManager.instance.PlaySfx(AudioManager.instance.sfxClips_CutScene[1]);
                break;
            case 3:
                skipped = true;
                Sequence sequence_1 = DOTween.Sequence();
                sequence_1.Append(characterObjects[1].GetComponent<RectTransform>().DOAnchorPosY(characterObjects[1].GetComponent<RectTransform>().anchoredPosition.y - 20f, 0.5f))
                        .Join(characterObjects[1].GetComponent<CanvasGroup>().DOFade(1f, 0.5f));
                AudioManager.instance.PlaySfx(AudioManager.instance.sfxClips_CutScene[0]);
                break;
            case 4:
                dialogueTMP.text = dialogues[currentDialogue];
                SetSpeaker(0);
                Effects(0, 0);
                characterObjects[1].GetComponent<CanvasGroup>().DOFade(0f, 0.5f);
                break;
            case 5:
                skipped = true;
                Sequence sequence_2 = DOTween.Sequence();
                sequence_2.Append(characterObjects[2].GetComponent<RectTransform>().DOAnchorPosY(characterObjects[2].GetComponent<RectTransform>().anchoredPosition.y - 20f, 0.5f))
                        .Join(characterObjects[2].GetComponent<CanvasGroup>().DOFade(1f, 0.5f));
                AudioManager.instance.PlaySfx(AudioManager.instance.sfxClips_CutScene[0]);
                break;
            case 6:
                dialogueTMP.text = dialogues[currentDialogue];
                SetSpeaker(0);
                characterObjects[0].GetComponent<Image>().sprite = characterSprites[2];
                characterObjects[0].GetComponent<Image>().SetNativeSize();
                characterObjects[0].transform.localScale = Vector3.one * 2f;
                Effects(0, 0);
                characterObjects[2].GetComponent<CanvasGroup>().DOFade(0f, 0.5f);
                break;
            case 7:
                dialogueTMP.text = dialogues[currentDialogue];
                SetSpeaker(-1);
                AudioManager.instance.PlaySfx(AudioManager.instance.sfxClips_CutScene[42]);
                break;
            case 8:
                dialogueTMP.text = dialogues[currentDialogue];
                SetSpeaker(0);
                characterObjects[0].GetComponent<Image>().sprite = characterSprites[3];
                characterObjects[0].GetComponent<Image>().SetNativeSize();
                characterObjects[0].transform.localScale = Vector3.one * 2.4f;
                Effects(0, 1);
                AudioManager.instance.PlaySfx(AudioManager.instance.sfxClips_CutScene[2]);
                break;
            case 9:
                skipped = true;
                Sequence sequence_3 = DOTween.Sequence();
                sequence_3.Append(characterObjects[3].GetComponent<RectTransform>().DOAnchorPosY(characterObjects[3].GetComponent<RectTransform>().anchoredPosition.y - 20f, 0.5f))
                        .Join(characterObjects[3].GetComponent<CanvasGroup>().DOFade(1f, 0.5f));
                AudioManager.instance.PlaySfx(AudioManager.instance.sfxClips_CutScene[0]);
                break;

            case 10:
                dialogueTMP.text = dialogues[currentDialogue];
                SetSpeaker(0);
                characterObjects[0].GetComponent<Image>().sprite = characterSprites[4];
                characterObjects[0].GetComponent<Image>().SetNativeSize();
                characterObjects[0].transform.localScale = Vector3.one * 1.95f;
                Effects(0, 0);
                characterObjects[3].GetComponent<CanvasGroup>().DOFade(0f, 0.5f);
                AudioManager.instance.PlaySfx(AudioManager.instance.sfxClips_DualLose[6]);
                break;

            case 11:
                dialogueTMP.text = dialogues[currentDialogue];
                SetSpeaker(-1);
                break;

            case 12:
                dialogueTMP.fontSize = 48f;
                dialogueTMP.text = dialogues[currentDialogue];
                SetSpeaker(0);
                characterObjects[0].GetComponent<Image>().sprite = characterSprites[5];
                characterObjects[0].GetComponent<Image>().SetNativeSize();
                characterObjects[0].transform.localScale = Vector3.one * 2.5f;
                Effects(0, 0);
                AudioManager.instance.PlaySfx(AudioManager.instance.sfxClips_CutScene[3]);
                break;
        }
    }
}

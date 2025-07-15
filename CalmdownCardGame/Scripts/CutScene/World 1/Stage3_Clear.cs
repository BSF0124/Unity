using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Stage3_Clear : CutScene
{
    private void Awake()
    {
        dialogues = new string[]{
            "뭐야, 개쉬운데?",
            "미리 예습해 온 거 아님?",
            "평소 게임에 재능이 없기로 유명했던 그였기에,\n시청자들은 그의 실력에 큰 놀라움을 금치 못했다.",
            "여러분이 간과한 사실이 있는데, 저 \"전문 가위바위보인\"이에요.\n저 같은 전문 가위바위보인 입장에서는 너무 밋밋해서 안 하는 장르거든요?",
            "그래서 저는 심심할 때면\n \"가위불바위총번개악마용물공기보스펀지늑대나무사람뱀\"을 합니다.",
            "그게 뭔데 씹덕아"
        };

        AudioManager.instance.PlayBgm(AudioManager.instance.bgmClips_CutScene[2]);
        AudioManager.instance.bgmPlayer.loop = true;
        AudioManager.instance.bgmPlayer.DOFade(AudioManager.instance.bgmVolume, 0.7f);
    }

    public override void DialogueEffect()
    {
        switch(currentDialogue)
        {
            case 0:
                characterObjects[0].GetComponent<Image>().sprite = characterSprites[0];
                characterObjects[0].GetComponent<Image>().SetNativeSize();
                characterObjects[0].transform.localScale = Vector3.one * 2f;
                characterObjects[0].SetActive(true);

                SetSpeaker(0);
                dialoguePanel.GetComponent<CanvasGroup>().DOFade(1f, 0.3f)
                    .OnComplete(()=> dialogueTMP.text = dialogues[currentDialogue]);
                Effects(0, 0);
                AudioManager.instance.PlaySfx(AudioManager.instance.sfxClips_CutScene[5]);
                break;
            case 1:
                skipped = true;
                Sequence sequence_1 = DOTween.Sequence();
                sequence_1.Append(characterObjects[1].GetComponent<RectTransform>().DOAnchorPosY(characterObjects[1].GetComponent<RectTransform>().anchoredPosition.y - 20f, 0.5f))
                        .Join(characterObjects[1].GetComponent<CanvasGroup>().DOFade(1f, 0.5f));
                AudioManager.instance.PlaySfx(AudioManager.instance.sfxClips_CutScene[0]);
                break;
            case 2:
                SetSpeaker(-1);
                dialogueTMP.text = dialogues[currentDialogue];
                break;

            case 3:
                dialogueTMP.fontSize = 50f;
                dialogueTMP.text = dialogues[currentDialogue];
                characterObjects[0].GetComponent<Image>().sprite = characterSprites[1];
                characterObjects[0].GetComponent<Image>().SetNativeSize();
                SetSpeaker(0);
                Effects(0, 0);
                characterObjects[1].GetComponent<CanvasGroup>().DOFade(0f, 0.5f);
                AudioManager.instance.PlaySfx(AudioManager.instance.sfxClips_CutScene[6]);
                break;

            case 4:
                dialogueTMP.text = dialogues[currentDialogue];
                characterObjects[0].GetComponent<Image>().sprite = characterSprites[2];
                characterObjects[0].GetComponent<Image>().SetNativeSize();
                SetSpeaker(0);
                Effects(0, 0);
                characterObjects[2].GetComponent<Image>().DOFade(1f, 0.3f);
                AudioManager.instance.PlaySfx(AudioManager.instance.sfxClips_CutScene[7]);
                break;

            case 5:
                skipped = true;
                Sequence sequence_2 = DOTween.Sequence();
                sequence_2.Append(characterObjects[3].GetComponent<RectTransform>().DOAnchorPosY(characterObjects[3].GetComponent<RectTransform>().anchoredPosition.y - 20f, 0.5f))
                        .Join(characterObjects[3].GetComponent<CanvasGroup>().DOFade(1f, 0.5f));
                AudioManager.instance.PlaySfx(AudioManager.instance.sfxClips_CutScene[0]);
                break;
        }
    }
}

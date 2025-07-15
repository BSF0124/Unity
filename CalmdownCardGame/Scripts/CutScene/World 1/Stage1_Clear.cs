using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Stage1_Clear : CutScene
{
    private void Awake()
    {
        dialogues = new string[]{
            "이거 이렇게 하는 거 맞아요?",
            "대 병 건",
            "왜 잘함?",
            "병건아 가라~ 재미없다~",
            "아 <incr a=1.5 f=2 w=2>뱀 ~ ~ ~</incr>"
        };

        AudioManager.instance.PlayBgm(AudioManager.instance.bgmClips_CutScene[1]);
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
                characterObjects[0].transform.localScale = Vector3.one * 1.2f;
                characterObjects[0].SetActive(true);

                SetSpeaker(0);
                dialoguePanel.GetComponent<CanvasGroup>().DOFade(1f, 0.3f)
                    .OnComplete(()=> dialogueTMP.text = dialogues[currentDialogue]);
                Effects(0, 0);
                break;

            case 1:
                skipped = true;
                Sequence sequence_1 = DOTween.Sequence();
                sequence_1.Append(characterObjects[1].GetComponent<RectTransform>().DOAnchorPosY(characterObjects[1].GetComponent<RectTransform>().anchoredPosition.y - 20f, 0.5f))
                        .Join(characterObjects[1].GetComponent<CanvasGroup>().DOFade(1f, 0.5f))
                        .OnComplete(()=> NextDialogue());
                AudioManager.instance.PlaySfx(AudioManager.instance.sfxClips_CutScene[0]);
                break;

            case 2:
                skipped = true;
                Sequence sequence_2 = DOTween.Sequence();
                sequence_2.Append(characterObjects[2].GetComponent<RectTransform>().DOAnchorPosY(characterObjects[2].GetComponent<RectTransform>().anchoredPosition.y - 20f, 0.5f))
                        .Join(characterObjects[2].GetComponent<CanvasGroup>().DOFade(1f, 0.5f))
                        .OnComplete(()=> NextDialogue());
                AudioManager.instance.PlaySfx(AudioManager.instance.sfxClips_CutScene[0]);
                break;

            case 3:
                skipped = true;
                Sequence sequence_3 = DOTween.Sequence();
                sequence_3.Append(characterObjects[3].GetComponent<RectTransform>().DOAnchorPosY(characterObjects[3].GetComponent<RectTransform>().anchoredPosition.y - 20f, 0.5f))
                        .Join(characterObjects[3].GetComponent<CanvasGroup>().DOFade(1f, 0.5f));
                AudioManager.instance.PlaySfx(AudioManager.instance.sfxClips_CutScene[0]);
                break;

            case 4:
                dialogueTMP.text = dialogues[currentDialogue];
                characterObjects[0].GetComponent<Image>().sprite = characterSprites[1];
                characterObjects[0].GetComponent<Image>().SetNativeSize();
                characterObjects[0].transform.localScale = Vector3.one * 2f;
                Effects(0, 1);
                AudioManager.instance.PlaySfx(AudioManager.instance.sfxClips_CutScene[4]);
                break;
        }
    }
}

using UnityEngine;
using TMPro;
using DG.Tweening;
using System.Collections;

public class DeckChangePanel : MonoBehaviour
{
    public DualManager dualManager;

    private Transform dialoguePanle;
    private TextMeshProUGUI dialogueText;
    private RectTransform deck_1;
    private RectTransform deck_2;
    private string dialogue = "아저씨 덱 좋아보이는데요?\n제가 좀 써도 되죠?";
    private string dialogue2 = "으아아악!!! 어째서\n 이런 똥덱을...";
    private float duration = 0.5f;

    private void Awake()
    {
        dialoguePanle = transform.GetChild(0);
        dialogueText = dialoguePanle.GetChild(0).GetComponent<TextMeshProUGUI>();
        deck_1 = transform.GetChild(1).GetComponent<RectTransform>();
        deck_2 = transform.GetChild(2).GetComponent<RectTransform>();

        StartCoroutine(DeckChange());
    }

    private IEnumerator DeckChange()
    {
        dialoguePanle.localScale = Vector3.zero;
        dialogueText.text = "";
        deck_1.anchoredPosition = new Vector2(150, -750);
        deck_2.anchoredPosition = new Vector2(-150, 750);

        AudioManager.instance.PlaySfx(AudioManager.instance.sfxClips_CutScene[0]);
        yield return dialoguePanle.DOScale(Vector3.one, duration).SetEase(Ease.OutBack).WaitForCompletion();
        yield return dialogueText.DOText(dialogue, 1f).SetEase(Ease.Linear).WaitForCompletion();
        
        yield return new WaitForSeconds(duration);

        AudioManager.instance.PlaySfx(AudioManager.instance.sfxClips_Main[8]);
        Tween posTween_1 = deck_1.DOAnchorPosY(0, duration);
        Tween posTween_2 = deck_2.DOAnchorPosY(0, duration);

        yield return new WaitWhile(() =>
            posTween_1.IsActive() && !posTween_1.IsComplete() ||
            posTween_2.IsActive() && !posTween_2.IsComplete()
        );
        
        yield return new WaitForSeconds(0.2f);

        AudioManager.instance.PlaySfx(AudioManager.instance.sfxClips_Main[8]);
        Tween posTween_3 = deck_1.DOAnchorPosY(750, duration);
        Tween posTween_4 = deck_2.DOAnchorPosY(-750, duration);

        yield return new WaitWhile(() =>
            posTween_3.IsActive() && !posTween_3.IsComplete() ||
            posTween_4.IsActive() && !posTween_4.IsComplete()
        );

        bool cardCheck = true;
        CardType type = CardDataManager.instance.GetCardByID(GameManager.instance.deckList[0]).cardType;
        foreach(int item in GameManager.instance.deckList)
        {
            if(type != CardDataManager.instance.GetCardByID(item).cardType)
            {
                cardCheck = false;
                break;
            }
        }

        if(cardCheck)
        {
            dialogueText.text = "";
            yield return dialogueText.GetComponent<RectTransform>().DOShakeAnchorPos(0.3f, 30, 30);
            yield return dialogueText.DOText(dialogue2, 1f).SetEase(Ease.Linear).WaitForCompletion();
            yield return new WaitForSeconds(0.5f);
        }

        yield return dialoguePanle.DOScale(Vector3.zero, duration).SetEase(Ease.InBack).WaitForCompletion();

        dualManager.ActivateDualModeObject();
        gameObject.SetActive(false);
    }
}

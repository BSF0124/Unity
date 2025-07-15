using System.Collections;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class DualStartPanel : MonoBehaviour
{
    public DualManager dualManager;
    public float duration = 0.5f;

    private RectTransform image;
    private TextMeshProUGUI text;

    private void Awake()
    {
        image = transform.GetChild(0).GetComponent<RectTransform>();
        text = transform.GetChild(1).GetComponent<TextMeshProUGUI>();

        StartCoroutine(DualStart());
    }

    private IEnumerator DualStart()
    {
        // 이미지, 텍스트 초기화
        image.localScale = new Vector3(1,0,1);
        text.color = new Color(1,1,1,0);
        yield return new WaitForSeconds(0.5f);
        
        Tween scaleTween_1 = image.DOScaleY(1, duration);
        Tween fadeTween_1 = text.DOFade(1, duration);

        yield return new WaitWhile(() =>
            scaleTween_1.IsActive() && !scaleTween_1.IsComplete() ||
            fadeTween_1.IsActive() && !fadeTween_1.IsComplete()
        );

        yield return new WaitForSeconds(duration);

        Tween scaleTween_2 = image.DOScaleY(0, duration);
        Tween fadeTween_2 = text.DOFade(0, duration);

        yield return new WaitWhile(() =>
            scaleTween_2.IsActive() && !scaleTween_2.IsComplete() ||
            fadeTween_2.IsActive() && !fadeTween_2.IsComplete()
        );

        // 듀얼 모드에 해당하는 오브젝트 활성화
        if(DualManager.stage != null && DualManager.stage.stageID == 18)
        {
            dualManager.animationPanels[3].SetActive(true);
        }
        else
        {
            dualManager.ActivateDualModeObject();
        }

        gameObject.SetActive(false);
    }
}

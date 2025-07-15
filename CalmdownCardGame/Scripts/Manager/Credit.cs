using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class Credit : MonoBehaviour
{
    private ScrollRect scrollRect; // ScrollView의 ScrollRect 컴포넌트
    private float scrollSpeed = 0.01f; // 스크롤 속도 (1.0이 최대)
    private float targetPosition = 0f; // 목표 스크롤 위치
    private bool scrollEnd = false;

    private void Awake()
    {
        AudioManager.instance.bgmPlayer.loop = false;
        AudioManager.instance.PlayBgm(AudioManager.instance.bgmClips_Game[17]);
        AudioManager.instance.bgmPlayer.DOFade(AudioManager.instance.bgmVolume, 0.7f);
        GameManager.instance.isStageSelected = false;

        scrollRect = GetComponent<ScrollRect>();
        PauseManager.instance.canPause = false;
        scrollRect.verticalNormalizedPosition = 1f;
        GetComponent<CanvasGroup>().alpha = 0f;
        GetComponent<CanvasGroup>().DOFade(1f, 0.5f);
    }

    private void Update()
    {
        if(scrollEnd)
            return;
        
        targetPosition = Mathf.Clamp01(targetPosition + scrollSpeed * Time.deltaTime);
        scrollRect.verticalNormalizedPosition = 1 - targetPosition;
        if (scrollRect.verticalNormalizedPosition <= 0f && !scrollEnd)
        {
            scrollRect.verticalNormalizedPosition = 0f;
            StartFadeOut();
        }

        if(Input.GetKeyDown(KeyCode.Escape) && !scrollEnd)
        {
            StartFadeOut();
        }
    }

    private void StartFadeOut()
    {
        scrollEnd = true;
        GetComponent<CanvasGroup>().DOFade(0f, 0.5f)
        .OnComplete(()=>
        {
            StartCoroutine(SceneLoader.instance.LoadScene(5, 0));
        });
    }
}

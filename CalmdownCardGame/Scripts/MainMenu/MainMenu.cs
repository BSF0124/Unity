using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private Button[] buttons;
    [SerializeField] private GameObject optionPanel;

    public Transform titleText;
    public Image background;
    public Image chim;

    private Coroutine coroutine;

    private void Awake()
    {
        if(GameManager.instance.isGameStart)
        {
            background.color = Color.black;
            chim.color = Color.black;
            chim.GetComponent<RectTransform>().anchoredPosition = new Vector2(450f, -1050f);

            titleText.localScale = Vector3.zero;
            foreach(Button item in buttons)
            {
                item.transform.localScale = Vector3.zero;
                item.interactable = false;
            }

            coroutine = StartCoroutine(MainAnimation());
        }
        else
        {
            PauseManager.instance.canPause = true;
            AudioManager.instance.bgmPlayer.loop = true;
            AudioManager.instance.PlayBgm(AudioManager.instance.bgmClips_Game[16]);
            AudioManager.instance.bgmPlayer.DOFade(AudioManager.instance.bgmVolume, 0.7f);
        }
    }

    private void Update()
    {
        if(GameManager.instance.isGameStart && (Input.GetKeyDown(KeyCode.Escape)||Input.GetKeyDown(KeyCode.Space)||Input.GetKeyDown(KeyCode.Return)))
        {
            GameManager.instance.isGameStart = false;
            StopCoroutine(coroutine);
            DOTween.KillAll();
            SkipAnimation();
        }

        if(optionPanel.activeSelf && (Input.GetKeyDown(KeyCode.Escape)||Input.GetKeyDown(KeyCode.Space)||Input.GetKeyDown(KeyCode.Return)))
        {
            optionPanel.SetActive(false);
        }
    }

    private IEnumerator MainAnimation()
    {
        AudioManager.instance.bgmPlayer.loop = false;
        AudioManager.instance.PlayBgm(AudioManager.instance.bgmClips_Game[18]);
        AudioManager.instance.bgmPlayer.DOFade(AudioManager.instance.bgmVolume, 0.7f);

        yield return background.DOColor(new Color(0.7f, 0.7f, 0.7f), 5f);
        yield return chim.GetComponent<RectTransform>().DOAnchorPosY(-40f, 5f).WaitForCompletion();
        yield return chim.DOColor(Color.white, 2f).WaitForCompletion();
        yield return new WaitForSeconds(1f);

        AudioManager.instance.PlaySfx(AudioManager.instance.sfxClips_Main[1]);
        yield return titleText.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack).WaitForCompletion();

        AudioManager.instance.PlaySfx(AudioManager.instance.sfxClips_Main[2]);
        foreach(Button item in buttons)
        {
            item.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);
        }
        yield return new WaitForSeconds(0.5f);

        foreach(Button item in buttons)
        {
            item.interactable = true;
        }

        PauseManager.instance.canPause = true;
        AudioManager.instance.bgmPlayer.loop = true;
        AudioManager.instance.PlayBgm(AudioManager.instance.bgmClips_Game[16]);
        AudioManager.instance.bgmPlayer.DOFade(AudioManager.instance.bgmVolume, 0.7f);
        GameManager.instance.isGameStart = false;
    }

    private  void SkipAnimation()
    {
        AudioManager.instance.StopBgm();
        PauseManager.instance.canPause = true;
        AudioManager.instance.bgmPlayer.loop = true;
        AudioManager.instance.PlayBgm(AudioManager.instance.bgmClips_Game[16]);
        AudioManager.instance.bgmPlayer.DOFade(AudioManager.instance.bgmVolume, 0.7f);

        background.color = new Color(0.7f,0.7f,0.7f);
        chim.color = Color.white;
        chim.GetComponent<RectTransform>().anchoredPosition = new Vector2(450f, -40f);
        
        titleText.localScale = Vector3.one;
        foreach(Button item in buttons)
        {
            item.transform.localScale = Vector3.one;
            item.interactable = true;
        }
    }

    // 게임 시작
    public void GameStart()
    {
        // 씬 이동
        if(SceneLoader.instance != null && PlayerDataManager.instance != null)
        {
            if(PlayerDataManager.instance.playerData.stage[0].stageClear == false)
            {
                StartCoroutine(SceneLoader.instance.LoadScene(0, 2));
            }

            else
            {
                StartCoroutine(SceneLoader.instance.LoadScene(0, 1));
            }
        }
        AudioManager.instance.PlaySfx(AudioManager.instance.sfxClips_Main[5]);
    }

    // 옵션
    public void Option()
    {
        optionPanel.SetActive(true);
        AudioManager.instance.PlaySfx(AudioManager.instance.sfxClips_Main[2]);
    }

    // 크레딧
    public void Credit()
    {
        StartCoroutine(SceneLoader.instance.LoadScene(0, 5));
    }

    // 게임 종료
    public void Exit()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;

        #else
        Application.Quit();
        
        #endif
    }

    // 버튼 텍스트 색 변경
    public void ButtonEnter(int index)
    {
        buttons[index].transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = Color.white;
    }
    public void ButtonExit(int index)
    {
        buttons[index].transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = Color.gray;
    }
}

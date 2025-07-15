using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader instance;
    public Image fadeImage;
    public GameObject cameraObject;
    public GameObject noticeObject;

    private float duration = 0.7f;
    private bool isNoticeOn = false;
    private Coroutine noticeCoroutine;

    private string[] sceneNames = {
        "MainMenu",
        "StoryMode",
        "CutScene",
        "Dual",
        "Nonogram",
        "Credit"
    };

    bool isSceneLoading = false;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        noticeCoroutine = StartCoroutine(Notice());
    }

    private void Update()
    {
        if(isNoticeOn && (Input.GetKeyDown(KeyCode.Escape)||Input.GetKeyDown(KeyCode.Space)||Input.GetKeyDown(KeyCode.Return)))
        {
            isNoticeOn = false;
            StopCoroutine(noticeCoroutine);

            noticeObject.GetComponent<CanvasGroup>().DOFade(0f, duration)
            .OnComplete(()=>
            {
                noticeObject.SetActive(false);
                Destroy(cameraObject);
                SceneManager.LoadScene("MainMenu", LoadSceneMode.Additive);
            });
        }
    }
    private IEnumerator Notice()
    {
        yield return new WaitForSeconds(1f);
        yield return noticeObject.GetComponent<CanvasGroup>().DOFade(1f, duration).WaitForCompletion();
        isNoticeOn = true;
        yield return new WaitForSeconds(7f);
        yield return noticeObject.GetComponent<CanvasGroup>().DOFade(0f, duration).WaitForCompletion();
        yield return new WaitForSeconds(1f);
        noticeObject.SetActive(false);
        Destroy(cameraObject);
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Additive);
        isNoticeOn = false;
    }


    /// <summary>
    /// 현재 씬을 비활성화 한 뒤, 다른 씬을 불러옴
    /// </summary>
    /// <param name="currentScene">현재 씬 (0: MainMenu, 1: StoryMode, 2: CutScene, 3: Dual, 4: Nonogram, 5: Credit)</param>
    /// <param name="loadScene">불러올 씬 (0: MainMenu, 1: StoryMode, 2: CutScene, 3: Dual, 4: Nonogram, 5: Credit)</param>
    /// <returns></returns>
    public IEnumerator LoadScene(int currentScene, int loadScene)
    {
        if(!isSceneLoading)
        {
            PauseManager.instance.canPause = false;
            isSceneLoading = true;
            fadeImage.gameObject.SetActive(true);

            if(AudioManager.instance.bgmPlayer.isPlaying)
            {
                AudioManager.instance.bgmPlayer.DOFade(0, duration)
                .OnComplete(()=>AudioManager.instance.StopBgm());
            }
            // Fade In
            yield return fadeImage.DOFade(1, duration).SetEase(Ease.OutExpo).WaitForCompletion();
            AudioManager.instance.StopAllSfx();

            if(PauseManager.instance.pausePanel.activeSelf)
            {
                PauseManager.instance.pausePanel.SetActive(false);
            }
            // 현재 씬 비활성화
            SceneManager.UnloadSceneAsync(sceneNames[currentScene]);
            // 새로운 씬 로드
            SceneManager.LoadScene(sceneNames[loadScene], LoadSceneMode.Additive);

            // Fade out
            yield return fadeImage.DOFade(0, duration).SetEase(Ease.InExpo).OnComplete(()=> 
            {
                GameManager.instance.isSequnceActivate = false;
                fadeImage.gameObject.SetActive(false);
                isSceneLoading = false;
                PauseManager.instance.canPause = loadScene!=5;
            });
        }
    }

    public int ReturnLoadScene()
    {
        for(int i = 0; i < sceneNames.Length; i++)
        {
            Scene scene = SceneManager.GetSceneByName(sceneNames[i]);
            if(scene.isLoaded)
            {
                return i;
            }
        }
        return -1;
    }
}

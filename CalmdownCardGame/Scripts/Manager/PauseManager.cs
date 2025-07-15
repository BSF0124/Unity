using UnityEngine;

public class PauseManager : MonoBehaviour
{
    public static PauseManager instance;

    public GameObject pausePanel;
    public GameObject optionPanel;

    public GameObject resumeButton;
    public GameObject optionButton;
    public GameObject skipButton;
    public GameObject gotoWorldMapButton;
    public GameObject gotoMainMenuButton;
    public GameObject quitButton;

    public bool canPause = false;
    
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

    private void Update()
    {
        if(canPause && Input.GetKeyDown(KeyCode.Escape) && SceneLoader.instance.ReturnLoadScene() != 0)
        {
            if(pausePanel.activeSelf)
            {
                if(optionPanel.activeSelf)
                {
                    optionPanel.SetActive(false);
                }
                else
                {
                    pausePanel.SetActive(false);
                }
            }

            else
            {
                gotoWorldMapButton.SetActive(SceneLoader.instance.ReturnLoadScene() != 1);
                skipButton.SetActive(SceneLoader.instance.ReturnLoadScene() == 2);
                optionPanel.SetActive(false);
                pausePanel.SetActive(true);
                AudioManager.instance.PlaySfx(AudioManager.instance.sfxClips_Main[4]);
            }
        }

        if(SceneLoader.instance.ReturnLoadScene() == 0)
        {
            optionPanel.SetActive(false);
            pausePanel.SetActive(false);
        }
    }

    public void Resume()
    {
        pausePanel.SetActive(false);
        AudioManager.instance.PlaySfx(AudioManager.instance.sfxClips_Main[6]);
    }

    public void SkipCutScene()
    {
        GameManager.instance.isSkipedCutScene = true;
        pausePanel.SetActive(false);
    }

    public void Option()
    {
        optionPanel.SetActive(true);
        AudioManager.instance.PlaySfx(AudioManager.instance.sfxClips_Main[2]);
    }

    public void GotoWorldMap()
    {
        GameManager.instance.isStageSelected = SceneLoader.instance.ReturnLoadScene() != 4;
        GameManager.instance.isStageClear = false;
        GameManager.instance.isSequnceActivate = false;
        GameManager.instance.deckList.Clear();
        StartCoroutine(SceneLoader.instance.LoadScene(SceneLoader.instance.ReturnLoadScene(), 1));
        pausePanel.SetActive(false);
        AudioManager.instance.PlaySfx(AudioManager.instance.sfxClips_Main[2]);
    }

    public void GotoMainMenu()
    {
        GameManager.instance.isStageSelected = false;
        GameManager.instance.isStageClear = false;
        GameManager.instance.isSequnceActivate = false;
        GameManager.instance.deckList.Clear();
        StartCoroutine(SceneLoader.instance.LoadScene(SceneLoader.instance.ReturnLoadScene(), 0));
        pausePanel.SetActive(false);
        AudioManager.instance.PlaySfx(AudioManager.instance.sfxClips_Main[2]);
    }

    public void Quit()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;

        #else
        Application.Quit();
        
        #endif
    }
}

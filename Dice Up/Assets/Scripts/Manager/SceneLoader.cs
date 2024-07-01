using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader instance;

    public RectTransform diceImage;         // 화면 전환에 사용될 diceImage

    public float duration = 1f;         // 화면 전환 시간
    
    [HideInInspector] public Vector3 diceAnchoredPosition;    // diceImage의 초기 위치
    [HideInInspector] public Dictionary<string, LoadSceneMode> loadScenes = new Dictionary<string, LoadSceneMode>();  // 씬 목록

    private Sequence diceSequence;    // 화면 전환 효과 1

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
        diceAnchoredPosition = diceImage.anchoredPosition;
        InitSceneInfo();
        SetDiceSequence();

        SceneManager.LoadScene("Menu", LoadSceneMode.Additive);
    }

    // 씬의 정보를 추가
    void InitSceneInfo()
    {
        loadScenes.Add("Menu", LoadSceneMode.Additive);
        loadScenes.Add("Game", LoadSceneMode.Additive);
    }

    // 씬을 불러오는 메서드
    public IEnumerator LoadScene(string sceneName, LoadSceneMode mode)
    {
        // 화면 전환 효과 1 실행
        diceImage.gameObject.SetActive(true);
        diceSequence.Restart();

        yield return new WaitForSeconds(duration * 2);

        // 다른 씬을 비활성화
        if(sceneName == "Menu" && SceneManager.GetSceneByName("Game").isLoaded)
        {
            SceneManager.UnloadSceneAsync("Game");
        }
        else if(sceneName == "Game" && SceneManager.GetSceneByName("Menu").isLoaded)
        {
            SceneManager.UnloadSceneAsync("Menu");
        }
        else
        {
            SceneManager.UnloadSceneAsync("Game");
        }
        // 씬을 불러옴
        SceneManager.LoadScene(sceneName, mode);
        
        yield return new WaitForSeconds(duration);
    }

    // 화면 전환 효과 시퀀스
    private void SetDiceSequence()
    {
        diceSequence = DOTween.Sequence();

        diceSequence.Append(diceImage.DOAnchorPos(new Vector2(0, 0), duration).SetEase(Ease.OutQuad))
        .Join(diceImage.DORotate(new Vector3(0, 0, 0), duration).SetEase(Ease.OutQuad))
        .AppendInterval(duration)
        .Append(diceImage.DOAnchorPos(new Vector2(-diceAnchoredPosition.x, 0), duration).SetEase(Ease.OutQuad))
        .Join(diceImage.DORotate(new Vector3(0, 0, -45), duration).SetEase(Ease.OutQuad))
        .OnComplete(SetDiceImageReset)
        .SetAutoKill(false)
        .Pause();
    }

    // 화면 전환 후 DiceImage 초기화
    private void SetDiceImageReset()
    {
        diceImage.gameObject.SetActive(false);
        diceImage.anchoredPosition = diceAnchoredPosition;
        diceImage.rotation = Quaternion.Euler(0, 0, 45);
    }
}

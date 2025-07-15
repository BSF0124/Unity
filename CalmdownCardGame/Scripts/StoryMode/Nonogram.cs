using UnityEngine;

public class Nonogram : MonoBehaviour
{
    public void SelectDifficulty(int difficulty)
    {
        if(GameManager.instance != null)
            GameManager.instance.current_Difficulty = (NNG_Difficulty)difficulty;
            
        // 씬 이동
        if(SceneLoader.instance != null)
            StartCoroutine(SceneLoader.instance.LoadScene(1, 4));

        AudioManager.instance.PlaySfx(AudioManager.instance.sfxClips_Main[5]);
    }
}

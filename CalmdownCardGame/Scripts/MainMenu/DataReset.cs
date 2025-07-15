using UnityEngine;

public class DataReset : MonoBehaviour
{  
    public void ResetData()
    {
        if(PlayerDataManager.instance != null)
        {
            PlayerDataManager.instance.ResetData();
        }
        gameObject.SetActive(false);
        AudioManager.instance.PlaySfx(AudioManager.instance.sfxClips_Main[2]);
    }

    public void Cancle()
    {
        gameObject.SetActive(false);
        AudioManager.instance.PlaySfx(AudioManager.instance.sfxClips_Main[2]);
    }
}

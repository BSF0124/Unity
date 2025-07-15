using UnityEngine;
using UnityEngine.EventSystems;

public class MenuButton : MonoBehaviour, IPointerEnterHandler
{
    public void MenuButtonClick()
    {
        PauseManager.instance.gotoWorldMapButton.SetActive(SceneLoader.instance.ReturnLoadScene() != 1);
        PauseManager.instance.skipButton.SetActive(SceneLoader.instance.ReturnLoadScene() == 2);
        PauseManager.instance.optionPanel.SetActive(false);
        PauseManager.instance.pausePanel.SetActive(true);
        AudioManager.instance.PlaySfx(AudioManager.instance.sfxClips_Main[4]);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(SceneLoader.instance.ReturnLoadScene() == 3)
        {
            if(!DualManager.isSequenceRunning)
                AudioManager.instance.PlaySfx(AudioManager.instance.sfxClips_Main[0]);
        }
        else
        {
            AudioManager.instance.PlaySfx(AudioManager.instance.sfxClips_Main[0]);
        }
    }
}

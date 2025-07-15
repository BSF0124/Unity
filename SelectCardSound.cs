using UnityEngine;
using UnityEngine.EventSystems;

public class SelectCardSound : MonoBehaviour, IPointerEnterHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        if(!DualManager.isSequenceRunning)
            AudioManager.instance.PlaySfx(AudioManager.instance.sfxClips_Main[0]);
    }
}

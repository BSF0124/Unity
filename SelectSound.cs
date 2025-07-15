using UnityEngine;
using UnityEngine.EventSystems;

public class SelectSound : MonoBehaviour, IPointerEnterHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        AudioManager.instance.PlaySfx(AudioManager.instance.sfxClips_Main[0]);
    }
}

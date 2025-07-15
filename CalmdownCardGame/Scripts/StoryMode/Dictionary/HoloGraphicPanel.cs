using UnityEngine;
using UnityEngine.EventSystems;

public class HoloGraphicPanel : MonoBehaviour, IPointerClickHandler
{
    public Dictionary dictionary;

    public void OnPointerClick(PointerEventData eventData)
    {
        dictionary.HoloGraphicDeactivate();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            dictionary.HoloGraphicDeactivate();
        }
    }
}

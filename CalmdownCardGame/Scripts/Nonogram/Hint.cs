using UnityEngine;
using UnityEngine.EventSystems;

public class Hint : MonoBehaviour, IPointerClickHandler
{
    private GameObject xImage;

    public bool isChecked = false;
    public bool isAutoChecked = false;
    public bool isColumn = false;
    public int line;
    public int order;

    private void Start()
    {
        xImage = transform.GetChild(0).GetChild(0).gameObject;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(eventData.button == PointerEventData.InputButton.Left)
        {
            isChecked = !isChecked;
            Check();
            AudioManager.instance.PlaySfx(AudioManager.instance.sfxClips_Main[0]);
        }
    }

    public void Check()
    {
        xImage.SetActive(isChecked);
    }

    public void AutoCheck(bool enable)
    {
        isAutoChecked = enable;
        
        if(!isChecked)
        {
            xImage.SetActive(isAutoChecked);
        }
    }
}

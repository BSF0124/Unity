using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using DG.Tweening;

public class StageCard : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public int stageIndex;
    public Sprite[] stageImages;
    public Sprite backImage;

    private Image image;
    private TextMeshProUGUI nameText;
    private TextMeshProUGUI explainText;

    void Awake()
    {
        image = GetComponent<Image>();
        nameText = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        explainText = transform.GetChild(1).GetComponent<TextMeshProUGUI>();

        Material mat = nameText.fontMaterial;
        mat.SetFloat(ShaderUtilities.ID_FaceDilate, 1f);
        mat.SetFloat(ShaderUtilities.ID_OutlineWidth, 1f);
        nameText.fontMaterial = mat;


        // 스테이지 클리어
        if(PlayerDataManager.instance != null)
        {
            if(stageIndex != 20)
            {
                // 스테이지 잠금 해제
                if(PlayerDataManager.instance.playerData.stage[stageIndex].stageClear)
                {
                    image.sprite = stageImages[0];
                    nameText.gameObject.SetActive(true);
                    explainText.gameObject.SetActive(true);

                    GetComponent<Button>().interactable = true;
                }

                // 스테이지 잠김
                else
                {
                    if(PlayerDataManager.instance.playerData.stage[stageIndex-1].stageClear)
                    {
                        image.sprite = stageImages[1];
                        nameText.gameObject.SetActive(true);
                        nameText.text = "???";
                        GetComponent<Button>().interactable = true;
                    }

                    else
                    {
                        image.sprite = backImage;
                        nameText.gameObject.SetActive(false);
                        GetComponent<Button>().interactable = false;
                    }

                    explainText.gameObject.SetActive(false);
                }
            }

            // 모든 것의 신
            else
            {
                if(PlayerDataManager.instance.playerData.stage[stageIndex].stageClear)
                {
                    image.sprite = stageImages[0];
                    nameText.gameObject.SetActive(false);
                    explainText.gameObject.SetActive(false);

                    GetComponent<Button>().interactable = true;
                }

                // 스테이지 잠김
                else
                {
                    if(PlayerDataManager.instance.playerData.stage[stageIndex-1].stageClear)
                    {
                        image.sprite = stageImages[1];
                        nameText.gameObject.SetActive(true);
                        explainText.gameObject.SetActive(true);
                        GetComponent<Button>().interactable = true;
                    }

                    else
                    {
                        image.sprite = backImage;
                        nameText.gameObject.SetActive(false);
                        explainText.gameObject.SetActive(false);
                        GetComponent<Button>().interactable = false;
                    }

                }
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.DOScale(Vector3.one*1.1f, 0.3f).SetEase(Ease.OutBack);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.InBack);
    }
}

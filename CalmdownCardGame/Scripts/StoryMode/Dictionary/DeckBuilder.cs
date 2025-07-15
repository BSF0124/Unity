using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class DeckBuilder : MonoBehaviour
{
    public GameObject deckListCardPrefab;
    public Transform verticalLayoutGroup;
    public Button submitButton;
    public Button hideButton;
    public Button showButton;

    public int deckCount;
    public bool isDeckFull = false;

    private void OnEnable()
    {
        if(GameManager.instance != null && GameManager.instance.current_Stage > 0)
        {
            deckCount = StageDataManager.instance.GetStageByID(GameManager.instance.current_Stage).deckCount;
        }
        hideButton.gameObject.SetActive(true);
        showButton.gameObject.SetActive(false);
        GetComponent<RectTransform>().anchoredPosition = new Vector2(-190f, 0);
    }

    private void Update()
    {
        if(GameManager.instance != null)
        {
            if(!GameManager.instance.isStageSelected)
            {
                showButton.gameObject.SetActive(false);
                gameObject.SetActive(false);
            }

            else
            {
                SubmitButtonOnOff(isDeckFull);
            }
        }
    }

    private void OnDisable()
    {
        if(GameManager.instance != null && GameManager.instance.deckList.Count != 0)
        {
            
            foreach(Transform child in verticalLayoutGroup)
            {
                Destroy(child.gameObject);
            }
            isDeckFull = false;
            SubmitButtonOnOff(isDeckFull);
            hideButton.gameObject.SetActive(false);
            showButton.gameObject.SetActive(false);
        }
    }

    public void AddCard(int cardID)
    {
        if(GameManager.instance != null && PlayerDataManager.instance != null && !isDeckFull)
        {
            DeckListCard deckListCard;

            if(!GameManager.instance.deckList.Contains(cardID))
            {
                GameObject temp1 = Instantiate(deckListCardPrefab, verticalLayoutGroup);
                deckListCard = temp1.GetComponent<DeckListCard>();
                deckListCard.Init(cardID);
            }

            else
            {
                foreach(Transform child in verticalLayoutGroup)
                {
                    deckListCard = child.GetComponent<DeckListCard>();
                    if(deckListCard.cardID == cardID)
                    {
                        deckListCard.count++;
                    }
                }
            }

            GameManager.instance.deckList.Add(cardID);
            PlayerDataManager.instance.playerData.cardOwnerships[cardID].quantity--;

            if(GameManager.instance.deckList.Count == deckCount)
            {
                isDeckFull = true;
                submitButton.GetComponent<RectTransform>().DOShakePosition(0.5f, 30, 30);
            }
        }
    }

    public void RemoveCard(int cardID)
    {
        if(GameManager.instance != null && PlayerDataManager.instance != null)
        {
            GameManager.instance.deckList.Remove(cardID);

            foreach(Transform child in verticalLayoutGroup)
            {
                DeckListCard deckListCard = child.GetComponent<DeckListCard>();
                if(deckListCard.cardID == cardID)
                {
                    if(deckListCard.count <= 1)
                    {
                        Destroy(child.gameObject);
                    }
                    else
                    {
                        deckListCard.count--;
                    }
                }
            }
            PlayerDataManager.instance.playerData.cardOwnerships[cardID].quantity++;

            if(isDeckFull)
            {
                isDeckFull = false;
            }
        }
    }

    private void SubmitButtonOnOff(bool on)
    {
        if(on)
        {
            Color color;
            ColorUtility.TryParseHtmlString("#F24822", out color);
            submitButton.GetComponent<Image>().color = color;
            submitButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "완료";
            submitButton.interactable = true;
        }
        else
        {
            submitButton.GetComponent<Image>().color = Color.white;
            submitButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = $"{GameManager.instance.deckList.Count} / {deckCount}";
            submitButton.interactable = false;
        }
    }

    public void SubmitButtonClick()
    {
        if(isDeckFull && SceneLoader.instance != null)
        {
            if(GameManager.instance != null)
            {
                PlayerDataManager.instance.SaveData();
                AudioManager.instance.PlaySfx(AudioManager.instance.sfxClips_Main[5]);
                List<int> indexs = new List<int>() {1,2,3};
                if(indexs.Contains(GameManager.instance.current_Stage))
                {
                    StartCoroutine(SceneLoader.instance.LoadScene(1, 3));
                }

                else
                {
                    StartCoroutine(SceneLoader.instance.LoadScene(1, 2));
                }
            }
        }
    }

    public void Hide()
    {
        hideButton.gameObject.SetActive(false);
        showButton.gameObject.SetActive(true);
        GetComponent<RectTransform>().DOAnchorPosX(220f, 0.3f);
        AudioManager.instance.PlaySfx(AudioManager.instance.sfxClips_Main[1]);
    }

    public void Show()
    {
        hideButton.gameObject.SetActive(true);
        showButton.gameObject.SetActive(false);
        GetComponent<RectTransform>().DOAnchorPosX(-190f, 0.3f);
        AudioManager.instance.PlaySfx(AudioManager.instance.sfxClips_Main[2]);
    }
}
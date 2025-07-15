using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using Febucci.UI;

public class CutScene : MonoBehaviour
{
    public int character;
    public GameObject dialoguePanel;
    public GameObject[] characterObjects;
    public Sprite[] characterSprites;

    [HideInInspector] public TextMeshProUGUI dialogueTMP;
    [HideInInspector] public GameObject[] nameObjects;
    [HideInInspector] public string[] dialogues;
    [HideInInspector] public int currentDialogue = -1;

    public TypewriterByCharacter typewriter;
    public bool skipped = false;

    private void Start()
    {
        if(character > 0)
        {
            dialogueTMP = transform.GetChild(2).GetChild(0).GetComponent<TextMeshProUGUI>();
            dialogueTMP.transform.parent.GetComponent<CanvasGroup>().alpha = 0f;
            nameObjects = new GameObject[character];
            for(int i=0; i<character; i++)
            {
                nameObjects[i] = dialogueTMP.transform.GetChild(i).gameObject;
            }
            typewriter = dialogueTMP.GetComponent<TypewriterByCharacter>();
            typewriter.waitLong = 0.2f;

            typewriter.onTextShowed.AddListener(SkippedCheck);
            dialogueTMP.transform.parent.GetComponent<Button>().onClick.AddListener(DialogueClick);
        }

        NextDialogue();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
        {
            DialogueClick();
        }

        if(GameManager.instance.isSkipedCutScene)
        {
            SkipCutScene();
        }
    }

    public virtual void DialogueClick()
    {
        // 다음 대화
        if(skipped)
        {
            skipped = false;
            NextDialogue();
        }

        // 대화 스킵
        else
        {
            if(typewriter != null)
                typewriter.SkipTypewriter();
        }
    }

    public void NextDialogue()
    {
        currentDialogue++;

        if(currentDialogue >= dialogues.Length)
        {
            // 씬 로드
            if(GameManager.instance != null && SceneLoader.instance != null)
            {
                if(GameManager.instance.isStageClear)
                {
                    if(GameManager.instance.current_Stage == 20)
                    {
                        transform.parent.GetComponent<CutSceneManager>().SetEpilogue();
                    }
                    else
                        StartCoroutine(SceneLoader.instance.LoadScene(2, 1));
                }
                else
                {
                    StartCoroutine(SceneLoader.instance.LoadScene(2, 3));
                }
            }
        }

        else
        {
            AudioManager.instance.StopAllSfx();
            DialogueEffect();
        }
    }

    public virtual void DialogueEffect()
    {

    }

    public void Effects(int index, int type)
    {
        RectTransform rectTransform = characterObjects[index].GetComponent<RectTransform>();

        switch(type)
        {
            case 0:
                rectTransform.DOShakeAnchorPos(0.3f, new Vector2(0, 20));
                break;
            case 1:
                rectTransform.DOShakeAnchorPos(0.3f, 30, 30);
                break;
            case 2:
                float targetScale = rectTransform.localScale.x;
                rectTransform.localScale = Vector3.zero;
                rectTransform.DOScale(targetScale, 0.3f).SetEase(Ease.OutBounce);
                break;
        }
    }

    public void SetSpeaker(int index)
    {
        if(!dialoguePanel.activeSelf)
        {
            dialoguePanel.SetActive(true);
        }

        if(index == -1)
        {
            foreach(GameObject name in nameObjects)
            {
                name.SetActive(false);
            }
        }

        else
        {
            for(int i=0; i<character; i++)
            {
                if(i == index)
                {
                    if(nameObjects.Length > i && nameObjects[i] != null)
                    {
                        nameObjects[i].SetActive(true);
                    }

                    if(characterObjects.Length > i && characterObjects[i] != null)
                    {
                        characterObjects[i].GetComponent<Image>().color = Color.white;
                        Transform parentTransform = characterObjects[i].transform.parent;
                        if(parentTransform != null)
                        {
                            int childCount = parentTransform.childCount;
                            characterObjects[i].transform.SetSiblingIndex(childCount - 1);
                        }
                    }
                }
                else
                {
                    if(nameObjects.Length > i && nameObjects[i] != null)
                    {
                        nameObjects[i].SetActive(false);
                    }

                    if(characterObjects.Length > i && characterObjects[i] != null)
                    {
                        characterObjects[i].GetComponent<Image>().color = Color.gray;
                    }
                }
            }
        }
    }

    public void GotoDual()
    {
        if(SceneLoader.instance != null)
        {
            StartCoroutine(SceneLoader.instance.LoadScene(2, 3));
        }
    }

    public void GotoWorldMap()
    {
        if(SceneLoader.instance != null)
        {
            StartCoroutine(SceneLoader.instance.LoadScene(2, 1));
        }
    }

    public void SkippedCheck()
    {
        skipped = true;
    }

    public void SkipCutScene()
    {
        GameManager.instance.isSkipedCutScene = false;
        currentDialogue = dialogues.Length;
        NextDialogue();
    }
}

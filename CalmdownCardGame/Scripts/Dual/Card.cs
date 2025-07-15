using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;


public class Card : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    public Material dissolve_Mat;   // Dissolve 쉐이더 머테리얼
    public Sprite backImage;        // 카드 뒷면 이미지
    public float duration = 0.3f;
    
    [HideInInspector] public RectTransform rectTransform;
    [HideInInspector] public RectTransform cardRect;
    [HideInInspector] public Image cardImage;
    [HideInInspector] public Vector2 dragOffset;
    [HideInInspector] public Vector2 initalPosition;    // 초기 위치
    [HideInInspector] public Quaternion initalRotation;    // 초기 각도


    // Dissolve 색
    public Color[] colors = 
    {
        new Color(2f,2f,2f,1f),
        new Color(0f, 1.3f, 4.3f, 1f),
        new Color(2.2f, 0f, 3f, 1f),
        new Color(3f, 0.5f, 0f, 1f)
    };

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        cardRect = transform.GetChild(0).GetComponent<RectTransform>();
        cardImage = transform.GetChild(0).GetComponent<Image>();
        initalPosition = rectTransform.anchoredPosition;
        initalRotation = rectTransform.rotation;
    }

    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        if(!DualManager.isSequenceRunning && !DualManager.isDragging)
        {
            cardRect.DOScale(new Vector3(1.1f, 1.1f, 1.1f), duration);
        }
    }

    public virtual void OnPointerExit(PointerEventData eventData)
    {
        if(!DualManager.isSequenceRunning && !DualManager.isDragging)
        {
            cardRect.DOScale(new Vector3(1f, 1f, 1f), duration);
        }
    }

    // 드래그 시작 위치 저장
    public virtual void OnBeginDrag(PointerEventData eventData)
    {
        if(!DualManager.isSequenceRunning && !DualManager.isGameOver)
        {
            DualManager.isDragging = true;
            cardRect.localScale = Vector3.one;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(DualManager.canvas.transform as RectTransform, eventData.position, eventData.pressEventCamera, out var localPointerPosition);
            dragOffset = localPointerPosition - rectTransform.anchoredPosition;
        }
    }

    // 카드의 위치를 드래그한 위치로 이동
    public virtual void OnDrag(PointerEventData eventData)
    {
        if(!DualManager.isSequenceRunning && !DualManager.isGameOver)
        {
            cardRect.localScale = Vector3.one;
            transform.DORotate(Vector3.zero, duration);
            Vector2 mousePosition;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(DualManager.canvas.transform as RectTransform, eventData.position, eventData.pressEventCamera, out mousePosition);
            rectTransform.anchoredPosition = mousePosition - dragOffset;
        }
    }

    // 드래그 종료 후 초기 위치로 이동
    public virtual void OnEndDrag(PointerEventData eventData)
    {
        if(!DualManager.isSequenceRunning && !DualManager.isGameOver)
        {
            rectTransform.DOAnchorPos(initalPosition, duration);
            transform.DORotateQuaternion(initalRotation, duration)
                .OnComplete(()=>{
                    DualManager.isDragging = false;
                    transform.rotation = initalRotation;
                    cardRect.localScale = Vector3.one;
                });
        }
    }

    public IEnumerator Flip(Sprite flipImage)
    {
        AudioManager.instance.PlaySfx(AudioManager.instance.sfxClips_DualMode[10]);
        yield return cardImage.transform.DORotate(new Vector3(0, -90, 0), 0.3f).WaitForCompletion();
    
        cardImage.sprite = flipImage;
        cardImage.transform.rotation = Quaternion.Euler(new Vector3(0, 90, 0));
        yield return new WaitForSeconds(0.3f);

        yield return cardImage.transform.DORotate(Vector3.zero, 0.3f).WaitForCompletion();
    }

    public IEnumerator Disappear(int index)
    {
        cardImage.material = dissolve_Mat;
        cardImage.material = new Material(cardImage.material);
        cardImage.material.SetTexture("_MainTex", HoloGraphicCard.SpriteToTexture2D(cardImage.GetComponent<Image>().sprite));
        cardImage.material.SetFloat("_Fade", 1);
        cardImage.material.SetFloat("_Scale", 20);
        cardImage.material.SetColor("_Color", colors[index]);

        if(cardImage.material != null)
        {
            float elapsedTime = 0f;
            //cardImage.material.SetFloat("_Fade", 1f);

            while(elapsedTime < 0.5f)
            {
                elapsedTime += Time.deltaTime;
                float _fade = Mathf.Lerp(1f, 0f, elapsedTime / 0.5f);
                cardImage.material.SetFloat("_Fade", _fade);
                yield return null;
            }
            cardImage.material.SetFloat("_Fade", 0f);
        }
    }

    public IEnumerator Appear(int index)
    {
        cardImage.material = dissolve_Mat;
        cardImage.material = new Material(cardImage.material);
        cardImage.material.SetTexture("_MainTex", HoloGraphicCard.SpriteToTexture2D(cardImage.GetComponent<Image>().sprite));
        cardImage.material.SetFloat("_Fade", 0);
        cardImage.material.SetFloat("_Scale", 20);
        cardImage.material.SetColor("_Color", colors[index]);

        if(cardImage.material != null)
        {
            float elapsedTime = 0f;
            //cardImage.material.SetFloat("_Fade", 1f);

            while(elapsedTime < 0.5f)
            {
                elapsedTime += Time.deltaTime;
                float _fade = Mathf.Lerp(0f, 1f, elapsedTime / 0.5f);
                cardImage.material.SetFloat("_Fade", _fade);
                yield return null;
            }

            cardImage.material.SetFloat("_Fade", 1f);
        }
    }

    public IEnumerator CreateCard()
    {
        cardImage.transform.localScale = Vector3.zero;
        cardImage.material = null;
        AudioManager.instance.PlaySfx(AudioManager.instance.sfxClips_Main[2]);
        yield return cardImage.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack).WaitForCompletion();
    }
}

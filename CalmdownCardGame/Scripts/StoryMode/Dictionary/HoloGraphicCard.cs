using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

public class HoloGraphicCard : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerMoveHandler
{
    public Vector3 cardRotateValue;     // 카드 회전 값
    public Canvas canvas;
    public RectTransform rectTransform;
    public RectTransform cardObject;
    public Image image;
    public Sprite holoTexture;
    public bool isMouseOver = false;   // 커서가 카드 위에 있는지 여부

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        cardObject = transform.GetChild(0).GetComponent<RectTransform>();
        image = transform.GetChild(0).GetComponent<Image>();
        image.material = new Material(image.material);
        image.color = new Color(1f,1f,1f,0f);
    }

    // 스프라이트 및 머테리얼 설정
    public void Init(CardData cardData)
    {
        image.sprite = cardData.cardSprite;

        image.material.SetTexture("_MainTex", SpriteToTexture2D(cardData.cardSprite));
        image.material.SetTexture("_Mask", SpriteToTexture2D(cardData.maskSprite));
        image.material.SetTexture("_HoloTex", SpriteToTexture2D(holoTexture));
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isMouseOver = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isMouseOver = false;
        CardRotateReset();
    }

    public void OnPointerMove(PointerEventData eventData)
    {
        // 커서가 카드 위에 있을 때 실행
        if (isMouseOver)
        {
            Vector2 localCursor;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, Input.mousePosition, canvas.worldCamera, out localCursor);
            float pivotX = (localCursor.x + rectTransform.rect.width / 2) / rectTransform.rect.width;
            float pivotY = (localCursor.y + rectTransform.rect.height / 2) / rectTransform.rect.height;

            // X, Y 회전 값을 선형 보간을 통해 계산
            float rotateX = Mathf.Lerp(-cardRotateValue.x, cardRotateValue.x, pivotY);
            float rotateY = Mathf.Lerp(cardRotateValue.y, -cardRotateValue.y, pivotX);

            // 카드 오브젝트의 회전 값 설정
            cardObject.rotation = Quaternion.Euler(rotateX, rotateY, 0);
        }
    }

    // 카드 Rotate 리셋 (포커스 이후)
    private void CardRotateReset()
    {
        cardObject.DORotate(Vector3.zero, 0.5f);
    }

    public static Texture2D SpriteToTexture2D(Sprite sprite)
    {
        if(sprite.rect.width != sprite.texture.width)
        {
            Texture2D newText = new Texture2D((int)sprite.rect.width,(int)sprite.rect.height);
            Color[] newColors = sprite.texture.GetPixels(
                (int)sprite.textureRect.x,
                (int)sprite.textureRect.y,
                (int)sprite.textureRect.width,
                (int)sprite.textureRect.height);

            newText.SetPixels(newColors);
            newText.Apply();
            return newText;
        } 
        else
            return sprite.texture;
    }
}

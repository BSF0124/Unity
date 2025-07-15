using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using UnityEngine.UI;

public class Card_TCM : Card, IPointerClickHandler
{
    public bool correct;
    public bool fake;

    // 초기 위치와 회전값 설정
    private void OnEnable()
    {
        rectTransform.anchoredPosition = initalPosition;
        cardImage.sprite = backImage;
        transform.localScale = Vector3.zero;
        transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(!DualManager.isSequenceRunning)
        {
            StartCoroutine(transform.parent.parent.GetComponent<ThreeCardMonteManager>().SelectCard(correct));
        }
    }

    public IEnumerator Fake_Flip(Sprite flipImage_1, Sprite flipImage_2)
    {
        yield return rectTransform.DORotate(new Vector3(0, -90, 0), 0.3f).WaitForCompletion();

        cardImage.sprite = flipImage_1;
        cardImage.transform.GetChild(0).GetComponent<Image>().sprite = flipImage_2;
        cardImage.transform.GetChild(0).GetComponent<RectTransform>().anchoredPosition = new Vector2(6, 0);
        
        cardImage.transform.GetChild(0).gameObject.SetActive(true);
        
        rectTransform.rotation = Quaternion.Euler(new Vector3(0, 90, 0));
        yield return new WaitForSeconds(0.3f);

        yield return rectTransform.DORotate(Vector3.zero, 0.3f).WaitForCompletion();
    }

    public override void OnBeginDrag(PointerEventData eventData)
    {
    }

    public override void OnDrag(PointerEventData eventData)
    {
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
    }
}

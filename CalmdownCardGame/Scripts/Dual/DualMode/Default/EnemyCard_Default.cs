using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using UnityEngine.UI;

public class EnemyCard_Default : Card, IPointerClickHandler
{
    public Tracker tracker;
    public List<CardStatus> enemyCards = new List<CardStatus>();
    public Status status;
    public Status status_2;

    public void Start()
    {
        status = transform.GetChild(0).GetChild(0).GetComponent<Status>();
        status_2 = transform.GetChild(1).GetChild(0).GetComponent<Status>();
    }

    // 트래커 On/Off
    public void OnPointerClick(PointerEventData eventData)
    {
        if(!DualManager.isDragging && !DualManager.isSequenceRunning)
        {
            if(tracker.gameObject.activeSelf)
            {
                AudioManager.instance.PlaySfx(AudioManager.instance.sfxClips_Main[1]);
                tracker.HideTracker();
                transform.parent.GetComponent<DefaultManager>().isTrackerActivated = false;
            }
            else
            {
                AudioManager.instance.PlaySfx(AudioManager.instance.sfxClips_Main[2]);
                tracker.ShowTracker(); 
                transform.parent.GetComponent<DefaultManager>().isTrackerActivated = true;
            }
            rectTransform.DOAnchorPos(initalPosition, duration);
        }
    }

    public IEnumerator Flip_2(Sprite flipImage)
    {
        Transform card = transform.GetChild(1);
        AudioManager.instance.PlaySfx(AudioManager.instance.sfxClips_DualMode[10]);
        yield return card.DORotate(new Vector3(0, -90, 0), 0.3f).WaitForCompletion();
    
        card.GetComponent<Image>().sprite = flipImage;
        card.rotation = Quaternion.Euler(new Vector3(0, 90, 0));
        yield return new WaitForSeconds(0.3f);

        yield return card.DORotate(Vector3.zero, 0.3f).WaitForCompletion();
    }
}

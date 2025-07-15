using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Enemy
{
    public CardData cardData;
    public int life;

    // 카드 스텟(생명력 관리)
    public Enemy(int cardID)
    {
        cardData = CardDataManager.instance.GetCardByID(cardID);

        switch(cardData.cardRarity)
        {
            case CardRarity.N:
                life = 1;
                break;
            case CardRarity.R:
                life = 2;
                break;
            case CardRarity.SR:
                life = 3;
                break;
            case CardRarity.Null:
                life = 1;
                break;
        }
    }
}

public class EnemyCard_Poker : MonoBehaviour
{
    public HandRanking handRanking;
    public Material dissolve_Mat;

    public List<Enemy> enemyDatas = new List<Enemy>();

    // Dissolve 색
    private Color[] colors = 
    {
        new Color(2f,2f,2f,1f),
        new Color(0f, 1.3f, 4.3f, 1f),
        new Color(2.2f, 0f, 3f, 1f),
        new Color(3f, 0.5f, 0f, 1f)
    };

    public IEnumerator Flip_Poker_1(Sprite flipImage)
    {
        AudioManager.instance.PlaySfx(AudioManager.instance.sfxClips_DualMode[10]);
        RectTransform cardRect = transform.GetChild(0).GetComponent<RectTransform>();

        yield return cardRect.DORotate(new Vector3(0, -90, 0), 0.3f).WaitForCompletion();
    
        transform.GetChild(0).GetComponent<Image>().sprite = flipImage;
        cardRect.rotation = Quaternion.Euler(new Vector3(0, 90, 0));
        yield return new WaitForSeconds(0.3f);

        yield return cardRect.DORotate(Vector3.zero, 0.3f).WaitForCompletion();
    }

    public IEnumerator Flip_Poker_2(Sprite flipImage)
    {
        AudioManager.instance.PlaySfx(AudioManager.instance.sfxClips_DualMode[10]);
        RectTransform cardRect = transform.GetChild(1).GetComponent<RectTransform>();

        yield return cardRect.DORotate(new Vector3(0, -90, 0), 0.3f).WaitForCompletion();
    
        transform.GetChild(1).GetComponent<Image>().sprite = flipImage;
        cardRect.rotation = Quaternion.Euler(new Vector3(0, 90, 0));
        yield return new WaitForSeconds(0.3f);

        yield return cardRect.DORotate(Vector3.zero, 0.3f).WaitForCompletion();
    }

    public IEnumerator Disappear_1(int index)
    {
        Image cardImage = transform.GetChild(0).GetComponent<Image>();
        cardImage.material = dissolve_Mat;
        cardImage.material = new Material(cardImage.material);
        cardImage.material.SetTexture("_MainTex", HoloGraphicCard.SpriteToTexture2D(cardImage.GetComponent<Image>().sprite));
        cardImage.material.SetFloat("_Fade", 1);
        cardImage.material.SetFloat("_Scale", 20);
        cardImage.material.SetColor("_Color", colors[index]);

        if(cardImage.material != null)
        {
            float elapsedTime = 0f;

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

    public IEnumerator Disappear_2(int index)
    {
        Image cardImage = transform.GetChild(1).GetComponent<Image>();
        cardImage.material = dissolve_Mat;
        cardImage.material = new Material(cardImage.material);
        cardImage.material.SetTexture("_MainTex", HoloGraphicCard.SpriteToTexture2D(cardImage.GetComponent<Image>().sprite));
        cardImage.material.SetFloat("_Fade", 1);
        cardImage.material.SetFloat("_Scale", 20);
        cardImage.material.SetColor("_Color", colors[index]);

        if(cardImage.material != null)
        {
            float elapsedTime = 0f;

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
}

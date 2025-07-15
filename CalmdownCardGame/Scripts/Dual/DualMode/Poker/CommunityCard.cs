using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CommunityCard : MonoBehaviour
{
    public GameObject comunityCardPrefab;
    public List<CardData> comunityCards = new List<CardData>();

    private float[] posX = new float[4] {-330f,-110f,110f,330f};

    public IEnumerator CreateComunityCards()
    {
        if(comunityCards.Count > 0)
        {
            AudioManager.instance.PlaySfx(AudioManager.instance.sfxClips_DualMode[3]);
            for(int i = comunityCards.Count - 1; i >= 0; i--)
            {
                StartCoroutine(transform.GetChild(i).GetComponent<Card>().Disappear((int)comunityCards[i].cardRarity));
            }
            yield return new WaitForSeconds(0.5f);

            foreach(Transform item in transform)
            {
                Destroy(item.gameObject);
            }
            comunityCards.Clear();
        }
        

        for(int i = 0; i < 4; i++)
        {
            int index;
            do{index = Random.Range(0, 30);}
            while(index == 11);
            comunityCards.Add(CardDataManager.instance.GetCardByID(index));

            GameObject _card = Instantiate(comunityCardPrefab, transform);
            PokerCard card = _card.GetComponent<PokerCard>();

            // 스프라이트, 포지션 설정
            card.Init(index);
            card.status.cardData = card.cardData;
            card.status.Init();
            card.rectTransform.anchoredPosition = new Vector2(posX[i],0);
            card.initalPosition = card.rectTransform.anchoredPosition;

            // 머테리얼 설정
            card.cardImage.material = card.dissolve_Mat;
            card.cardImage.material = new Material(card.cardImage.material);
            card.cardImage.GetComponent<Image>().material.SetFloat("_Fade", 0);

            // 스테이터스 설정
            card.status.Init();

            StartCoroutine(card.Appear((int)comunityCards[i].cardRarity));
        }
        AudioManager.instance.PlaySfx(AudioManager.instance.sfxClips_DualMode[2]);
    }
}

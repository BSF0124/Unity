using UnityEngine;

public class PlayingCard : Platform
{
    public Sprite[] sprites;
    
    void Awake()
    {
        GetComponent<SpriteRenderer>().sprite = sprites[Random.Range(0, sprites.Length)];

        int num = Random.Range(0, 4);
        switch(num)
        {
            case 0:
                transform.rotation = Quaternion.Euler(new Vector3(0, 0, Random.Range(-10f, 10f)));
                break;
            case 1:
                transform.rotation = Quaternion.Euler(new Vector3(0, 0, Random.Range(80f, 100f)));
                break;
            case 2:
                transform.rotation = Quaternion.Euler(new Vector3(0, 0, Random.Range(170f, 190f)));
                break;
            case 3:
                transform.rotation = Quaternion.Euler(new Vector3(0, 0, Random.Range(260f, 280f)));
                break;
        }
    }
}

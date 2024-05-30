using UnityEngine;

public class Janggi_Prefab : MonoBehaviour
{
    public Sprite[] sprites;

    void Awake()
    {
        GetComponent<SpriteRenderer>().sprite = sprites[Random.Range(0, sprites.Length)];
    }
}

using UnityEngine;

public class Janggi : Platform
{
    public GameObject[] janggi_Prefab;

    void Awake()
    {
        float rand = Random.Range(0f, 1f);

        if (rand < 0.70f)
        {
            Instantiate(janggi_Prefab[0], Vector2.zero, Quaternion.identity, transform);
        }
        else if (rand < 0.95f)
        {
            Instantiate(janggi_Prefab[1], Vector2.zero, Quaternion.identity, transform);
        }
        else
        {
            Instantiate(janggi_Prefab[2], Vector2.zero, Quaternion.identity, transform);
        }
    }
}

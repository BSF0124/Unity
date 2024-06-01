using UnityEngine;

public class Chess : Platform
{
    public GameObject[] chess_Prefab;

    private int type;
    
    void Awake()
    {
        type = Random.Range(0, chess_Prefab.Length);
        GameObject chess = Instantiate(chess_Prefab[type], Vector2.zero, Quaternion.identity, transform);

        int num = Random.Range(0, 2);

        switch(num)
        {
            case 0:
                chess.transform.rotation = Quaternion.Euler(new Vector3(0, 0, Random.Range(80f, 100f)));
                break;
            case 1:
                chess.transform.rotation = Quaternion.Euler(new Vector3(0, 0, Random.Range(260f, 280f)));
                break;
            
        }

        chess.transform.localPosition = Vector3.zero;
    }
}

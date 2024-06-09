using UnityEngine;

public class PlatformManager : MonoBehaviour
{
    [SerializeField] private GameObject[] platform_Prefabs;
    // private int count = 0;
    private bool isLeft = false;

    private float coordinates_Y;
    
    void Awake()
    {
        coordinates_Y = Random.Range(-4.0f, -3.0f);
        Instantiate(platform_Prefabs[0], new Vector2(Random.Range(2, PlayerPrefs.GetFloat("screenRight")), coordinates_Y), Quaternion.identity, transform);
    }

    void Update()
    {
        CreatePlatform();
    }

    private void CreatePlatform()
    {
        if(coordinates_Y + 5f >= PlayerPrefs.GetFloat("CreateLine"))
            return;

        else
        {
            Vector2 position;

            if(isLeft)
            {
                position = new Vector2(Random.Range(2, PlayerPrefs.GetFloat("screenRight")),
                Random.Range(3.0f, 4.0f) + coordinates_Y);
                coordinates_Y = position.y;
                isLeft = false;
            }

            else
            {
                position = new Vector2(Random.Range(PlayerPrefs.GetFloat("screenLeft"), -2),
                Random.Range(3.0f, 4.0f) + coordinates_Y);
                coordinates_Y = position.y;
                isLeft = true;
            }
            
            if(PlayerPrefs.GetInt("Score") >= 60)
            {
                Instantiate(platform_Prefabs[Random.Range(0, platform_Prefabs.Length)], position, Quaternion.identity, transform);
            }
            else if(PlayerPrefs.GetInt("Score") >= 50)
            {
                Instantiate(platform_Prefabs[5], position, Quaternion.identity, transform);
            }
            else if(PlayerPrefs.GetInt("Score") >= 40)
            {
                Instantiate(platform_Prefabs[4], position, Quaternion.identity, transform);
            }
            else if(PlayerPrefs.GetInt("Score") >= 30)
            {
                Instantiate(platform_Prefabs[3], position, Quaternion.identity, transform);
            }
            else if(PlayerPrefs.GetInt("Score") >= 20)
            {
                Instantiate(platform_Prefabs[2], position, Quaternion.identity, transform);
            }
            else if(PlayerPrefs.GetInt("Score") >= 10)
            {
                Instantiate(platform_Prefabs[1], position, Quaternion.identity, transform);
            }
            else
            {
                Instantiate(platform_Prefabs[0], position, Quaternion.identity, transform);
            }
        }
    }
}

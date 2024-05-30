using UnityEngine;

public class PlatformManager : MonoBehaviour
{
    [SerializeField] private GameObject[] platform_Prefabs;
    private int count = 0;

    private float coordinates_Y;
    
    void Awake()
    {
        coordinates_Y = Random.Range(-3.0f, -2.0f);
        count++;
        GameObject platform = Instantiate(platform_Prefabs[0], new Vector2(Random.Range(2, PlayerPrefs.GetFloat("screenRight")), coordinates_Y), Quaternion.identity, transform);
        platform.transform.name = count.ToString();
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
            GameObject platform;
            Vector2 position;

            count++;
            if(count % 2 == 0)
            {
                position = new Vector2(Random.Range(PlayerPrefs.GetFloat("screenLeft"), -2),
                Random.Range(4.0f, 6.0f) + coordinates_Y);
                coordinates_Y = position.y;
            }

            else
            {
                position = new Vector2(Random.Range(2, PlayerPrefs.GetFloat("screenRight")),
                Random.Range(4.0f, 6.0f) + coordinates_Y);
                coordinates_Y = position.y;
            }

            if(count >= 40)
            {
                platform = Instantiate(platform_Prefabs[4], position, Quaternion.identity, transform);
            }

            else if(count >= 30)
            {
                platform = Instantiate(platform_Prefabs[3], position, Quaternion.identity, transform);
            }
            else if(count >= 20)
            {
                platform = Instantiate(platform_Prefabs[2], position, Quaternion.identity, transform);
            }
            else if(count >= 10)
            {
                platform = Instantiate(platform_Prefabs[1], position, Quaternion.identity, transform);
            }
            else
            {
                platform = Instantiate(platform_Prefabs[0], position, Quaternion.identity, transform);
            }
            platform.transform.name = count.ToString();
        }
    }
}

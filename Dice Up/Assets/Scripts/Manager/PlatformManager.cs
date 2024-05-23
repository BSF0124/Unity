using UnityEngine;

public class PlatformManager : MonoBehaviour
{
    [SerializeField] private GameObject[] platform_Prefabs;
    private int count = 0;

    private float coordinates_Y;
    
    void Start()
    {
        coordinates_Y = Random.Range(-4.5f, -3.5f);
        count++;
        GameObject platform = Instantiate(platform_Prefabs[0], new Vector2(Random.Range(-1, PlayerPrefs.GetFloat("screenRight")), coordinates_Y), Quaternion.identity, transform);
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
            count++;
            Vector2 position;
            if(count % 2 == 0)
            {
                position = new Vector2(Random.Range(PlayerPrefs.GetFloat("screenLeft"), 1),
                Random.Range(4.0f, 6.0f) + coordinates_Y);
                coordinates_Y = position.y;
            }

            else
            {
                position = new Vector2(Random.Range(-1, PlayerPrefs.GetFloat("screenRight")),
                Random.Range(4.0f, 6.0f) + coordinates_Y);
                coordinates_Y = position.y;
            }

            GameObject platform = Instantiate(platform_Prefabs[0], position, Quaternion.identity, transform);
            platform.transform.name = count.ToString();
        }
    }
}

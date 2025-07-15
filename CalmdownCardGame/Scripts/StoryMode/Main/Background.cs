using UnityEngine;
using UnityEngine.UI;

public class Background : MonoBehaviour
{
    [SerializeField] private RawImage _img;
    [SerializeField] private float _x, _y;

    private void Start()
    {
        if(SceneLoader.instance.ReturnLoadScene() == 3)
        {
            if(GameManager.instance.current_Stage == 20)
            {
                GetComponent<RawImage>().color = new Color(0.55f, 0.18f, 0.18f);
            }
        }
    }

    private void Update()
    {
        _img.uvRect = new Rect(_img.uvRect.position + new Vector2(_x, _y) * Time.deltaTime, _img.uvRect.size);
    }
}

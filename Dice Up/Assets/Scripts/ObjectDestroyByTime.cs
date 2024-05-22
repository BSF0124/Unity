using UnityEngine;

public class ObjectDestroyByTime : MonoBehaviour
{
    [SerializeField]
    private float destroyTime;

    private void Awake()
    {
        // destroyTime 시간 뒤에 GameObject 삭제
        Destroy(gameObject, destroyTime);
    }
}

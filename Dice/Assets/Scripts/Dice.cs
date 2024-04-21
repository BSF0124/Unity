using UnityEngine;

public class Dice : MonoBehaviour
{
    public float speed = 1;

    void Update()
    {
        // Input.GetAxis는 약간 밀리게 동작
        // Input.GetAxisRaw는 바로 멈추게 동작
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 temp = new Vector3(h,v, 0).normalized;
        transform.position += temp * speed * Time.deltaTime;
    }
}

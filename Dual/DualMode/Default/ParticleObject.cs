using UnityEngine;

public class ParticleObject : MonoBehaviour
{
    private ParticleSystem particleSystem;

    void Awake()
    {
        // 파티클 시스템을 가져옴
        particleSystem = GetComponent<ParticleSystem>();
    }

    void Update()
    {
        // 파티클 시스템이 멈췄는지 확인
        if (particleSystem && particleSystem.isStopped)
        {
            // 오브젝트 비활성화
            gameObject.SetActive(false);
        }
    }
}

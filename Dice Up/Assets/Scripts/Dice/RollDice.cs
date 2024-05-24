using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RollDice : MonoBehaviour
{
    [SerializeField] private Sprite[] diceImages;       // 주사위 이미지들
    [SerializeField] private GameObject[] chanceImage;  // 굴리기 남은 횟수 표시 이미지
    [SerializeField] private Slider timerSlider;        // 남은 시간을 표시하기 위한 슬라이더

    private SpriteRenderer spriteRenderer;              // 주사위 이미지 변경을 위한 SpriteRenderer 
    private Rigidbody2D rb;                             // 주사위를 굴리기 위한 rigidbodt2D 컴포넌트
    private Vector3 position;                           // 주사위 초기 위치값

    [HideInInspector] public bool isRollEnd = false;    // 굴리기 끝
    [HideInInspector] public int currentDice;           // 현재 주사위 눈
    
    private int chance = 3;                             // 굴리기 횟수
    private float time = 1.5f;                          // 제한 시간
    private float currentTime;                          // 현재 시간
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        timerSlider.maxValue = time;
        position = transform.position;
        SetDice();
    }

    // 활성화 될 때 변수 초기화
    private void OnEnable() 
    {
        isRollEnd = false;
        chance = 3;
        timerSlider.transform.GetChild(1).gameObject.SetActive(true);
        SetDice();
    }

    private void Update()
    {
        // 주사위 굴리기가 모두 끝나면 입력을 받지 않음
        if(isRollEnd)
        {return;}

        // 제한 시간 종료
        if(currentTime <= 0)
        {
            timerSlider.transform.GetChild(1).gameObject.SetActive(false);
            SetRollEnd();
            rb.velocity = Vector3.zero;
        }

        currentTime -= Time.deltaTime;
        timerSlider.value = currentTime;

        // 스페이스바를 눌러 주사위를 다시 굴림
        if(chance != 0 && Input.GetKeyDown(KeyCode.Space))
        {
            chance--;
            SetDice();
        }

    }

    // 벽에 닿으면 무작위 주사위로 변경
    private void OnCollisionEnter2D(Collision2D other) 
    {
        currentDice = Random.Range(1, 7);
        spriteRenderer.sprite = diceImages[currentDice - 1];
    }

    // 굴리기 전 각종 설정을 한 뒤, 주사위를 굴림
    private void SetDice()
    {
        transform.position = position;
        currentDice = Random.Range(1, 7);
        spriteRenderer.sprite = diceImages[currentDice-1];
        currentTime = time;
        SetChanceImage();
        Roll();
    }

    // 굴리기 메서드
    private void Roll()
    {
        Vector3 force = Vector3.zero;
        int rand1 = Random.Range(0, 2);
        int rand2 = Random.Range(0, 2);
        force.x = rand1==0? Random.Range(-1200, -600) : Random.Range(600, 1200);
        force.y = rand2==0? Random.Range(-1200, -600) : Random.Range(600, 1200);
        rb.AddForce(force);
    }

    // 굴리기 남은 횟수 이미지 업데이트
    private void SetChanceImage()
    {
        switch(chance)
        {
            case 3:
                chanceImage[0].SetActive(true);
                chanceImage[1].SetActive(true);
                chanceImage[2].SetActive(true);
                break;
            case 2:
                chanceImage[0].SetActive(true);
                chanceImage[1].SetActive(true);
                chanceImage[2].SetActive(false);
                break;
            case 1:
                chanceImage[0].SetActive(true);
                chanceImage[1].SetActive(false);
                chanceImage[2].SetActive(false);
                break;
            case 0:
                chanceImage[0].SetActive(false);
                chanceImage[1].SetActive(false);
                chanceImage[2].SetActive(false);
                break;
        }
    }

    // 굴리기 종료
    private void SetRollEnd()
    {
        isRollEnd = true;
    }
}

using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RolltheDice : MonoBehaviour
{
    [SerializeField] private Sprite[] diceImages;
    [SerializeField] private TextMeshProUGUI chanceText;
    [SerializeField] private Slider timerSlider;

    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;
    private Vector3 position;

    [HideInInspector] public bool isRollEnd = false;
    [HideInInspector] public int currentDice;
    
    private int chance = 3;
    private float timer = 1.5f;
    private float currentTime;
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        position = transform.position;
        SetDice();
    }

    private void OnEnable() 
    {
        isRollEnd = false;
        chance = 3;
        timerSlider.transform.GetChild(1).gameObject.SetActive(true);
        SetDice();
    }

    private void Update()
    {
        if(isRollEnd)
        {return;}

        if(currentTime <= 0)
        {
            timerSlider.transform.GetChild(1).gameObject.SetActive(false);
            rb.velocity = Vector3.zero;
            Invoke("SetRollEnd", 1f);
        }

        currentTime -= Time.deltaTime;
        timerSlider.value = currentTime;

        if(chance != 0 && Input.GetKeyDown(KeyCode.Space))
        {
            chance--;
            SetDice();
        }

    }

    private void OnCollisionEnter2D(Collision2D other) 
    {
        currentDice = Random.Range(1, 7);
        spriteRenderer.sprite = diceImages[currentDice - 1];
    }
    
    private void SetDice()
    {
        transform.position = position;
        currentDice = Random.Range(1, 7);
        spriteRenderer.sprite = diceImages[currentDice-1];
        currentTime = timer;
        chanceText.text = "X " + chance;
        Roll();
    }

    private void Roll()
    {
        Vector3 force = Vector3.zero;
        int rand1 = Random.Range(0, 2);
        int rand2 = Random.Range(0, 2);
        force.x = rand1==0? Random.Range(-1500, -500) : Random.Range(500, 1500);
        force.y = rand2==0? Random.Range(-1500, -500) : Random.Range(500, 1500);
        rb.AddForce(force);
    }

    private void SetRollEnd()
    {
        isRollEnd = true;
    }
}

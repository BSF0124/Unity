using UnityEngine;

public class RolltheDice : MonoBehaviour
{
    [SerializeField] private Sprite[] diceImages;
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;
    public int currentDice;
    
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        // currentDice = Random.Range(1, 7);
        // spriteRenderer.sprite = diceImages[currentDice-1];
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            Vector3 force = Vector3.zero;
            int rand1 = Random.Range(0, 2);
            int rand2 = Random.Range(0, 2);
            force.x = rand1==0? Random.Range(-1000, -500) : Random.Range(-1000, -500);
            force.y = rand2==0? Random.Range(-1000, -500) : Random.Range(-1000, -500);
            
            rb.AddForce(force);
        }
    }

    private void OnCollisionEnter2D(Collision2D other) {
        spriteRenderer.sprite = diceImages[Random.Range(1, 7)-1];
        
    }
}

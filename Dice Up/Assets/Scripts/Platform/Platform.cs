using UnityEngine;

public class Platform : MonoBehaviour
{
    public virtual void Update()
    {
        if(transform.position.y <= PlayerPrefs.GetFloat("DeadLine"))
        {
            Destroy(gameObject);
        }
    }
}

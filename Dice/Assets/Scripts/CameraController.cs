using System.Collections;
using UnityEngine;
using DG.Tweening;
using UnityEngine.U2D;

public class CameraController : MonoBehaviour
{

    public GameObject dice;         // 플레이어
    private GameObject diceObject;  // 카메라가 추적할 오브젝트     

    public float minX = 0f;   
    public float minY = 0f;
    private float duration = 0.5f;

    private void Awake()
    {
        diceObject = dice;
    }

    private void Update()
    {
        SetCameraPosition();
    }

    public void SetCameraPosition()
    {
        if(diceObject != null)
        {
            Vector3 position = new Vector3(diceObject.transform.position.x, diceObject.transform.position.y, -10);

            if(position.x <= minX)
            {
                position.x = minX;
            }
            if(position.y <= minY)
            {
                position.y = minY;
            }
            transform.position = position;
        }
    }

    public IEnumerator SetDiceObject(GameObject newDice)
    {
        diceObject = null;
        transform.DOMove(newDice.transform.position, duration);
        yield return new WaitForSeconds(duration);
        diceObject = newDice;
    }
}

using UnityEngine;
using DG.Tweening;
using System.Collections;

public class MovingPlatform : Platform
{
    public Sprite[] sprites;
    public GameObject platform_Prefab;

    private GameObject[] platforms;
    private int column;
    private int row;

    private float move_X;
    private float position_X;
    private float duration = 1.5f;

    private void Awake()
    {
        row = Random.Range(2, 5);
        column = Random.Range(1, 3);
        platforms = new GameObject[row * column];

        SetPlatform();
        SetSprite();
        transform.GetComponent<BoxCollider2D>().size = new Vector2(row, column);

        position_X = transform.position.x;
        if(position_X >= 0)
        {
            move_X = Random.Range(-3f, -1f);
        }
        else
        {
            move_X = Random.Range(1f, 3f);
        }
        StartCoroutine(PlatformMovement());
    }

    private void SetPlatform()
    {
        for(int i=0; i<column*row; i++)
        {
            platforms[i] = Instantiate(platform_Prefab, Vector2.zero, Quaternion.identity, transform);
        }

        if(row % 2 == 0)
        {
            if(column % 2 == 0)
            {
                for(int i=0; i<column; i++)
                {
                    for(int j=0; j<row; j++)
                    {
                        platforms[j + row*i].transform.localPosition = new Vector2(-row / 2 + 0.5f + j, -i + 0.5f);
                    }
                }
            }

            else
            {
                for(int i=0; i<row; i++)
                {
                    platforms[i].transform.localPosition =  new Vector2(-row / 2 + 0.5f + i, 0);
                }
            }
        }

        else
        {
            if(column % 2 == 0)
            {
                for(int i=0; i<column; i++)
                {
                    for(int j=0; j<row; j++)
                    {
                        platforms[j + row*i].transform.localPosition = new Vector2(-row / 2 + j, -i + 0.5f);
                    }
                }
            }

            else
            {
                for(int i=0; i<row; i++)
                {
                    platforms[i].transform.localPosition =  new Vector2(-row / 2 + i, 0);
                }
            }
        }
    }

    private void SetSprite()
    {
        platforms[0].GetComponent<SpriteRenderer>().sprite = sprites[0];
        for(int i=1; i<=row-2; i++)
        { 
            platforms[i].GetComponent<SpriteRenderer>().sprite = sprites[1];
        }
        platforms[row-1].GetComponent<SpriteRenderer>().sprite = sprites[2];

        if(column == 2)
        {
            platforms[row].GetComponent<SpriteRenderer>().sprite = sprites[3];
            for(int i=row+1; i<=row*column-2; i++)
            { 
                platforms[i].GetComponent<SpriteRenderer>().sprite = sprites[4];
            }
            platforms[row*column-1].GetComponent<SpriteRenderer>().sprite = sprites[5];
        }
    }

    private IEnumerator PlatformMovement()
    {
        transform.DOMove(new Vector3(transform.position.x+move_X,transform.position.y,transform.position.z), duration).SetEase(Ease.InOutQuad);
        yield return new WaitForSeconds(duration);

        transform.DOMove(new Vector3(position_X,transform.position.y,transform.position.z), duration).SetEase(Ease.InOutQuad);
        yield return new WaitForSeconds(duration);

        StartCoroutine(PlatformMovement());
    }
}

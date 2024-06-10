using UnityEngine;

public class ScreenSet : MonoBehaviour
{
    void Start()
    {
        SetResolution(405, 720);
    }

    public void SetResolution(int setWidth, int setHeight)
    {
        //해상도를 설정값에 따라 변경
        //3번째 파라미터는 풀스크린 모드를 설정 > true : 풀스크린, false : 창모드
        Screen.SetResolution(setWidth, setHeight, false);
    }
}

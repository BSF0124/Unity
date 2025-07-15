using UnityEngine;

public static class ResolutionUtility
{
    public static bool CheckMinimumResolution(int width)
    {
        if(width >= 1024)
            return true;
        
        return false;
    }

    public static bool Check16To9Ratio(int width, int height)
    {
        float aspectRatio = (float)width / height;
        return Mathf.Approximately(aspectRatio, 16.0f / 9.0f);
    }

    public static bool CheckRefreshRateRatio(float refreshRate)
    {
        return Mathf.Approximately(refreshRate, 60f);
    }
}

using UnityEngine;

public class ResolutionData
{
    public readonly int Width;
    public readonly int Height;
    public RefreshRate RefreshRateRatio;

    public ResolutionData(int width, int height, RefreshRate refreshRateRatio)
    {
        Width = width;
        Height = height;
        RefreshRateRatio = refreshRateRatio;
    }

    public override string ToString()
    {
        return $"{Width} x {Height}";
    }
}

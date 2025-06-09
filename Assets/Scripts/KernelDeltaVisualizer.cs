using UnityEngine;

public enum DeltaComputationMode
{
    Mean,
    Max,
    Normalized // ✅ This is the missing value
}

public static class KernelDeltaVisualizer
{
    public static DeltaComputationMode DeltaMode = DeltaComputationMode.Mean;

    public static float ComputeDelta(float[,] a, float[,] b)
    {
        return DeltaMode switch
        {
            DeltaComputationMode.Max => MaxDelta(a, b),
            _ => MeanDelta(a, b),
        };
    }

    public static float MeanDelta(float[,] a, float[,] b)
    {
        int sizeX = a.GetLength(0);
        int sizeY = a.GetLength(1);
        float total = 0f;

        for (int x = 0; x < sizeX; x++)
            for (int y = 0; y < sizeY; y++)
                total += Mathf.Abs(a[x, y] - b[x, y]);

        return total / (sizeX * sizeY);
    }

    public static float MaxDelta(float[,] a, float[,] b)
    {
        int sizeX = a.GetLength(0);
        int sizeY = a.GetLength(1);
        float max = 0f;

        for (int x = 0; x < sizeX; x++)
            for (int y = 0; y < sizeY; y++)
            {
                float delta = Mathf.Abs(a[x, y] - b[x, y]);
                if (delta > max) max = delta;
            }

        return max;
    }

    public static float[,] GetDeltaMatrix(float[,] a, float[,] b)
    {
        int sizeX = a.GetLength(0);
        int sizeY = a.GetLength(1);
        float[,] delta = new float[sizeX, sizeY];

        for (int x = 0; x < sizeX; x++)
            for (int y = 0; y < sizeY; y++)
                delta[x, y] = Mathf.Abs(a[x, y] - b[x, y]);

        return delta;
    }
}
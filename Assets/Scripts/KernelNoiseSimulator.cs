// KernelNoiseSimulator.cs (Extended Version)
// Supports: Global noise control, temporal decay, localized smearing, blended profiles

using UnityEngine;

public static class KernelNoiseSimulator
{
    public enum NoiseModel { None, Depolarizing, GaussianBlur, RandomCollapse }

    // 🔧 Global UI-controlled strength (default 0.1)
    public static float GlobalNoiseStrength = 0.1f;

    // ✅ Global toggle controlled by UI
    public static bool SmearingEnabled = false;
    
    // 📦 Apply a single noise model to an entire kernel matrix
    public static float[,] ApplyNoise(float[,] kernel, NoiseModel model, float strength = -1f)
    {
        int sizeX = kernel.GetLength(0);
        int sizeY = kernel.GetLength(1);
        float[,] noisy = new float[sizeX, sizeY];
        float actualStrength = (strength >= 0) ? strength : GlobalNoiseStrength;

        for (int x = 0; x < sizeX; x++)
        {
            for (int y = 0; y < sizeY; y++)
            {
                noisy[x, y] = Mathf.Clamp01(ApplySingleNoise(kernel[x, y], model, actualStrength));
            }
        }

        return noisy;
    }

    // 🧪 Apply multiple models sequentially (blended effect)
    public static float[,] ApplyBlendedNoise(float[,] kernel, (NoiseModel, float)[] blendProfile)
    {
        float[,] noisy = (float[,])kernel.Clone();

        foreach (var (model, weight) in blendProfile)
            noisy = ApplyNoise(noisy, model, weight);

        return noisy;
    }

    // 🔬 Apply noise to a local region only
    public static float[,] ApplyLocalizedNoise(float[,] kernel, NoiseModel model, float strength, RectInt region)
    {
        int sizeX = kernel.GetLength(0);
        int sizeY = kernel.GetLength(1);
        float[,] noisy = (float[,])kernel.Clone();

        for (int x = region.xMin; x < region.xMax && x < sizeX; x++)
        {
            for (int y = region.yMin; y < region.yMax && y < sizeY; y++)
            {
                noisy[x, y] = Mathf.Clamp01(ApplySingleNoise(kernel[x, y], model, strength));
            }
        }

        return noisy;
    }

    // ⏳ Animate toward decoherence (collapse toward 0.5)
    public static void ApplyTemporalDecay(ref float[,] kernel, NoiseModel model, float deltaTime)
    {
        float strength = deltaTime * GlobalNoiseStrength;
        int sizeX = kernel.GetLength(0);
        int sizeY = kernel.GetLength(1);

        for (int x = 0; x < sizeX; x++)
        {
            for (int y = 0; y < sizeY; y++)
            {
                kernel[x, y] = Mathf.Lerp(kernel[x, y], 0.5f, strength);
            }
        }
    }

    // 🎨 Optional: Create a smeared color map (with opacity decay)
    public static Color MapSmearingColor(float value, float strength)
    {
        float alpha = Mathf.Clamp01(1.0f - strength);
        return new Color(value, 0.2f * value, 1f - value, alpha);
    }

    // 🧩 Core noise function
    private static float ApplySingleNoise(float value, NoiseModel model, float strength)
    {
        return model switch
        {
            NoiseModel.Depolarizing => Mathf.Lerp(value, 0.5f, strength),
            NoiseModel.GaussianBlur => value + Random.Range(-strength, strength),
            NoiseModel.RandomCollapse => Random.value < strength ? 0f : value,
            _ => value
        };
    }
}
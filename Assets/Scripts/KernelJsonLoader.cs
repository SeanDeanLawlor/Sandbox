// KernelJsonLoader.cs (using NativeFilePicker)
using UnityEngine;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;

#if UNITY_EDITOR
using UnityEditor;
#endif

public static class KernelJsonLoader
{
    public static float[,] LoadFromUserPath()
    {
#if UNITY_EDITOR
        string path = EditorUtility.OpenFilePanel("Select Kernel JSON", "", "json");
        if (string.IsNullOrEmpty(path))
        {
            Debug.LogWarning("⚠️ No file selected.");
            return null;
        }
        return LoadFromFile(path);
#else
        string path = NativeFilePicker.PickFile(null, new string[] { "json" });
        if (string.IsNullOrEmpty(path))
        {
            Debug.LogWarning("⚠️ No kernel file picked.");
            return null;
        }
        return LoadFromFile(path);
#endif
    }

    public static float[,] LoadFromFile(string path)
    {
        try
        {
            string json = File.ReadAllText(path);
            var wrapper = JsonConvert.DeserializeObject<KernelWrapper>(json);
            if (wrapper == null || wrapper.kernel == null)
            {
                Debug.LogError("❌ Invalid kernel JSON.");
                return null;
            }

            int rows = wrapper.kernel.Count;
            int cols = wrapper.kernel[0].Count;
            float[,] matrix = new float[rows, cols];

            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                    matrix[i, j] = wrapper.kernel[i][j];

            Debug.Log($"✅ Loaded kernel from {path}");
            return matrix;
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"❌ Failed to load kernel JSON: {ex.Message}");
            return null;
        }
    }

    public static void SaveToFile(string path, float[,] matrix)
    {
        try
        {
            var wrapper = new KernelWrapper { kernel = ToNestedList(matrix) };
            string json = JsonConvert.SerializeObject(wrapper, Formatting.Indented);
            File.WriteAllText(path, json);
            Debug.Log($"💾 Saved kernel to: {path}");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"❌ Failed to save kernel JSON: {ex.Message}");
        }
    }

    private static List<List<float>> ToNestedList(float[,] matrix)
    {
        int rows = matrix.GetLength(0);
        int cols = matrix.GetLength(1);
        var nested = new List<List<float>>(rows);
        for (int i = 0; i < rows; i++)
        {
            var row = new List<float>(cols);
            for (int j = 0; j < cols; j++)
                row.Add(matrix[i, j]);
            nested.Add(row);
        }
        return nested;
    }

    private class KernelWrapper
    {
        public List<List<float>> kernel;
    }
}
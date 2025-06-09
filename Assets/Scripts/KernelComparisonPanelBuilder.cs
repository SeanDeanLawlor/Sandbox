using UnityEngine;
using UnityEngine.UI;
using System;

public class KernelComparisonPanelBuilder : MonoBehaviour
{
    public GameObject BuildPanel(Transform parent, float[,] deltaMatrix, Action onRestore)
    {
        GameObject panel = new GameObject("KernelComparisonPanel");
        panel.transform.SetParent(parent);
        RectTransform rt = panel.AddComponent<RectTransform>();
        rt.sizeDelta = new Vector2(360, 200);

        Image bg = panel.AddComponent<Image>();
        bg.color = new Color(0.2f, 0.1f, 0.1f, 0.85f);
        Font font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        // === Stats
        Text stats = CreateLabel(panel.transform, new Vector2(0, 60), font, $"Mean Δ: {ComputeMean(deltaMatrix):F3} | Max Δ: {ComputeMax(deltaMatrix):F3}");

        // === Toggle Button
        Button toggleBtn = CreateButton(panel.transform, new Vector2(0, 10), font, "Restore View");
        toggleBtn.onClick.AddListener(() => onRestore?.Invoke());

        // === Bar graph (Optional - placeholder)
        GameObject barGraph = new GameObject("BarGraphPlaceholder");
        barGraph.transform.SetParent(panel.transform);
        RectTransform graphRT = barGraph.AddComponent<RectTransform>();
        graphRT.anchoredPosition = new Vector2(0, -50);
        graphRT.sizeDelta = new Vector2(300, 40);
        Image graphBG = barGraph.AddComponent<Image>();
        graphBG.color = new Color(0.8f, 0.3f, 0.3f, 0.5f); // Placeholder visual

        return panel;
    }

    private Text CreateLabel(Transform parent, Vector2 pos, Font font, string content)
    {
        GameObject go = new GameObject("StatLabel");
        go.transform.SetParent(parent);
        RectTransform rt = go.AddComponent<RectTransform>();
        rt.anchoredPosition = pos;
        rt.sizeDelta = new Vector2(300, 30);

        Text label = go.AddComponent<Text>();
        label.font = font;
        label.text = content;
        label.alignment = TextAnchor.MiddleCenter;
        label.color = Color.white;
        return label;
    }

    private Button CreateButton(Transform parent, Vector2 pos, Font font, string label)
    {
        GameObject go = new GameObject("ToggleButton");
        go.transform.SetParent(parent);
        RectTransform rt = go.AddComponent<RectTransform>();
        rt.anchoredPosition = pos;
        rt.sizeDelta = new Vector2(180, 40);

        Button btn = go.AddComponent<Button>();
        Text text = new GameObject("Text").AddComponent<Text>();
        text.transform.SetParent(go.transform);
        text.font = font;
        text.text = label;
        text.color = Color.white;
        text.alignment = TextAnchor.MiddleCenter;
        return btn;
    }

    private float ComputeMean(float[,] matrix)
    {
        float sum = 0;
        foreach (float val in matrix) sum += val;
        return sum / (matrix.GetLength(0) * matrix.GetLength(1));
    }

    private float ComputeMax(float[,] matrix)
    {
        float max = float.MinValue;
        foreach (float val in matrix)
            max = Mathf.Max(max, val);
        return max;
    }
}
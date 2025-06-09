using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class HypothesisTestPanelBuilder : MonoBehaviour
{
    public GameObject BuildPanel(Transform parent, KernelExplorer explorer)
    {
        GameObject panel = new GameObject("HypothesisTestPanel", typeof(RectTransform));
        panel.transform.SetParent(parent, false);

        RectTransform rt = panel.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(360, 260);
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = Vector2.zero;

        Image bg = panel.AddComponent<Image>();
        bg.color = new Color(0.1f, 0.1f, 0.1f, 0.85f);

        Font font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        List<string> kernels = new List<string>(explorer.GetAvailableKernelNames());

        Dropdown dropdownA = CreateDropdown(panel.transform, new Vector2(0, 80), kernels, font);
        Dropdown dropdownB = CreateDropdown(panel.transform, new Vector2(0, 40), kernels, font);
        Text output = null;

        Button compareBtn = CreateButton(panel.transform, new Vector2(0, 0), font, "Compare Kernels", () =>
        {
            string k1 = dropdownA.options[dropdownA.value].text;
            string k2 = dropdownB.options[dropdownB.value].text;

            float[,] matA = explorer.GetKernelMatrix(k1);
            float[,] matB = explorer.GetKernelMatrix(k2);

            KernelDeltaVisualizer.DeltaMode = DeltaComputationMode.Normalized;
            float[,] delta = KernelDeltaVisualizer.GetDeltaMatrix(matA, matB);
            explorer.DisplayDeltaMatrix(delta);


            float mean = KernelDeltaVisualizer.MeanDelta(matA, matB);
            float max = KernelDeltaVisualizer.MaxDelta(matA, matB);
            if (output != null)
                output.text = $"Δ Mean: {mean:F3}, Max: {max:F3}";

            Debug.Log($"🧠 Compared {k1} vs {k2}. Δ Mean: {mean}, Max: {max}");
        });

        output = CreateLabel(panel.transform, new Vector2(0, -60), font, "Δ Kernel will be shown here...");

        return panel;
    }

    Dropdown CreateDropdown(Transform parent, Vector2 pos, List<string> options, Font font)
    {
        GameObject go = new GameObject("Dropdown", typeof(RectTransform));
        go.transform.SetParent(parent, false);

        RectTransform rt = go.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(200, 30);
        rt.anchoredPosition = pos;

        Dropdown dd = go.AddComponent<Dropdown>();
        dd.captionText = CreateLabel(go.transform, Vector2.zero, font, options.Count > 0 ? options[0] : "None");
        dd.options.Clear();
        foreach (string opt in options)
            dd.options.Add(new Dropdown.OptionData(opt));

        return dd;
    }

    Button CreateButton(Transform parent, Vector2 pos, Font font, string label, UnityEngine.Events.UnityAction action)
    {
        GameObject go = new GameObject("Button", typeof(RectTransform));
        go.transform.SetParent(parent, false);

        RectTransform rt = go.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(180, 30);
        rt.anchoredPosition = pos;

        Image img = go.AddComponent<Image>();
        img.color = new Color(0.2f, 0.4f, 0.8f);

        Button btn = go.AddComponent<Button>();
        btn.onClick.AddListener(action);

        Text txt = new GameObject("Text").AddComponent<Text>();
        txt.transform.SetParent(go.transform, false);
        txt.font = font;
        txt.text = label;
        txt.color = Color.white;
        txt.alignment = TextAnchor.MiddleCenter;
        RectTransform textRT = txt.GetComponent<RectTransform>();
        textRT.sizeDelta = rt.sizeDelta;
        textRT.anchoredPosition = Vector2.zero;

        return btn;
    }

    Text CreateLabel(Transform parent, Vector2 pos, Font font, string content)
    {
        GameObject go = new GameObject("Label", typeof(RectTransform));
        go.transform.SetParent(parent, false);

        RectTransform rt = go.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(300, 30);
        rt.anchoredPosition = pos;

        Text lbl = go.AddComponent<Text>();
        lbl.font = font;
        lbl.text = content;
        lbl.color = Color.cyan;
        lbl.alignment = TextAnchor.MiddleCenter;

        return lbl;
    }
}
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ZXRewriterPanelBuilder : MonoBehaviour
{
    public GameObject BuildPanel(Transform parent, ZXRewriter zxRewriter)
    {
        GameObject panel = new GameObject("ZXRewriterPanel");
        panel.transform.SetParent(parent);
        panel.AddComponent<CanvasRenderer>();
        RectTransform rt = panel.AddComponent<RectTransform>();
        rt.sizeDelta = new Vector2(380, 440);

        Image bg = panel.AddComponent<Image>();
        bg.color = new Color(0.08f, 0.08f, 0.08f, 0.9f);
        Font font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        // === Provider Dropdown ===
        Dropdown providerDropdown = CreateDropdown(panel.transform, new Vector2(0, 160), new List<string> { "OpenAI", "Local", "Custom" });
        zxRewriter.providerDropdown = providerDropdown;

        // === Endpoint Field ===
        InputField endpointField = CreateInput(panel.transform, new Vector2(0, 120), font, "http://localhost:5050/rewrite");
        zxRewriter.endpointField = endpointField;

        // Autofill endpoint based on provider selection
        providerDropdown.onValueChanged.AddListener((index) =>
        {
            string provider = providerDropdown.options[index].text.ToLower();
            if (provider == "openai")
                endpointField.text = "https://api.openai.com/v1/chat/completions";
            else if (provider == "local")
                endpointField.text = "http://localhost:5050/rewrite";
        });

        // === AI Toggle ===
        Toggle aiToggle = CreateToggle(panel.transform, new Vector2(0, 80), font, "Use AI Rewrite");
        aiToggle.isOn = true;
        aiToggle.onValueChanged.AddListener((val) => zxRewriter.useAI = val);

        // === Circuit Input ===
        InputField circuitInput = CreateInput(panel.transform, new Vector2(0, 40), font, "Enter circuit...");
        zxRewriter.circuitInput = circuitInput;

        // === Rewrite Button ===
        Button rewriteBtn = CreateButton(panel.transform, new Vector2(0, 0), font, "Rewrite Circuit");
        zxRewriter.rewriteButton = rewriteBtn;

        // === Circuit Label ===
        Text circuitDisplay = CreateLabel(panel.transform, new Vector2(0, -40), font, "Circuit Output...");
        zxRewriter.zxCircuitLabel = circuitDisplay;

        // === Feedback Label ===
        Text resultLabel = CreateLabel(panel.transform, new Vector2(0, -80), font, "Awaiting rewrite...");
        zxRewriter.resultLabel = resultLabel;

        // === Snapshot Button ===
        var snapBtn = CreateButton(panel.transform, new Vector2(0, -120), font, "📸 Save ZX Snapshot");

        snapBtn.onClick.AddListener(() => {
        ZXDiagramViewer viewer = FindObjectOfType<ZXDiagramViewer>();

        SnapshotUtility.TakeSnapshot(
            zxRewriter.circuitInput.text,            // circuit
            null,                                     // kernelMatrix
            "ZXSnapshot",                             // kernelLabel
            zxRewriter.lastRules ?? new string[0],    // qubitLabels (used for rules here)
            GlobalSettings.SelectedQubitType.ToString(), // qubitType
            0f,                                       // fidelity
            "default",                                // colorMap
            new Vector2Int[0],                        // errorPositions
            new Vector2Int[0],                        // syndromePositions
            zxRewriter.lastRules ?? new string[0],    // zxRules
            zxRewriter.lastRewritten ?? "",           // zxRewrittenStr
            "ZX_Rewriter",                            // zxDiagramName
            viewer != null ? viewer.GetDiagram() : null // zxDiagram
        );
    });

        return panel;
    }



    private InputField CreateInput(Transform parent, Vector2 pos, Font font, string placeholder)
    {
        GameObject go = new GameObject("InputField");
        go.transform.SetParent(parent);
        RectTransform rt = go.AddComponent<RectTransform>();
        rt.anchoredPosition = pos;
        rt.sizeDelta = new Vector2(300, 30);

        InputField field = go.AddComponent<InputField>();
        Text text = new GameObject("Text").AddComponent<Text>();
        text.transform.SetParent(go.transform);
        text.font = font;
        text.color = Color.white;
        field.textComponent = text;

        Text ph = new GameObject("Placeholder").AddComponent<Text>();
        ph.transform.SetParent(go.transform);
        ph.font = font;
        ph.text = placeholder;
        ph.color = Color.gray;
        field.placeholder = ph;

        return field;
    }

    private Button CreateButton(Transform parent, Vector2 pos, Font font, string label)
    {
        GameObject go = new GameObject("Button");
        go.transform.SetParent(parent);
        RectTransform rt = go.AddComponent<RectTransform>();
        rt.anchoredPosition = pos;
        rt.sizeDelta = new Vector2(200, 30);

        Button btn = go.AddComponent<Button>();
        Text btnText = new GameObject("Text").AddComponent<Text>();
        btnText.transform.SetParent(go.transform);
        btnText.font = font;
        btnText.text = label;
        btnText.alignment = TextAnchor.MiddleCenter;
        btnText.color = Color.white;

        return btn;
    }

    private Text CreateLabel(Transform parent, Vector2 pos, Font font, string content)
    {
        GameObject go = new GameObject("Label");
        go.transform.SetParent(parent);
        RectTransform rt = go.AddComponent<RectTransform>();
        rt.anchoredPosition = pos;
        rt.sizeDelta = new Vector2(320, 30);

        Text label = go.AddComponent<Text>();
        label.font = font;
        label.text = content;
        label.color = Color.white;
        label.alignment = TextAnchor.MiddleCenter;

        return label;
    }

    private Toggle CreateToggle(Transform parent, Vector2 pos, Font font, string label)
    {
        GameObject go = new GameObject("Toggle");
        go.transform.SetParent(parent);
        RectTransform rt = go.AddComponent<RectTransform>();
        rt.anchoredPosition = pos;
        rt.sizeDelta = new Vector2(280, 30);

        Toggle toggle = go.AddComponent<Toggle>();
        Text lbl = new GameObject("Label").AddComponent<Text>();
        lbl.transform.SetParent(go.transform);
        lbl.font = font;
        lbl.text = label;
        lbl.color = Color.white;
        lbl.rectTransform.anchoredPosition = new Vector2(80, 0);
        lbl.rectTransform.sizeDelta = rt.sizeDelta;

        return toggle;
    }

    private Dropdown CreateDropdown(Transform parent, Vector2 pos, List<string> options)
    {
        GameObject go = new GameObject("Dropdown");
        go.transform.SetParent(parent);
        RectTransform rt = go.AddComponent<RectTransform>();
        rt.anchoredPosition = pos;
        rt.sizeDelta = new Vector2(240, 30);

        Dropdown dd = go.AddComponent<Dropdown>();
        dd.AddOptions(options);
        return dd;
    }
}
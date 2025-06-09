// ZXDiagramPanelBuilder.cs (final with Save + Load + Toggles)
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ZXDiagramPanelBuilder : MonoBehaviour
{
    public GameObject BuildPanel(Transform parent, ZXDiagramJsonLoader loaderScript)
    {
        GameObject panel = new GameObject("ZXDiagramPanel");
        panel.transform.SetParent(parent);
        panel.AddComponent<CanvasRenderer>();
        RectTransform rt = panel.AddComponent<RectTransform>();
        rt.sizeDelta = new Vector2(360, 320);

        Image bg = panel.AddComponent<Image>();
        bg.color = new Color(0.1f, 0.1f, 0.1f, 0.88f);

        Font font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        // === Load Diagram Button ===
        Button loadBtn = CreateButton(panel.transform, new Vector2(0, 120), new Vector2(180, 30), font, "📂 Load ZX Diagram");
        loadBtn.onClick.AddListener(() => loaderScript.OpenFileDialogAndLoad());
        // === Export Diagram Button ===
        Button exportBtn = CreateButton(panel.transform, new Vector2(0, 80), new Vector2(180, 30), font, "💾 Export Diagram");
        exportBtn.onClick.AddListener(() => loaderScript.ExportCurrentDiagram());

        // === Save As Field + Button ===
        InputField saveInput = CreateInputField(panel.transform, new Vector2(-60, 40), new Vector2(160, 30), font, "Diagram name");
        Button saveBtn = CreateButton(panel.transform, new Vector2(100, 40), new Vector2(80, 30), font, "Save");
        saveBtn.onClick.AddListener(() =>
        {
            if (!string.IsNullOrEmpty(saveInput.text))
            {
                var diagram = loaderScript.viewer.GetCurrentDiagram();
                ZXDiagramManager.SaveDiagram(diagram, saveInput.text);
                loaderScript.statusLabel.text = "✅ Saved as: " + saveInput.text;
            }
        });

        // === Load Dropdown ===
        Dropdown loadDropdown = CreateDropdown(panel.transform, new Vector2(0, 0), new Vector2(200, 30), ZXDiagramManager.ListSavedDiagrams());
        loadDropdown.onValueChanged.AddListener(index =>
        {
            string name = loadDropdown.options[index].text;
            var diagram = ZXDiagramManager.LoadDiagramByName(name);
            loaderScript.viewer.DisplayDiagram(diagram);
            loaderScript.statusLabel.text = "📂 Loaded: " + name;
        });

        // === Export Toggles ===
        Toggle autoExportToggle = CreateToggle(panel.transform, new Vector2(-100, -40), font, "Auto-export");
        loaderScript.autoExportToggle = autoExportToggle;

        Toggle bundleToggle = CreateToggle(panel.transform, new Vector2(80, -40), font, "Snapshot bundle");
        loaderScript.snapshotBundleToggle = bundleToggle;

        // === 3D View Toggle ===
        Toggle view3D = CreateToggle(panel.transform, new Vector2(0, -130), font, "🧊 3D View");
        loaderScript.viewer.use3DViewToggle = view3D;
        view3D.onValueChanged.AddListener(_ => loaderScript.viewer.Redraw());

        // === Status Label ===
        Text label = CreateLabel(panel.transform, new Vector2(0, -170), new Vector2(300, 30), font, "🧮 ZX diagram loader ready");
        loaderScript.statusLabel = label;

        return panel;
    }

    private Button CreateButton(Transform parent, Vector2 pos, Vector2 size, Font font, string label)
    {
        GameObject go = new GameObject("Button");
        go.transform.SetParent(parent);
        RectTransform rt = go.AddComponent<RectTransform>();
        rt.anchoredPosition = pos;
        rt.sizeDelta = size;

        Button btn = go.AddComponent<Button>();
        Image img = go.AddComponent<Image>();
        img.color = new Color(0.3f, 0.6f, 0.9f, 0.9f);

        Text text = new GameObject("Text").AddComponent<Text>();
        text.transform.SetParent(go.transform);
        text.font = font;
        text.text = label;
        text.alignment = TextAnchor.MiddleCenter;
        text.color = Color.white;
        text.rectTransform.anchoredPosition = Vector2.zero;
        text.rectTransform.sizeDelta = size;

        return btn;
    }

    private InputField CreateInputField(Transform parent, Vector2 pos, Vector2 size, Font font, string placeholder)
    {
        GameObject go = new GameObject("InputField");
        go.transform.SetParent(parent);
        RectTransform rt = go.AddComponent<RectTransform>();
        rt.anchoredPosition = pos;
        rt.sizeDelta = size;

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

    private Dropdown CreateDropdown(Transform parent, Vector2 pos, Vector2 size, List<string> options)
    {
        GameObject go = new GameObject("Dropdown");
        go.transform.SetParent(parent);
        RectTransform rt = go.AddComponent<RectTransform>();
        rt.anchoredPosition = pos;
        rt.sizeDelta = size;

        Dropdown dd = go.AddComponent<Dropdown>();
        dd.AddOptions(options);
        return dd;
    }

    private Toggle CreateToggle(Transform parent, Vector2 pos, Font font, string label)
    {
        GameObject go = new GameObject("Toggle");
        go.transform.SetParent(parent);
        RectTransform rt = go.AddComponent<RectTransform>();
        rt.anchoredPosition = pos;
        rt.sizeDelta = new Vector2(140, 30);

        Toggle toggle = go.AddComponent<Toggle>();
        Text lbl = new GameObject("Label").AddComponent<Text>();
        lbl.transform.SetParent(go.transform);
        lbl.font = font;
        lbl.text = label;
        lbl.color = Color.white;
        lbl.rectTransform.anchoredPosition = new Vector2(60, 0);
        lbl.rectTransform.sizeDelta = rt.sizeDelta;

        return toggle;
    }

    private Text CreateLabel(Transform parent, Vector2 pos, Vector2 size, Font font, string content)
    {
        GameObject go = new GameObject("Label");
        go.transform.SetParent(parent);
        RectTransform rt = go.AddComponent<RectTransform>();
        rt.anchoredPosition = pos;
        rt.sizeDelta = size;

        Text label = go.AddComponent<Text>();
        label.font = font;
        label.text = content;
        label.color = Color.white;
        label.alignment = TextAnchor.MiddleCenter;

        return label;
    }
}

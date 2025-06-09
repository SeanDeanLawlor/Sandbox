// KernelLoaderPanelBuilder.cs (Finalized with CanvasRenderer + LegacyRuntime font support)
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class KernelLoaderPanelBuilder : MonoBehaviour
{
    // Overload with 3 args for FullSceneBuilder
    public GameObject BuildPanel(Transform parent, KernelLoaderUI loaderUI, KernelExplorer explorer)
    {
        loaderUI.kernelExplorer = explorer;
        return BuildPanel(parent, loaderUI);
    }

    // Overload with 4 args for SceneStartup
    public GameObject BuildPanel(Transform parent, KernelLoaderUI loaderUI, KernelExplorer explorer, LicenseKeyManager licenseMgr)
    {
        loaderUI.kernelExplorer = explorer;
        return BuildPanel(parent, loaderUI);
    }

    // Core method
    public GameObject BuildPanel(Transform parent, KernelLoaderUI loaderUI)
    {
        GameObject panel = new GameObject("KernelLoaderPanel", typeof(RectTransform));
        panel.transform.SetParent(parent, false);
        panel.AddComponent<CanvasRenderer>(); // ✅ Needed for UI visuals

        RectTransform rt = panel.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(360, 240);

        VerticalLayoutGroup layout = panel.AddComponent<VerticalLayoutGroup>();
        layout.spacing = 8;
        layout.childForceExpandHeight = false;

        Font font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        loaderUI.manualPathInput = CreateInputField(panel.transform, "Manual Path...", font);
        CreateButton(panel.transform, "Load from Path", font, loaderUI.LoadFromManualPath);
        CreateButton(panel.transform, "Load Sample Kernel", font, loaderUI.LoadBundledSample);
        CreateButton(panel.transform, "Browse...", font, loaderUI.LoadFromFileDialog);

        loaderUI.colorMapDropdown = CreateDropdown(panel.transform, font, new string[] {
            "BlueRed", "Hot", "Cool", "Viridis"
        });

        loaderUI.statusLabel = CreateLabel(panel.transform, font, "Ready.");

        CreateButton(panel.transform, "Take Snapshot", font, () => {
            KernelExplorer explorer = loaderUI.kernelExplorer;
            loaderUI.SetFeedback("📸 Snapshot taken!");

            SnapshotUtility.TakeSnapshot(
                circuit: "",
                kernelMatrix: explorer.GetKernelMatrix(),
                kernelLabel: "Imported",
                qubitLabels: new string[0],
                qubitType: "Data",
                fidelity: 1.0f,
                colorMap: loaderUI.colorMapDropdown.options[loaderUI.colorMapDropdown.value].text,
                errorPositions: new Vector2Int[0],
                syndromePositions: new Vector2Int[0],
                zxRules: new string[0],
                zxRewrittenStr: "",
                zxDiagramName: "",
                zxDiagram: null
            );
        });

        CreateButton(panel.transform, "Export All Snapshots", font, () => {
            SnapshotExportUtility.ExportAllSnapshotsToJson();
            loaderUI.SetFeedback("✅ Exported all snapshots!");
        });

        CreateButton(panel.transform, "Clear Snapshots", font, () => {
            SnapshotManager.Instance.ClearAllSnapshots();
            loaderUI.SetFeedback("🧹 Cleared all snapshots.");
        });

        return panel;
    }

    InputField CreateInputField(Transform parent, string placeholderText, Font font)
    {
        GameObject go = new GameObject("InputField", typeof(RectTransform));
        go.transform.SetParent(parent, false);
        go.AddComponent<CanvasRenderer>(); // ✅ Needed

        InputField input = go.AddComponent<InputField>();

        GameObject textGO = new GameObject("Text");
        textGO.transform.SetParent(go.transform, false);
        textGO.AddComponent<CanvasRenderer>();
        Text text = textGO.AddComponent<Text>();
        text.font = font;
        text.color = Color.black;
        input.textComponent = text;

        GameObject placeholderGO = new GameObject("Placeholder");
        placeholderGO.transform.SetParent(go.transform, false);
        placeholderGO.AddComponent<CanvasRenderer>();
        Text placeholder = placeholderGO.AddComponent<Text>();
        placeholder.font = font;
        placeholder.text = placeholderText;
        placeholder.color = Color.gray;
        input.placeholder = placeholder;

        return input;
    }

    Dropdown CreateDropdown(Transform parent, Font font, string[] options)
    {
        GameObject go = new GameObject("Dropdown", typeof(RectTransform));
        go.transform.SetParent(parent, false);
        go.AddComponent<CanvasRenderer>();

        Dropdown dd = go.AddComponent<Dropdown>();

        GameObject labelGO = new GameObject("Label");
        labelGO.transform.SetParent(go.transform, false);
        labelGO.AddComponent<CanvasRenderer>();
        Text label = labelGO.AddComponent<Text>();
        label.font = font;
        label.color = Color.white;
        dd.captionText = label;

        dd.options.Clear();
        foreach (var o in options)
            dd.options.Add(new Dropdown.OptionData(o));

        return dd;
    }

    Button CreateButton(Transform parent, string labelText, Font font, UnityAction action)
    {
        GameObject go = new GameObject("Button", typeof(RectTransform));
        go.transform.SetParent(parent, false);
        go.AddComponent<CanvasRenderer>();

        Button btn = go.AddComponent<Button>();

        GameObject textGO = new GameObject("Text");
        textGO.transform.SetParent(go.transform, false);
        textGO.AddComponent<CanvasRenderer>();
        Text txt = textGO.AddComponent<Text>();
        txt.font = font;
        txt.text = labelText;
        txt.alignment = TextAnchor.MiddleCenter;
        txt.color = Color.white;

        btn.onClick.AddListener(action);
        return btn;
    }

    Text CreateLabel(Transform parent, Font font, string content)
    {
        GameObject go = new GameObject("Text", typeof(RectTransform));
        go.transform.SetParent(parent, false);
        go.AddComponent<CanvasRenderer>();

        Text lbl = go.AddComponent<Text>();
        lbl.font = font;
        lbl.text = content;
        lbl.color = Color.white;
        lbl.alignment = TextAnchor.MiddleCenter;
        return lbl;
    }
}
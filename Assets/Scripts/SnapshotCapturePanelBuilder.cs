using UnityEngine;
using UnityEngine.UI;

public class SnapshotCapturePanelBuilder : MonoBehaviour
{
    public GameObject BuildPanel(Transform parent)
    {
        GameObject panel = new GameObject("SnapshotCapturePanel");
        panel.transform.SetParent(parent);
        RectTransform rt = panel.AddComponent<RectTransform>();
        rt.sizeDelta = new Vector2(240, 100);

        Image bg = panel.AddComponent<Image>();
        bg.color = new Color(0.1f, 0.1f, 0.1f, 0.8f);
        Font font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        Button snapshotBtn = CreateButton(panel.transform, new Vector2(0, 10), font, "📸 Take Snapshot");
        snapshotBtn.onClick.AddListener(() => {
            string[] mockRules = { "Mock: Rule A", "Mock: Rule B" };
            Vector2Int[] mockErrors = { new Vector2Int(1, 2), new Vector2Int(3, 1) };
            Vector2Int[] mockSyndromes = { new Vector2Int(2, 2) };
            float[,] mockKernel = new float[2, 2] { { 1.0f, 0.2f }, { 0.2f, 1.0f } };

            SnapshotUtility.TakeSnapshot(
                circuit: "H - CNOT - T - H",
                kernelMatrix: mockKernel,
                kernelLabel: GlobalSettings.SelectedQubitType.ToString(),
                qubitLabels: new string[0],
                qubitType: GlobalSettings.SelectedQubitType.ToString(),
                fidelity: 0.94f,
                colorMap: "BlueRed",
                errorPositions: mockErrors,
                syndromePositions: mockSyndromes,
                zxRules: mockRules,
                zxRewrittenStr: "H - Z - T - H",
                zxDiagramName: "MockDiagram",
                zxDiagram: null
            );
        });

        return panel;
    }

    private Button CreateButton(Transform parent, Vector2 pos, Font font, string label)
    {
        GameObject go = new GameObject("SnapshotButton");
        go.transform.SetParent(parent);
        RectTransform rt = go.AddComponent<RectTransform>();
        rt.anchoredPosition = pos;
        rt.sizeDelta = new Vector2(180, 40);

        Button btn = go.AddComponent<Button>();
        Text btnText = new GameObject("Text").AddComponent<Text>();
        btnText.transform.SetParent(go.transform);
        btnText.font = font;
        btnText.text = label;
        btnText.color = Color.white;
        btnText.alignment = TextAnchor.MiddleCenter;
        return btn;
    }
}

// SnapshotTimelinePanelBuilder.cs
using UnityEngine;
using UnityEngine.UI;

public class SnapshotTimelinePanelBuilder : MonoBehaviour
{
    public GameObject BuildPanel(Transform parent)
    {
        GameObject panel = new GameObject("SnapshotTimelinePanel");
        panel.transform.SetParent(parent, false);

        RectTransform rt = panel.AddComponent<RectTransform>();
        rt.sizeDelta = new Vector2(360, 140);
        panel.AddComponent<CanvasRenderer>();

        Image bg = panel.AddComponent<Image>();
        bg.color = new Color(0.1f, 0.1f, 0.1f, 0.88f);

        Font font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        GameObject buttonGO = new GameObject("CompareButton");
        buttonGO.transform.SetParent(panel.transform);
        RectTransform brt = buttonGO.AddComponent<RectTransform>();
        brt.anchoredPosition = new Vector2(0, 35);
        brt.sizeDelta = new Vector2(180, 30);

        Button compareBtn = buttonGO.AddComponent<Button>();
        Image btnImg = buttonGO.AddComponent<Image>();
        btnImg.color = new Color(0.3f, 0.6f, 0.3f, 0.9f);

        Text btnText = new GameObject("Text").AddComponent<Text>();
        btnText.transform.SetParent(buttonGO.transform);
        btnText.text = "Compare With Current";
        btnText.font = font;
        btnText.color = Color.white;
        btnText.alignment = TextAnchor.MiddleCenter;
        btnText.rectTransform.anchoredPosition = Vector2.zero;
        btnText.rectTransform.sizeDelta = brt.sizeDelta;

        // === Snapshot List Dropdown ===
        Dropdown snapshotDropdown = CreateDropdown(panel.transform, new Vector2(0, -10), new Vector2(300, 30));

        // === Assign to controller ===
        SnapshotTimelineUI timeline = panel.AddComponent<SnapshotTimelineUI>();
        timeline.dropdown = snapshotDropdown;
        timeline.compareButton = compareBtn;

        return panel;
    }

    private Dropdown CreateDropdown(Transform parent, Vector2 pos, Vector2 size)
    {
        GameObject go = new GameObject("SnapshotDropdown");
        go.transform.SetParent(parent);
        RectTransform rt = go.AddComponent<RectTransform>();
        rt.anchoredPosition = pos;
        rt.sizeDelta = size;

        Dropdown dd = go.AddComponent<Dropdown>();
        Image img = go.AddComponent<Image>();
        img.color = new Color(0.2f, 0.2f, 0.2f, 0.9f);

        Text label = new GameObject("Label").AddComponent<Text>();
        label.transform.SetParent(go.transform);
        label.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        label.text = "Select Snapshot";
        label.alignment = TextAnchor.MiddleCenter;
        label.color = Color.white;
        label.rectTransform.anchoredPosition = Vector2.zero;
        label.rectTransform.sizeDelta = size;

        dd.captionText = label;

        return dd;
    }
}
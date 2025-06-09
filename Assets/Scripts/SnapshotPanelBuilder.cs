using UnityEngine;
using UnityEngine.UI;

public class SnapshotPanelBuilder : MonoBehaviour
{
    public GameObject BuildPanel(Transform parent, SnapshotTimelineUI timelineUI)
    {
        GameObject panel = new GameObject("SnapshotPanel");
        panel.transform.SetParent(parent);
        RectTransform rt = panel.AddComponent<RectTransform>();
        rt.sizeDelta = new Vector2(360, 400);

        Image bg = panel.AddComponent<Image>();
        bg.color = new Color(0.12f, 0.12f, 0.12f, 0.9f);
        Font font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        // Timeline label
        Text label = CreateLabel(panel.transform, new Vector2(0, 160), font, "Snapshot Timeline");

        // Detail label
        Text details = CreateLabel(panel.transform, new Vector2(0, 100), font, "No snapshot selected.");
        timelineUI.detailLabel = details;

        // Prev / Next buttons
        Button prevBtn = CreateButton(panel.transform, new Vector2(-100, 60), font, "< Prev");
        Button nextBtn = CreateButton(panel.transform, new Vector2(100, 60), font, "Next >");
        timelineUI.prevButton = prevBtn;
        timelineUI.nextButton = nextBtn;

        // Scroll View placeholder (content goes here)
        GameObject scrollContent = new GameObject("ListParent");
        scrollContent.transform.SetParent(panel.transform);
        RectTransform scrollRT = scrollContent.AddComponent<RectTransform>();
        scrollRT.anchoredPosition = new Vector2(0, -40);
        scrollRT.sizeDelta = new Vector2(300, 200);
        timelineUI.listParent = scrollRT;

        return panel;
    }

    private Text CreateLabel(Transform parent, Vector2 pos, Font font, string content)
    {
        GameObject go = new GameObject("Label");
        go.transform.SetParent(parent);
        RectTransform rt = go.AddComponent<RectTransform>();
        rt.anchoredPosition = pos;
        rt.sizeDelta = new Vector2(300, 30);

        Text text = go.AddComponent<Text>();
        text.font = font;
        text.text = content;
        text.alignment = TextAnchor.MiddleCenter;
        text.color = Color.white;
        return text;
    }

    private Button CreateButton(Transform parent, Vector2 pos, Font font, string label)
    {
        GameObject go = new GameObject("Button");
        go.transform.SetParent(parent);
        RectTransform rt = go.AddComponent<RectTransform>();
        rt.anchoredPosition = pos;
        rt.sizeDelta = new Vector2(120, 30);

        Button btn = go.AddComponent<Button>();
        Text text = new GameObject("Text").AddComponent<Text>();
        text.transform.SetParent(go.transform);
        text.font = font;
        text.text = label;
        text.color = Color.white;
        text.alignment = TextAnchor.MiddleCenter;
        return btn;
    }
}
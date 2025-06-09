using UnityEngine;
using UnityEngine.UI;

public class QECPanelBuilder : MonoBehaviour
{
    public void BuildPanel(Transform parent, QECController qec)
    {
        Font font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        // === Take QEC Snapshot Button ===
        GameObject btnObj = new GameObject("TakeSnapshotButton");
        btnObj.transform.SetParent(parent);
        RectTransform rt = btnObj.AddComponent<RectTransform>();
        rt.anchoredPosition = new Vector2(0, -260);
        rt.sizeDelta = new Vector2(200, 40);

        Button button = btnObj.AddComponent<Button>();
        Image img = btnObj.AddComponent<Image>();
        img.color = new Color(0.2f, 0.8f, 0.3f, 0.9f);

        Text text = new GameObject("Text").AddComponent<Text>();
        text.transform.SetParent(btnObj.transform);
        text.font = font;
        text.text = "📸 Take QEC Snapshot";
        text.alignment = TextAnchor.MiddleCenter;
        text.color = Color.black;
        text.rectTransform.anchoredPosition = Vector2.zero;
        text.rectTransform.sizeDelta = rt.sizeDelta;

        button.onClick.AddListener(() => {
            string qt = GlobalSettings.SelectedQubitType.ToString();
            float fidelity = float.Parse(qec.fidelityLabel.text.Replace("Fidelity: ", ""));

            SnapshotUtility.TakeSnapshot(
                "QEC Lattice Snapshot",
                null,
                qt,
                new string[0],
                qt,
                fidelity,
                "Default",
                qec.GetActiveErrors(),
                qec.GetSyndromePositions(),
                new string[0],
                "N/A",
                "",
                null
            );
        });

        // === Toggle Syndromes Button ===
        GameObject toggleObj = new GameObject("ToggleSyndromesButton");
        toggleObj.transform.SetParent(parent);
        RectTransform trt = toggleObj.AddComponent<RectTransform>();
        trt.anchoredPosition = new Vector2(0, -310);
        trt.sizeDelta = new Vector2(200, 40);

        Button toggleButton = toggleObj.AddComponent<Button>();
        Image toggleImg = toggleObj.AddComponent<Image>();
        toggleImg.color = new Color(0.3f, 0.5f, 0.9f, 0.9f);

        Text toggleText = new GameObject("Text").AddComponent<Text>();
        toggleText.transform.SetParent(toggleObj.transform);
        toggleText.font = font;
        toggleText.text = "🧪 Toggle Syndromes";
        toggleText.alignment = TextAnchor.MiddleCenter;
        toggleText.color = Color.white;
        toggleText.rectTransform.anchoredPosition = Vector2.zero;
        toggleText.rectTransform.sizeDelta = trt.sizeDelta;

        toggleButton.onClick.AddListener(() => {
            bool hide = qec.ToggleSyndromesHidden();
            Debug.Log($"🔁 Syndrome visibility set to: {!hide}");
        });

        // === Export QEC Snapshot Button ===
        GameObject exportObj = new GameObject("ExportQECButton");
        exportObj.transform.SetParent(parent);

        RectTransform exportRt = exportObj.AddComponent<RectTransform>();
        exportRt.anchoredPosition = new Vector2(0, -360);
        exportRt.sizeDelta = new Vector2(200, 40);

        Button exportButton = exportObj.AddComponent<Button>();
        Image exportImg = exportObj.AddComponent<Image>();
        exportImg.color = new Color(0.85f, 0.5f, 0.2f, 0.9f);

        Text exportText = new GameObject("Text").AddComponent<Text>();
        exportText.transform.SetParent(exportObj.transform);
        exportText.font = font;
        exportText.text = "📤 Export QEC Snapshot";
        exportText.alignment = TextAnchor.MiddleCenter;
        exportText.color = Color.white;
        exportText.rectTransform.anchoredPosition = Vector2.zero;
        exportText.rectTransform.sizeDelta = exportRt.sizeDelta;

        exportButton.onClick.AddListener(() => {
            var snap = SnapshotManager.Instance.GetLatestSnapshot();
            if (snap != null)
            {
                QECExportUtility.ExportSnapshotToJson(snap);
                QECExportUtility.ExportSnapshotToTxt(snap);
            }
            else
            {
                Debug.LogWarning("⚠️ No snapshot available to export.");
            }
        });

        // === Replay Button ===
        GameObject replayObj = new GameObject("ReplayQECButton");
        replayObj.transform.SetParent(parent);
        RectTransform replayRt = replayObj.AddComponent<RectTransform>();
        replayRt.anchoredPosition = new Vector2(0, -410);
        replayRt.sizeDelta = new Vector2(200, 40);

        Button replayBtn = replayObj.AddComponent<Button>();
        Image replayImg = replayObj.AddComponent<Image>();
        replayImg.color = new Color(0.2f, 0.6f, 0.8f, 0.9f);

        Text replayText = new GameObject("Text").AddComponent<Text>();
        replayText.transform.SetParent(replayObj.transform);
        replayText.font = font;
        replayText.text = "🔁 Replay QEC Snapshot";
        replayText.alignment = TextAnchor.MiddleCenter;
        replayText.color = Color.white;
        replayText.rectTransform.anchoredPosition = Vector2.zero;
        replayText.rectTransform.sizeDelta = replayRt.sizeDelta;

        replayBtn.onClick.AddListener(() =>
        {
            Snapshot snap = SnapshotManager.Instance.GetLatestSnapshot();
            if (snap != null && FindObjectOfType<QECController>() is QECController qecCtrl)
                qecCtrl.LoadSnapshot(snap);
        });
    }
}

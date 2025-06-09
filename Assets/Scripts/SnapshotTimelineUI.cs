// SnapshotTimelineUI.cs (Fixed for Snapshot.cs v3 compatibility)
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class SnapshotTimelineUI : MonoBehaviour
{
    public Transform listParent;
    public GameObject snapshotEntryPrefab;

    public Text detailLabel;
    public Button prevButton;
    public Button nextButton;
    public Button compareButton;

    private int currentIndex = 0;
    private List<Snapshot> snapshotList;

    public Dropdown dropdown;

    void Start()
    {
        // snapshotList = SnapshotManager.Instance.GetAllSnapshots();

        // if (prevButton) prevButton.onClick.AddListener(() => Navigate(-1));
        // if (nextButton) nextButton.onClick.AddListener(() => Navigate(+1));
        // if (compareButton) compareButton.onClick.AddListener(CompareToCurrent);

        // RebuildList();
    }

    void RefreshUI()
    {
        if (snapshotList.Count == 0)
        {
            detailLabel.text = "No snapshots yet.";
            return;
        }

        currentIndex = Mathf.Clamp(currentIndex, 0, snapshotList.Count - 1);
        Snapshot snap = snapshotList[currentIndex];

        string source = snap.kernelMatrix != null ? "Kernel" :
                        (snap.activeErrorPositions != null && snap.activeErrorPositions.Length > 0) ? "QEC" :
                        (!string.IsNullOrEmpty(snap.zxRewritten)) ? "ZX" : "Unknown";

        detailLabel.text = $"Snapshot #{currentIndex + 1}\n" +
                           $"Experiment: {snap.experimentId}\n" +
                           $"Fidelity: {snap.fidelity:F2}\n" +
                           $"Qubits: {(snap.qubitLabels != null ? snap.qubitLabels.Length.ToString() : "N/A")}\n" +
                           $"Time: {snap.timestamp}\n" +
                           $"Source: {source}";

        HighlightActiveEntry();

        // Try update ZX viewer
        ZXSnapshotViewer.TryUpdateFromTimeline(snap);

        // Try load QEC state
        QECController qec = FindObjectOfType<QECController>();
        if (qec != null)
            qec.LoadSnapshot(snap);
    }

    void HighlightActiveEntry()
    {
        for (int i = 0; i < listParent.childCount; i++)
        {
            var img = listParent.GetChild(i).GetComponent<Image>();
            if (img != null)
                img.color = (i == currentIndex) ? Color.yellow : Color.gray;
        }
    }

    public void RebuildList()
    {
        foreach (Transform child in listParent)
            Destroy(child.gameObject);

        snapshotList = SnapshotManager.Instance.GetAllSnapshots();

        for (int i = 0; i < snapshotList.Count; i++)
        {
            int index = i;
            Snapshot snap = snapshotList[i];

            GameObject entry = Instantiate(snapshotEntryPrefab, listParent);
            entry.GetComponentInChildren<Text>().text = $"#{index + 1} | Snap {snap.experimentId}";
            entry.GetComponent<Button>().onClick.AddListener(() =>
            {
                currentIndex = index;
                RefreshUI();
            });
        }

        RefreshUI();
    }

    void Navigate(int direction)
    {
        currentIndex += direction;
        currentIndex = Mathf.Clamp(currentIndex, 0, snapshotList.Count - 1);
        RefreshUI();
    }

    void CompareToCurrent()
    {
        Snapshot snapshot = snapshotList[currentIndex];
        float[,] liveKernel = FindObjectOfType<KernelExplorer>().GetKernelMatrix();

        float[,] delta = KernelDeltaVisualizer.GetDeltaMatrix(liveKernel, snapshot.kernelMatrix);

        FindObjectOfType<KernelExplorer>().DisplayDeltaMatrix(delta);

        Debug.Log($"🔍 Compared snapshot {snapshot.experimentId} to current kernel.");

        // Optional: create comparison overlay panel
        GameObject builderObj = new GameObject("KernelComparisonBuilder");
        var builder = builderObj.AddComponent<KernelComparisonPanelBuilder>();

        GameObject panel = builder.BuildPanel(
            GameObject.Find("Canvas").transform,
            delta,
            () =>
            {
                FindObjectOfType<KernelExplorer>().RestoreOriginalKernel();
                Destroy(builderObj);
            }
        );
    }
}
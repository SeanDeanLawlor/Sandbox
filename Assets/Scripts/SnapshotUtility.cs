// SnapshotUtility.cs (Revised for Group 1 Fixes v4)
using UnityEngine;
using System.IO;

public static class SnapshotUtility
{
    // Updated signature to align with the revised Snapshot constructor in Snapshot.cs (v3)
    public static void TakeSnapshot(
        string circuit,             // circuit string
        float[,] kernelMatrix,      // kernel matrix
        string kernelLabel,         // kernel label
        string[] qubitLabels,       // qubit labels
        string qubitType,           // qubit type
        float fidelity,             // fidelity
        string colorMap,            // color map
        Vector2Int[] errorPositions,    // active error positions
        Vector2Int[] syndromePositions, // syndrome positions
        string[] zxRules,           // zx rewrite rules applied
        string zxRewrittenStr,      // zx diagram as string after rewrites
        string zxDiagramName = "",   // zx diagram name (optional)
        ZXDiagramViewer.ZXDiagram zxDiagram = null // zx diagram object (optional)
    )
    {
        string id = $"EXP-{System.Guid.NewGuid().ToString().Substring(0, 8)}";

        Snapshot snap = new Snapshot(
            id,
            circuit,
            kernelMatrix,
            kernelLabel,
            qubitLabels, 
            qubitType,
            fidelity,
            colorMap,
            errorPositions,
            syndromePositions,
            zxRules,
            zxRewrittenStr,
            zxDiagramName,
            zxDiagram
        );

        if (SnapshotManager.Instance != null)
        {
            SnapshotManager.Instance.AddSnapshot(snap);
        }
        else
        {
            Debug.LogError("SnapshotManager.Instance is null. Cannot save snapshot.");
        }

        Debug.Log($"📸 Snapshot saved: {snap.experimentId} - Circuit: {(string.IsNullOrEmpty(snap.circuit) ? "N/A" : snap.circuit)}, Kernel: {(string.IsNullOrEmpty(snap.kernelLabel) ? "N/A" : snap.kernelLabel)}");
    }

    public static void ExportAllToJson(string path)
    {
        var allSnapshots = SnapshotManager.Instance.GetAllSnapshots();
        if (allSnapshots == null || allSnapshots.Count == 0)
        {
            Debug.LogWarning("No snapshots to export.");
            return;
        }

        string json = JsonUtility.ToJson(new SnapshotListWrapper { snapshots = allSnapshots }, true);
        File.WriteAllText(path, json);
        Debug.Log($"📤 Exported {allSnapshots.Count} snapshots to {path}");
    }

    [System.Serializable]
    private class SnapshotListWrapper
    {
        public System.Collections.Generic.List<Snapshot> snapshots;
    }
}


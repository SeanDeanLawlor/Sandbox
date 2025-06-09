using System.IO;
using UnityEngine;
using System.Text;
using System;

public static class QECExportUtility
{
    public static void ExportSnapshotToJson(Snapshot snap)
    {
        string dir = Path.Combine(Application.dataPath, "../QECExports");
        if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

        string filename = $"{snap.experimentId}_qec.json";
        string path = Path.Combine(dir, filename);

        var exportData = new
        {
            experimentId = snap.experimentId,
            timestamp = snap.timestamp,
            qubitType = snap.qubitType,
            fidelity = snap.fidelity,
            errorPositions = snap.activeErrorPositions,
            syndromePositions = snap.syndromePositions
        };

        string json = JsonUtility.ToJson(exportData, true);
        File.WriteAllText(path, json);

        Debug.Log($"✅ Exported snapshot to: {path}");
    }

    public static void ExportSnapshotToTxt(Snapshot snap)
    {
        string dir = Path.Combine(Application.dataPath, "../QECExports");
        if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

        string filename = $"{snap.experimentId}_qec.txt";
        string path = Path.Combine(dir, filename);

        StringBuilder sb = new();
        sb.AppendLine($"Experiment ID: {snap.experimentId}");
        sb.AppendLine($"Timestamp: {snap.timestamp}");
        sb.AppendLine($"Qubit Type: {snap.qubitType}");
        sb.AppendLine($"Fidelity: {snap.fidelity:F2}");
        sb.AppendLine("Active Error Positions:");
        foreach (var pos in snap.activeErrorPositions)
            sb.AppendLine($"  - ({pos.x}, {pos.y})");
        sb.AppendLine("Syndrome Positions:");
        foreach (var pos in snap.syndromePositions)
            sb.AppendLine($"  - ({pos.x}, {pos.y})");

        File.WriteAllText(path, sb.ToString());
        Debug.Log($"✅ Exported snapshot to: {path}");
    }
}
using System.IO;
using UnityEngine;
using System.Text;
using System.Globalization;

public static class ZXExportUtility
{
    public static void ExportSnapshotToJson(Snapshot snap)
    {
        string dir = Path.Combine(Application.dataPath, "../ZXExports");
        if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

        string filename = $"{snap.experimentId}_zx.json";
        string path = Path.Combine(dir, filename);

        var exportData = new
        {
            experimentId = snap.experimentId,
            timestamp = snap.timestamp,
            qubitType = snap.qubitType,
            circuit = snap.circuit,
            rewrittenCircuit = snap.zxRewritten,
            zxRulesApplied = snap.zxRewriteRules
        };

        string json = JsonUtility.ToJson(exportData, true);
        File.WriteAllText(path, json);

        Debug.Log($"✅ Exported ZX snapshot to: {path}");
    }

    public static void ExportSnapshotToTxt(Snapshot snap)
    {
        string dir = Path.Combine(Application.dataPath, "../ZXExports");
        if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

        string filename = $"{snap.experimentId}_zx.txt";
        string path = Path.Combine(dir, filename);

        StringBuilder sb = new();
        sb.AppendLine($"Experiment ID: {snap.experimentId}");
        sb.AppendLine($"Timestamp: {snap.timestamp}");
        sb.AppendLine($"Qubit Type: {snap.qubitType}");
        sb.AppendLine($"Original Circuit: {snap.circuit}");
        sb.AppendLine($"Rewritten Circuit: {snap.zxRewritten}");
        sb.AppendLine("ZX Rules Applied:");
        foreach (string rule in snap.zxRewriteRules)
            sb.AppendLine($"  - {rule}");

        File.WriteAllText(path, sb.ToString());
        Debug.Log($"✅ Exported ZX snapshot to: {path}");
    }
}
// SnapshotExportUtility.cs
using UnityEngine;
using System.IO;
using System.Text;
using System.Collections.Generic;
using Newtonsoft.Json;

public static class SnapshotExportUtility
{
    private static string ExportPath => Path.Combine(Application.dataPath, "Exports");

    public static void ExportAllSnapshotsToJson()
    {
        EnsureExportFolder();
        List<Snapshot> all = SnapshotManager.Instance.GetAllSnapshots();
        string json = JsonHelper.ToJson(all.ToArray(), true);

        string path = Path.Combine(ExportPath, $"AllSnapshots_{System.DateTime.Now:yyyyMMdd_HHmmss}.json");
        File.WriteAllText(path, json);
        Debug.Log($"📦 Exported all snapshots to JSON: {path}");
    }

    public static void ExportAllSnapshotsToCSV()
    {
        EnsureExportFolder();
        List<Snapshot> all = SnapshotManager.Instance.GetAllSnapshots();
        StringBuilder csv = new StringBuilder();

        csv.AppendLine("ID,Fidelity,QubitType,Timestamp,RewriteRules");

        foreach (Snapshot snap in all)
        {
            string rules = (snap.zxRewriteRules != null) ? string.Join("|", snap.zxRewriteRules) : "";
            csv.AppendLine($"{snap.experimentId},{snap.fidelity:F2},{snap.qubitType},{snap.timestamp},{rules}");
        }

        string path = Path.Combine(ExportPath, $"AllSnapshots_{System.DateTime.Now:yyyyMMdd_HHmmss}.csv");
        File.WriteAllText(path, csv.ToString());
        Debug.Log($"📄 Exported all snapshots to CSV: {path}");
    }

    public static void ExportAllToJson(string filePath)
    {
        if (SnapshotManager.Instance == null)
        {
            Debug.LogError("❌ SnapshotManager is null, cannot export.");
            return;
        }

        List<Snapshot> allSnapshots = SnapshotManager.Instance.GetAllSnapshots();
        if (allSnapshots == null || allSnapshots.Count == 0)
        {
            Debug.LogWarning("⚠️ No snapshots available to export.");
            return;
        }

        try
        {
            string json = JsonConvert.SerializeObject(allSnapshots, Formatting.Indented);
            string directory = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            File.WriteAllText(filePath, json);
            Debug.Log($"✅ Exported {allSnapshots.Count} snapshots to {filePath}");
        }
        catch (System.Exception ex)
        {
            Debug.LogError("❌ Failed to export snapshots: " + ex.Message);
        }
    }

    private static void EnsureExportFolder()
    {
        if (!Directory.Exists(ExportPath))
            Directory.CreateDirectory(ExportPath);
    }
}

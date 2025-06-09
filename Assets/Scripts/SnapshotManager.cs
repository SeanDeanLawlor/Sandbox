// SnapshotManager.cs (Revised for Group 1 Fixes v3)
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System;

public class SnapshotManager : MonoBehaviour
{
    public static SnapshotManager Instance;
    public List<Snapshot> snapshots = new List<Snapshot>();
    private int counter = 0; // Used for generating unique IDs if needed

    public event Action OnSnapshotTaken;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // This legacy method uses object initializers. Ensure Snapshot.cs has a parameterless constructor
    // and that all field names used here (experimentId, kernelMatrix, etc.) are public in Snapshot.cs.
    public void TakeSnapshotLegacy(float[,] kernelMatrixData, string legacyKernelLabel, float legacyFidelity, string[] legacyQubitLabels, string legacyColorMap, Vector2Int[] legacyQecErrors, Vector2Int[] legacyQecSyndromes)
    {
        var snap = new Snapshot // Uses parameterless constructor + object initializer
        {
            experimentId = "SNAP_MGR_LEGACY_" + counter++, 
            timestamp = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), // Ensure consistent timestamp format
            kernelMatrix = kernelMatrixData != null ? (float[,])kernelMatrixData.Clone() : null,
            kernelLabel = legacyKernelLabel,
            fidelity = legacyFidelity,
            qubitLabels = legacyQubitLabels != null ? (string[])legacyQubitLabels.Clone() : new string[0],
            colorMap = legacyColorMap,
            activeErrorPositions = legacyQecErrors ?? new Vector2Int[0],
            syndromePositions = legacyQecSyndromes ?? new Vector2Int[0]
            // Other fields from Snapshot.cs will use their default values from the parameterless constructor
        };
        AddSnapshot(snap);
    }

    // Method to be called by SnapshotUtility.cs and other parts of the system
    public void AddSnapshot(Snapshot snap)
    {
        if (snap == null) 
        {
            Debug.LogError("Attempted to add a null snapshot.");
            return;
        }
        if (snapshots == null) snapshots = new List<Snapshot>();
        snapshots.Add(snap);
        Debug.Log("🧠 Snapshot added: ID " + snap.experimentId + ", Label: " + (snap.kernelLabel ?? snap.circuit ?? "N/A"));
        NotifySnapshotTaken(); // Notify listeners
    }

    public List<Snapshot> GetAllSnapshots()
    {
        return snapshots;
    }

    public void NotifySnapshotTaken()
    {
        OnSnapshotTaken?.Invoke();
    }

    public void ExportSnapshots(string path)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(path));
        using StreamWriter sw = new StreamWriter(path);
        foreach (var snap in snapshots)
        {
            // Ensure field names match Snapshot.cs (experimentId, kernelLabel, circuit, fidelity, timestamp)
            sw.WriteLine($"[{snap.timestamp}] {(snap.kernelLabel ?? snap.circuit ?? snap.experimentId)} | Fidelity: {snap.fidelity:F3}");
        }
        Debug.Log("✅ Snapshots exported to: " + path);
    }

    public Snapshot GetLatestSnapshot()
    {
        return snapshots.Count > 0 ? snapshots[snapshots.Count - 1] : null;
    }

    public void ClearAllSnapshots()
    {
        snapshots.Clear();
        counter = 0; 
    }

    public void BundleZXDiagram(string name, ZXDiagramViewer.ZXDiagram diagram, List<string> rewriteSteps)
    {
        var last = GetLatestSnapshot();
        if (last != null)
        {
            last.zxDiagram = diagram;
            last.zxDiagramName = name;
            // Ensure field name matches Snapshot.cs (zxRewriteRules) and type conversion is correct
            last.zxRewriteRules = rewriteSteps != null ? rewriteSteps.ToArray() : new string[0]; 
            Debug.Log($"📎 ZX Diagram \'{name}\' bundled into snapshot ID {last.experimentId}");
        }
    }

    public void BundleKernelMetadata(string label, float fidelityParam)
    {
        var last = GetLatestSnapshot();
        if (last != null)
        {
            last.kernelLabel = label;
            last.fidelity = fidelityParam;
            Debug.Log($"🧬 Bundled kernel: {label} | Fidelity: {last.fidelity:F3}");
        }
    }

    public void BundleRewriteRules(string[] rules) // Parameter is string[]
    {
        var last = GetLatestSnapshot();
        if (last != null)
        {
            // Ensure field name matches Snapshot.cs (zxRewriteRules) and type is string[]
            last.zxRewriteRules = rules ?? new string[0]; 
            Debug.Log("📜 Rewrites bundled: " + (rules != null ? string.Join(", ", rules) : "null"));
        }
    }

    public void LogQECErrorPositions(Vector2Int[] errorCoords)
    {
        var last = GetLatestSnapshot();
        if (last != null)
        {
            // Ensure field name matches Snapshot.cs (activeErrorPositions)
            last.activeErrorPositions = errorCoords ?? new Vector2Int[0]; 
            Debug.Log($"🪲 QEC error positions updated: {(errorCoords != null ? errorCoords.Length : 0)} errors");
        }
    }
}




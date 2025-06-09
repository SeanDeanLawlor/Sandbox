// SnapshotEntry.cs (used by Timeline list)
using UnityEngine;
using UnityEngine.UI;

public class SnapshotEntry : MonoBehaviour
{
    public Text idLabel;
    public Text typeLabel;
    public Text timeLabel;

    private Snapshot data;

    public void PopulateFromSnapshot(Snapshot snap)
    {
        data = snap;

        if (idLabel != null)
            idLabel.text = snap.experimentId;

        if (typeLabel != null)
            typeLabel.text = $"{snap.qubitType}  •  Fidelity: {snap.fidelity:F2}";

        if (timeLabel != null)
            timeLabel.text = snap.timestamp;
    }

    public Snapshot GetSnapshot() => data;
} 

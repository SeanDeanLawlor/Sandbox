// ZXSnapshotViewer.cs (full version with diagram + circuit + rules)
using UnityEngine;
using UnityEngine.UI;

public class ZXSnapshotViewer : MonoBehaviour
{
    public Text originalCircuitLabel;
    public Text rewrittenCircuitLabel;
    public Text rulesLabel;
    public Text diagramNameLabel;

    public ZXDiagramViewer viewer;

    public void DisplaySnapshot(Snapshot snap)
    {
        if (snap == null)
        {
            originalCircuitLabel.text = "⚠️ No snapshot selected.";
            rewrittenCircuitLabel.text = "";
            rulesLabel.text = "";
            diagramNameLabel.text = "";
            viewer.ClearDiagram();
            return;
        }

        // Circuit info
        originalCircuitLabel.text = $"🧮 Original Circuit:\n{snap.circuit}";
        rewrittenCircuitLabel.text = $"🔁 Rewritten Circuit:\n{snap.zxRewritten}";

        // Rewrite rules
        if (snap.zxRewriteRules != null && snap.zxRewriteRules.Length > 0)
            rulesLabel.text = $"📜 ZX Rules:\n- " + string.Join("\n- ", snap.zxRewriteRules);
        else
            rulesLabel.text = "📜 ZX Rules:\nNone recorded.";

        // ZX Diagram (if any)
        if (snap.zxDiagram != null)
        {
            diagramNameLabel.text = $"📎 Diagram: {snap.zxDiagramName}";
            viewer.DisplayDiagram(snap.zxDiagram);
        }
        else
        {
            diagramNameLabel.text = "📎 No ZX diagram";
            viewer.ClearDiagram();
        }
    }

    public static void TryUpdateFromTimeline(Snapshot snap)
    {
        ZXSnapshotViewer viewerInstance = FindObjectOfType<ZXSnapshotViewer>();
        if (viewerInstance != null)
        {
            viewerInstance.DisplaySnapshot(snap);
        }
    }
}
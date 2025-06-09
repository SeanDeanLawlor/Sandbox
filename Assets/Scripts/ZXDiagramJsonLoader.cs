// ZXDiagramJsonLoader.cs (with export + toggles)
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Collections.Generic;

public class ZXDiagramJsonLoader : MonoBehaviour
{
    public Text statusLabel;
    public ZXDiagramViewer viewer;

    public Toggle autoExportToggle;
    public Toggle snapshotBundleToggle;

    [System.Serializable] public class JsonZXNode { public float[] position; public string type; }
    [System.Serializable] public class JsonZXEdge { public int from; public int to; public bool hadamard; }
    [System.Serializable] public class JsonZXDiagram { public JsonZXNode[] nodes; public JsonZXEdge[] edges; }



    public void LoadFromPath(string path)
    {
        if (!File.Exists(path))
        {
            if (statusLabel) statusLabel.text = "❌ File not found.";
            return;
        }

        string json = File.ReadAllText(path);
        JsonZXDiagram parsed = JsonUtility.FromJson<JsonZXDiagram>(json);

        ZXDiagramViewer.ZXDiagram diagram = new ZXDiagramViewer.ZXDiagram();

        foreach (var node in parsed.nodes)
        {
            diagram.nodes.Add(new ZXDiagramViewer.ZXNode {
                position = new Vector2(node.position[0], node.position[1]),
                type = ParseNodeType(node.type)
            });
        }

        foreach (var edge in parsed.edges)
        {
            diagram.edges.Add(new ZXDiagramViewer.ZXEdge {
                fromIndex = edge.from,
                toIndex = edge.to,
                isHadamard = edge.hadamard
            });
        }

        viewer.DisplayDiagram(diagram);
        viewer.SetDirty(); // trigger auto-export

        if (autoExportToggle != null && autoExportToggle.isOn)
        {
            ExportDiagramToJson(diagram, "Exports/AutoExport_ZX.json");
            ExportDiagramToText(diagram, "Exports/AutoExport_ZX.txt");
        }

        if (snapshotBundleToggle != null && snapshotBundleToggle.isOn)
        {
            string label = "Loaded_JSON_" + Path.GetFileNameWithoutExtension(path);
            var zxRules = new List<string> { "Loaded JSON" };
            SnapshotManager.Instance.BundleZXDiagram(label, diagram, zxRules);
        }

        if (statusLabel) statusLabel.text = "✅ ZX Diagram loaded.";
    }

    ZXDiagramViewer.ZXNodeType ParseNodeType(string raw) => raw.ToLower() switch
    {
        "x" => ZXDiagramViewer.ZXNodeType.X,
        "h" => ZXDiagramViewer.ZXNodeType.H,
        "input" => ZXDiagramViewer.ZXNodeType.Input,
        "output" => ZXDiagramViewer.ZXNodeType.Output,
        _ => ZXDiagramViewer.ZXNodeType.Z
    };

    public void ExportCurrentDiagram()
    {
        if (viewer == null) return;
        ExportDiagramToJson(viewer.GetCurrentDiagram(), "Exports/ExportedZXDiagram.json");
        ExportDiagramToText(viewer.GetCurrentDiagram(), "Exports/ExportedZXDiagram.txt");
        if (statusLabel) statusLabel.text = "💾 Diagram exported.";
    }

    public void ExportDiagramToJson(ZXDiagramViewer.ZXDiagram diagram, string path)
    {
        JsonZXDiagram json = new JsonZXDiagram
        {
            nodes = diagram.nodes.ConvertAll(n => new JsonZXNode {
                position = new float[] { n.position.x, n.position.y },
                type = n.type.ToString()
            }).ToArray(),
            edges = diagram.edges.ConvertAll(e => new JsonZXEdge {
                from = e.fromIndex, to = e.toIndex, hadamard = e.isHadamard
            }).ToArray()
        };

        Directory.CreateDirectory("Exports");
        File.WriteAllText(path, JsonUtility.ToJson(json, true));
    }

    public void ExportDiagramToText(ZXDiagramViewer.ZXDiagram diagram, string path)
    {
        Directory.CreateDirectory("Exports");
        using StreamWriter sw = new StreamWriter(path);
        sw.WriteLine("# ZX Diagram Export (TXT)");
        foreach (var n in diagram.nodes)
            sw.WriteLine($"Node {n.type} at ({n.position.x}, {n.position.y})");
        foreach (var e in diagram.edges)
            sw.WriteLine($"Edge {e.fromIndex}->{e.toIndex} {(e.isHadamard ? "[H]" : "")}");
    }


    public void OpenFileDialogAndLoad()
    {
        string path = MacFilePicker.OpenFile();
        if (string.IsNullOrEmpty(path))
        {
            if (statusLabel) statusLabel.text = "❌ File load canceled.";
            return;
        }

        LoadFromPath(path);
    }

}

// ZXDiagramExporter.cs
using UnityEngine;
using System.IO;
using System;

public class ZXDiagramExporter : MonoBehaviour
{
    public ZXDiagramViewer viewer;

    public void ExportCurrentDiagram()
    {
        if (viewer == null || viewer.CurrentDiagram == null)
        {
            Debug.LogWarning("⚠️ No diagram available to export.");
            return;
        }

        string jsonPath = Path.Combine(Application.dataPath, "Exports");
        Directory.CreateDirectory(jsonPath);

        string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        string baseName = Path.Combine(jsonPath, $"ZXDiagram_{timestamp}");

        string jsonFile = baseName + ".json";
        string txtFile = baseName + ".txt";

        // Serialize to JSON
        string json = JsonUtility.ToJson(ConvertToSerializable(viewer.CurrentDiagram), true);
        File.WriteAllText(jsonFile, json);

        // Serialize to TXT
        string txt = GenerateTxt(viewer.CurrentDiagram);
        File.WriteAllText(txtFile, txt);

        Debug.Log($"✅ ZX Diagram exported to {jsonFile} and {txtFile}");
    }

    ZXDiagramJsonLoader.JsonZXDiagram ConvertToSerializable(ZXDiagramViewer.ZXDiagram diagram)
    {
        var jnodes = new ZXDiagramJsonLoader.JsonZXNode[diagram.nodes.Count];
        for (int i = 0; i < diagram.nodes.Count; i++)
        {
            jnodes[i] = new ZXDiagramJsonLoader.JsonZXNode
            {
                type = diagram.nodes[i].type.ToString(),
                position = new float[] { diagram.nodes[i].position.x, diagram.nodes[i].position.y }
            };
        }

        var jedges = new ZXDiagramJsonLoader.JsonZXEdge[diagram.edges.Count];
        for (int i = 0; i < diagram.edges.Count; i++)
        {
            jedges[i] = new ZXDiagramJsonLoader.JsonZXEdge
            {
                from = diagram.edges[i].fromIndex,
                to = diagram.edges[i].toIndex,
                hadamard = diagram.edges[i].isHadamard
            };
        }

        return new ZXDiagramJsonLoader.JsonZXDiagram
        {
            nodes = jnodes,
            edges = jedges
        };
    }

    string GenerateTxt(ZXDiagramViewer.ZXDiagram diagram)
    {
        System.Text.StringBuilder sb = new();
        sb.AppendLine("# ZX Diagram Export\n");

        sb.AppendLine("Nodes:");
        for (int i = 0; i < diagram.nodes.Count; i++)
        {
            var n = diagram.nodes[i];
            sb.AppendLine($"[{i}] {n.type} at ({n.position.x}, {n.position.y})");
        }

        sb.AppendLine("\nEdges:");
        foreach (var e in diagram.edges)
        {
            sb.AppendLine($"{e.fromIndex} -> {e.toIndex} {(e.isHadamard ? "[H]" : "")}");
        }

        return sb.ToString();
    }
}

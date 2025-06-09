// ZXDiagramManager.cs
using UnityEngine;
using System.Collections.Generic;
using System.IO;

public static class ZXDiagramManager
{
    private static string basePath = "Exports/Diagrams/";

    public static void SaveDiagram(ZXDiagramViewer.ZXDiagram diagram, string name)
    {
        Directory.CreateDirectory(basePath);

        string jsonPath = Path.Combine(basePath, name + ".json");
        string txtPath = Path.Combine(basePath, name + ".txt");

        var json = new ZXDiagramJsonLoader.JsonZXDiagram
        {
            nodes = diagram.nodes.ConvertAll(n => new ZXDiagramJsonLoader.JsonZXNode {
                position = new float[] { n.position.x, n.position.y },
                type = n.type.ToString()
            }).ToArray(),
            edges = diagram.edges.ConvertAll(e => new ZXDiagramJsonLoader.JsonZXEdge {
                from = e.fromIndex,
                to = e.toIndex,
                hadamard = e.isHadamard
            }).ToArray()
        };

        File.WriteAllText(jsonPath, JsonUtility.ToJson(json, true));

        using StreamWriter sw = new StreamWriter(txtPath);
        sw.WriteLine("# ZX Diagram Export: " + name);
        foreach (var n in diagram.nodes)
            sw.WriteLine($"Node {n.type} at ({n.position.x}, {n.position.y})");
        foreach (var e in diagram.edges)
            sw.WriteLine($"Edge {e.fromIndex}->{e.toIndex} {(e.isHadamard ? "[H]" : "")}");
    }

    public static List<string> ListSavedDiagrams()
    {
        Directory.CreateDirectory(basePath);
        var files = Directory.GetFiles(basePath, "*.json");
        List<string> names = new();
        foreach (var f in files)
            names.Add(Path.GetFileNameWithoutExtension(f));
        return names;
    }

    public static ZXDiagramViewer.ZXDiagram LoadDiagramByName(string name)
    {
        string path = Path.Combine(basePath, name + ".json");
        if (!File.Exists(path)) return null;

        string json = File.ReadAllText(path);
        var parsed = JsonUtility.FromJson<ZXDiagramJsonLoader.JsonZXDiagram>(json);
        var diagram = new ZXDiagramViewer.ZXDiagram();

        foreach (var node in parsed.nodes)
            diagram.nodes.Add(new ZXDiagramViewer.ZXNode {
                position = new Vector2(node.position[0], node.position[1]),
                type = ParseNodeType(node.type)
            });

        foreach (var edge in parsed.edges)
            diagram.edges.Add(new ZXDiagramViewer.ZXEdge {
                fromIndex = edge.from,
                toIndex = edge.to,
                isHadamard = edge.hadamard
            });

        return diagram;
    }

    private static ZXDiagramViewer.ZXNodeType ParseNodeType(string raw)
    {
        return raw.ToLower() switch
        {
            "x" => ZXDiagramViewer.ZXNodeType.X,
            "h" => ZXDiagramViewer.ZXNodeType.H,
            "input" => ZXDiagramViewer.ZXNodeType.Input,
            "output" => ZXDiagramViewer.ZXNodeType.Output,
            _ => ZXDiagramViewer.ZXNodeType.Z
        };
    }
}

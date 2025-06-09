// ZXDiagramViewer.cs (final full version with 3D toggle + export + redraw)
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ZXDiagramViewer : MonoBehaviour
{
    public GameObject nodePrefab;
    public GameObject edgePrefab;
    public Transform diagramParent;
    public Toggle use3DViewToggle; // 3D toggle from panel

    private List<GameObject> nodeObjects = new();
    private List<GameObject> edgeObjects = new();

    public ZXDiagram CurrentDiagram => GetCurrentDiagram();

    public enum ZXNodeType { Z, X, H, Input, Output }

    [System.Serializable]
    public class ZXNode
    {
        public Vector2 position;
        public ZXNodeType type;
    }

    [System.Serializable]
    public class ZXEdge
    {
        public int fromIndex;
        public int toIndex;
        public bool isHadamard;
    }

    [System.Serializable]
    public class ZXDiagram
    {
        public List<ZXNode> nodes = new();
        public List<ZXEdge> edges = new();
    }

    public void DisplayDiagram(ZXDiagram diagram)
    {
        ClearDiagram();

        for (int i = 0; i < diagram.nodes.Count; i++)
        {
            var node = diagram.nodes[i];
            GameObject obj = Instantiate(nodePrefab, diagramParent);

            Vector3 pos = node.position;
            if (use3DViewToggle != null && use3DViewToggle.isOn)
                pos.z = -i * 10f;

            obj.GetComponent<RectTransform>().anchoredPosition3D = pos;

            Image img = obj.GetComponent<Image>();
            img.color = node.type switch
            {
                ZXNodeType.Z => Color.green,
                ZXNodeType.X => Color.red,
                ZXNodeType.H => Color.white,
                ZXNodeType.Input => new Color(0.2f, 0.6f, 1f),
                ZXNodeType.Output => new Color(1f, 0.6f, 0.2f),
                _ => Color.gray
            };

            nodeObjects.Add(obj);
        }

        foreach (var edge in diagram.edges)
        {
            GameObject obj = Instantiate(edgePrefab, diagramParent);
            var line = obj.GetComponent<UILineRenderer>();
            Vector2 from = diagram.nodes[edge.fromIndex].position;
            Vector2 to = diagram.nodes[edge.toIndex].position;

            line.Points = new[] { from, to };
            line.LineColor = edge.isHadamard ? Color.cyan : Color.black;
            line.LineThickness = edge.isHadamard ? 2f : 3.5f;
            line.Dashed = edge.isHadamard;

            edgeObjects.Add(obj);
        }
    }

    public void ClearDiagram()
    {
        foreach (var obj in nodeObjects) Destroy(obj);
        foreach (var obj in edgeObjects) Destroy(obj);
        nodeObjects.Clear();
        edgeObjects.Clear();
    }

    public void SetDirty()
    {
        var loader = FindObjectOfType<ZXDiagramJsonLoader>();
        if (loader != null && loader.autoExportToggle != null && loader.autoExportToggle.isOn)
        {
            var diagram = GetCurrentDiagram();
            loader.ExportDiagramToJson(diagram, "Exports/AutoExport_ZX.json");
            loader.ExportDiagramToText(diagram, "Exports/AutoExport_ZX.txt");
            if (loader.statusLabel) loader.statusLabel.text = "💾 Auto-exported on change.";
        }
    }

    public void Redraw()
    {
        var loader = FindObjectOfType<ZXDiagramJsonLoader>();
        if (loader != null)
        {
            var diagram = GetCurrentDiagram();
            if (diagram != null)
                DisplayDiagram(diagram);
        }
    }

    public ZXDiagram GetCurrentDiagram()
    {
        var loader = FindObjectOfType<ZXDiagramJsonLoader>();
        if (loader != null && loader.viewer == this)
        {
            // Safely construct the diagram from current state if needed
            return loader.viewer.GetCurrentDiagramInternal();
        }
        return null;
    }

    // Optional internal method if you want GetCurrentDiagram to reflect the diagram as drawn
    public ZXDiagram GetCurrentDiagramInternal()
    {
        ZXDiagram diagram = new();

        for (int i = 0; i < nodeObjects.Count; i++)
        {
            var obj = nodeObjects[i];
            var rt = obj.GetComponent<RectTransform>();
            Vector2 pos2D = new(rt.anchoredPosition3D.x, rt.anchoredPosition3D.y);
            Color color = obj.GetComponent<Image>().color;

            var type = ZXNodeType.Z;
            if (color == Color.red) type = ZXNodeType.X;
            else if (color == Color.white) type = ZXNodeType.H;
            else if (color == new Color(0.2f, 0.6f, 1f)) type = ZXNodeType.Input;
            else if (color == new Color(1f, 0.6f, 0.2f)) type = ZXNodeType.Output;

            diagram.nodes.Add(new ZXNode { position = pos2D, type = type });
        }

        for (int i = 0; i < edgeObjects.Count; i++)
        {
            var line = edgeObjects[i].GetComponent<UILineRenderer>();
            if (line != null && line.Points.Length >= 2)
            {
                diagram.edges.Add(new ZXEdge
                {
                    fromIndex = i, // could be enhanced to resolve actual node indices
                    toIndex = i + 1,
                    isHadamard = line.Dashed
                });
            }
        }

        return diagram;
    }
    public ZXDiagram GetDiagram()
    {
        return GetCurrentDiagram();
    }
}
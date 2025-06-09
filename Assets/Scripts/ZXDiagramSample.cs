// ZXDiagramSample.cs – Generates a sample visual ZX diagram using nodes and UI lines
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ZXDiagramSample : MonoBehaviour
{
    public RectTransform diagramParent;
    public GameObject nodePrefab;
    public Color zColor = new Color(0.2f, 0.8f, 0.2f);
    public Color xColor = new Color(0.2f, 0.4f, 1f);
    public Color hColor = new Color(1f, 1f, 0.2f);

    private List<GameObject> nodes = new();
    private List<UILineRenderer> edges = new();

    void Start()
    {
        CreateNode("Z", new Vector2(-100, 0), zColor);
        CreateNode("X", new Vector2(0, 0), xColor);
        CreateNode("H", new Vector2(100, 0), hColor);

        ConnectNodes(0, 1);
        ConnectNodes(1, 2);
    }

    GameObject CreateNode(string label, Vector2 position, Color color)
    {
        GameObject node = Instantiate(nodePrefab, diagramParent);
        node.GetComponent<RectTransform>().anchoredPosition = position;
        node.GetComponent<Image>().color = color;
        node.GetComponentInChildren<Text>().text = label;

        nodes.Add(node);
        return node;
    }

    void ConnectNodes(int fromIndex, int toIndex)
    {
        GameObject lineObj = new GameObject($"Edge_{fromIndex}_{toIndex}", typeof(RectTransform), typeof(UILineRenderer));
        lineObj.transform.SetParent(diagramParent, false);

        UILineRenderer line = lineObj.GetComponent<UILineRenderer>();
        line.color = Color.white;
        line.lineWidth = 3f;

        Vector2 fromPos = nodes[fromIndex].GetComponent<RectTransform>().anchoredPosition;
        Vector2 toPos = nodes[toIndex].GetComponent<RectTransform>().anchoredPosition;

        line.AddLine(fromPos, toPos);
        edges.Add(line);
    }
}
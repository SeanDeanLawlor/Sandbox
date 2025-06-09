// UILineRenderer.cs (fully compatible with ZXDiagramViewer.cs)
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

[RequireComponent(typeof(CanvasRenderer))]
public class UILineRenderer : Graphic
{
    private List<Vector2> dynamicPoints = new List<Vector2>();
    public Vector2[] Points
    {
        get => dynamicPoints.ToArray();
        set
        {
            dynamicPoints = new List<Vector2>(value);
            SetVerticesDirty();
        }
    }

    [SerializeField] private float _lineWidth = 2f;
    public float lineWidth { get => _lineWidth; set { _lineWidth = value; SetVerticesDirty(); } }    public float LineThickness
    {
        get => lineWidth;
        set
        {
            lineWidth = value;
            SetVerticesDirty();
        }
    }

    private Color lineColor = Color.white;
    public Color LineColor
    {
        get => lineColor;
        set
        {
            lineColor = value;
            SetVerticesDirty();
        }
    }

    public bool Dashed = false;
    public float DashSize = 8f;

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();
        if (dynamicPoints == null || dynamicPoints.Count < 2)
            return;

        for (int i = 0; i < dynamicPoints.Count; i += 2)
        {
            Vector2 start = dynamicPoints[i];
            Vector2 end = dynamicPoints[i + 1];

            if (Dashed)
                DrawDashedLine(vh, start, end);
            else
                DrawLineSegment(vh, start, end);
        }
    }

    void DrawLineSegment(VertexHelper vh, Vector2 start, Vector2 end)
    {
        Vector2 dir = (end - start).normalized;
        Vector2 normal = new Vector2(-dir.y, dir.x) * lineWidth * 0.5f;

        UIVertex[] quad = new UIVertex[4];
        quad[0] = CreateVertex(start - normal);
        quad[1] = CreateVertex(start + normal);
        quad[2] = CreateVertex(end + normal);
        quad[3] = CreateVertex(end - normal);

        int startIndex = vh.currentVertCount;
        foreach (var v in quad)
            vh.AddVert(v);

        vh.AddTriangle(startIndex, startIndex + 1, startIndex + 2);
        vh.AddTriangle(startIndex, startIndex + 2, startIndex + 3);
    }

    void DrawDashedLine(VertexHelper vh, Vector2 start, Vector2 end)
    {
        float length = Vector2.Distance(start, end);
        int segments = Mathf.FloorToInt(length / (DashSize * 2));
        Vector2 dir = (end - start).normalized;

        for (int i = 0; i < segments; i++)
        {
            Vector2 segStart = start + dir * i * DashSize * 2;
            Vector2 segEnd = segStart + dir * DashSize;
            DrawLineSegment(vh, segStart, segEnd);
        }
    }

    UIVertex CreateVertex(Vector2 pos)
    {
        return new UIVertex
        {
            position = pos,
            color = lineColor
        };
    }

    public void SetDirty() => SetVerticesDirty();

    public void AddLine(Vector2 a, Vector2 b)
    {
        dynamicPoints.Add(a);
        dynamicPoints.Add(b);
        SetVerticesDirty();
    }

    public void ClearLines()
    {
        dynamicPoints.Clear();
        SetVerticesDirty();
    }
}
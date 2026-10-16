using UnityEngine;
using UnityEngine.UI;

// Resolution-independent terminal shapes; no font glyphs or bitmap textures required.
public class EncyclopediaTerminalGraphic : MaskableGraphic
{
    public enum Symbol { Panel, Chevron, Diamond }

    [SerializeField] Symbol symbol;
    [SerializeField] Color outlineColor = new Color(0.12f, 0.27f, 0.29f);
    [SerializeField] float thickness = 1f;
    [SerializeField] float cornerCut = 9f;

    public Symbol Shape
    {
        set
        {
            symbol = value;
            SetVerticesDirty();
        }
    }

    public Color OutlineColor
    {
        set
        {
            outlineColor = value;
            SetVerticesDirty();
        }
    }

    protected override void OnPopulateMesh(VertexHelper mesh)
    {
        mesh.Clear();
        Rect rect = rectTransform.rect;
        if (symbol == Symbol.Chevron)
        {
            Vector2 a = new Vector2(rect.xMin + rect.width * 0.3f, rect.yMin + rect.height * 0.15f);
            Vector2 b = new Vector2(rect.xMin + rect.width * 0.7f, rect.center.y);
            Vector2 c = new Vector2(a.x, rect.yMax - rect.height * 0.15f);
            Line(mesh, a, b, color, thickness);
            Line(mesh, b, c, color, thickness);
            return;
        }
        if (symbol == Symbol.Diamond)
        {
            Vector2 center = rect.center;
            Vector2[] diamond = { new Vector2(center.x, rect.yMax), new Vector2(rect.xMax, center.y),
                new Vector2(center.x, rect.yMin), new Vector2(rect.xMin, center.y) };
            for (int i = 0; i < diamond.Length; i++)
            {
                Line(mesh, diamond[i], diamond[(i + 1) % diamond.Length], color, thickness);
            }
            return;
        }

        float cut = Mathf.Min(cornerCut, Mathf.Min(rect.width, rect.height) * 0.3f);
        Vector2[] points = { new Vector2(rect.xMin, rect.yMin + cut), new Vector2(rect.xMin, rect.yMax),
            new Vector2(rect.xMax - cut, rect.yMax), new Vector2(rect.xMax, rect.yMax - cut),
            new Vector2(rect.xMax, rect.yMin), new Vector2(rect.xMin + cut, rect.yMin) };
        AddVertex(mesh, rect.center, color);
        foreach (Vector2 point in points)
        {
            AddVertex(mesh, point, color);
        }
        for (int i = 0; i < points.Length; i++)
        {
            mesh.AddTriangle(0, i + 1, (i + 1) % points.Length + 1);
            Line(mesh, points[i], points[(i + 1) % points.Length], outlineColor, thickness);
        }
    }

    static void Line(VertexHelper mesh, Vector2 a, Vector2 b, Color tint, float width)
    {
        Vector2 direction = (b - a).normalized;
        Vector2 normal = new Vector2(-direction.y, direction.x) * width * 0.5f;
        int first = mesh.currentVertCount;
        AddVertex(mesh, a - normal, tint);
        AddVertex(mesh, a + normal, tint);
        AddVertex(mesh, b + normal, tint);
        AddVertex(mesh, b - normal, tint);
        mesh.AddTriangle(first, first + 1, first + 2);
        mesh.AddTriangle(first, first + 2, first + 3);
    }

    static void AddVertex(VertexHelper mesh, Vector2 position, Color tint)
    {
        UIVertex vertex = UIVertex.simpleVert;
        vertex.position = position;
        vertex.color = tint;
        vertex.uv0 = new Vector4(0.5f, 0.5f, 0f, 0f);
        mesh.AddVert(vertex);
    }
}

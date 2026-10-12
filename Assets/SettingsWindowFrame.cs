using UnityEngine;
using UnityEngine.UI;

// Draw the orange Image as four strips, leaving the content area transparent.
// This avoids multiplying the orange and green panels' transparency.
public class SettingsWindowFrame : BaseMeshEffect
{
    [SerializeField] RectTransform contentArea;
    readonly Vector3[] corners = new Vector3[4];

    public override void ModifyMesh(VertexHelper mesh)
    {
        if (!IsActive() || contentArea == null || mesh.currentVertCount == 0)
            return;

        UIVertex source = UIVertex.simpleVert;
        mesh.PopulateUIVertex(ref source, 0);
        Rect outer = graphic.rectTransform.rect;
        contentArea.GetWorldCorners(corners);
        Vector3 bottomLeft = graphic.rectTransform.InverseTransformPoint(corners[0]);
        Vector3 topRight = graphic.rectTransform.InverseTransformPoint(corners[2]);
        float left = Mathf.Clamp(bottomLeft.x, outer.xMin, outer.xMax);
        float right = Mathf.Clamp(topRight.x, outer.xMin, outer.xMax);
        float bottom = Mathf.Clamp(bottomLeft.y, outer.yMin, outer.yMax);
        float top = Mathf.Clamp(topRight.y, outer.yMin, outer.yMax);

        mesh.Clear();
        AddRectangle(mesh, source, outer.xMin, outer.yMin, outer.xMax, bottom);
        AddRectangle(mesh, source, outer.xMin, top, outer.xMax, outer.yMax);
        AddRectangle(mesh, source, outer.xMin, bottom, left, top);
        AddRectangle(mesh, source, right, bottom, outer.xMax, top);
    }

    static void AddRectangle(VertexHelper mesh, UIVertex vertex, float left, float bottom,
        float right, float top)
    {
        if (right <= left || top <= bottom)
            return;

        int first = mesh.currentVertCount;
        vertex.uv0 = new Vector4(0.5f, 0.5f, 0f, 0f);
        vertex.position = new Vector3(left, bottom, 0f); mesh.AddVert(vertex);
        vertex.position = new Vector3(left, top, 0f); mesh.AddVert(vertex);
        vertex.position = new Vector3(right, top, 0f); mesh.AddVert(vertex);
        vertex.position = new Vector3(right, bottom, 0f); mesh.AddVert(vertex);
        mesh.AddTriangle(first, first + 1, first + 2);
        mesh.AddTriangle(first, first + 2, first + 3);
    }
}

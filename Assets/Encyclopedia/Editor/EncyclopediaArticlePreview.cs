using TMPro;
using UnityEditor;
using UnityEngine;

// Render actual TMP glyphs/materials rather than approximating them with IMGUI rich text.
public sealed class EncyclopediaArticlePreview
{
    readonly PreviewRenderUtility preview = new PreviewRenderUtility();
    readonly RectTransform canvasRect;
    readonly TextMeshProUGUI text;
    float scroll;

    public EncyclopediaArticlePreview(TMP_Text defaults)
    {
        var root = new GameObject("Article preview", typeof(RectTransform));
        root.hideFlags = HideFlags.HideAndDontSave;
        root.layer = 5;
        canvasRect = root.GetComponent<RectTransform>();
        canvasRect.pivot = new Vector2(0f, 1f);
        Canvas canvas = root.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        canvas.worldCamera = preview.camera;

        var content = new GameObject("Article text", typeof(RectTransform));
        content.hideFlags = HideFlags.HideAndDontSave;
        content.layer = 5;
        content.transform.SetParent(root.transform, false);
        text = content.AddComponent<TextMeshProUGUI>();
        text.font = defaults.font;
        text.fontSize = defaults.fontSize;
        text.fontStyle = defaults.fontStyle;
        text.color = defaults.color;
        text.richText = true;
        text.textWrappingMode = TextWrappingModes.Normal;
        text.raycastTarget = false;
        text.rectTransform.anchorMin = new Vector2(0f, 1f);
        text.rectTransform.anchorMax = new Vector2(1f, 1f);
        text.rectTransform.pivot = new Vector2(0.5f, 1f);

        preview.AddSingleGO(root);
        preview.camera.orthographic = true;
        preview.camera.nearClipPlane = 0.1f;
        preview.camera.farClipPlane = 100f;
        preview.camera.clearFlags = CameraClearFlags.SolidColor;
        preview.camera.backgroundColor = new Color(0.21f, 0.21f, 0.21f);
    }

    public bool Draw(string formattedText)
    {
        float previousScroll = scroll;
        Rect area = GUILayoutUtility.GetRect(100f, 300f, GUILayout.ExpandWidth(true));
        Rect viewport = new Rect(area.x, area.y, Mathf.Max(80f, area.width - 16f), area.height);
        float textWidth = Mathf.Max(60f, viewport.width - 24f);
        text.text = formattedText;
        float contentHeight = Mathf.Max(viewport.height, text.GetPreferredValues(formattedText, textWidth, 0f).y + 24f);
        float maximumScroll = Mathf.Max(0f, contentHeight - viewport.height);
        scroll = Mathf.Clamp(scroll, 0f, maximumScroll);

        if (Event.current.type == EventType.ScrollWheel && viewport.Contains(Event.current.mousePosition))
        {
            scroll = Mathf.Clamp(scroll + Event.current.delta.y * 20f, 0f, maximumScroll);
            Event.current.Use();
        }
        scroll = GUI.VerticalScrollbar(new Rect(viewport.xMax, area.y, 16f, area.height),
            scroll, viewport.height, 0f, contentHeight);

        if (Event.current.type == EventType.Repaint)
        {
            canvasRect.sizeDelta = new Vector2(viewport.width, viewport.height);
            text.rectTransform.sizeDelta = new Vector2(-24f, contentHeight);
            text.rectTransform.anchoredPosition = new Vector2(0f, scroll - 12f);
            preview.camera.transform.SetPositionAndRotation(
                new Vector3(viewport.width * 0.5f, -viewport.height * 0.5f, -10f), Quaternion.identity);
            preview.camera.orthographicSize = viewport.height * 0.5f;
            text.ForceMeshUpdate();
            Canvas.ForceUpdateCanvases();
            preview.BeginPreview(viewport, GUIStyle.none);
            preview.Render(true);
            GUI.DrawTexture(viewport, preview.EndPreview(), ScaleMode.StretchToFill, false);
        }
        return !Mathf.Approximately(previousScroll, scroll);
    }

    public void Cleanup()
    {
        preview.Cleanup();
    }
}

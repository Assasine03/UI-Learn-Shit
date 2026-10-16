using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(EncyclopediaEntry))]
public class EncyclopediaEntryEditor : Editor
{
    const string BodyControl = "EncyclopediaArticleBody";
    SerializedProperty id;
    SerializedProperty title;
    SerializedProperty body;
    SerializedProperty keywords;
    SerializedProperty children;
    SerializedProperty fonts;
    EncyclopediaArticlePreview preview;
    TMP_FontAsset chosenFont;
    Color chosenColor = new Color(1f, 0.35f, 0.3f);
    float chosenSize = 120f;
    int sizeUnit;
    int selectionStart;
    int selectionEnd;
    bool restoreSelection;
    bool previewDirty = true;
    bool detailsOpen;
    Vector2 sourceScroll;
    string previewText = "";
    string previewError = "";

    void OnEnable()
    {
        id = serializedObject.FindProperty("id");
        title = serializedObject.FindProperty("title");
        body = serializedObject.FindProperty("body");
        keywords = serializedObject.FindProperty("keywords");
        children = serializedObject.FindProperty("children");
        fonts = serializedObject.FindProperty("fonts");

        GameObject prefab = AssetDatabase.FindAssets("Encyclopedia t:Prefab", new[] { "Assets/Encyclopedia" })
            .Select(guid => AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GUIDToAssetPath(guid)))
            .Single(candidate => candidate.name == "Encyclopedia");
        var controller = new SerializedObject(prefab.GetComponent<EncyclopediaController>());
        var defaults = (TMP_Text)controller.FindProperty("article").objectReferenceValue;
        chosenFont = defaults.font;
        preview = new EncyclopediaArticlePreview(defaults);
        Undo.undoRedoPerformed += InvalidatePreview;
        EditorApplication.projectChanged += InvalidatePreview;
    }

    void OnDisable()
    {
        Undo.undoRedoPerformed -= InvalidatePreview;
        EditorApplication.projectChanged -= InvalidatePreview;
        preview.Cleanup();
    }

    void InvalidatePreview()
    {
        previewDirty = true;
        Repaint();
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        CaptureSelection();
        EditorGUILayout.LabelField("Artikel-Editor", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(title, new GUIContent("Überschrift"));
        EditorGUILayout.PropertyField(id, new GUIContent("Artikel-ID"));
        EditorGUILayout.HelpBox("Text im Feld markieren und formatieren. Ohne Auswahl wird ein markierter Platzhalter eingefügt. " +
            "Die Vorschau darunter zeigt die tatsächliche TMP-Darstellung.", MessageType.Info);

        DrawToolbar();
        // The edit field deliberately shows the tags. Rendering is handled by the TMP preview.
        var sourceStyle = new GUIStyle(EditorStyles.textArea) { wordWrap = true, richText = false };
        sourceScroll = EditorGUILayout.BeginScrollView(sourceScroll, GUILayout.Height(240f));
        GUI.SetNextControlName(BodyControl);
        body.stringValue = EditorGUILayout.TextArea(body.stringValue, sourceStyle,
            GUILayout.MinHeight(220f), GUILayout.ExpandHeight(true));
        EditorGUILayout.EndScrollView();
        RestoreSelection();
        CaptureSelection();

        EditorGUILayout.PropertyField(keywords, new GUIContent("Zusätzliche Suchbegriffe"), true);
        EditorGUILayout.PropertyField(children, new GUIContent("Unterartikel / Unterkategorien"), true);
        detailsOpen = EditorGUILayout.Foldout(detailsOpen, "Technische Angaben und verwendete Schriften", true);
        if (detailsOpen)
        {
            EditorGUILayout.PropertyField(fonts, new GUIContent("Referenzierte TMP-Schriften"), true);
        }
        if (serializedObject.ApplyModifiedProperties())
        {
            previewDirty = true;
        }

        if (GUILayout.Button("Artikel speichern"))
        {
            AssetDatabase.SaveAssetIfDirty(target);
        }

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("TextMeshPro-Vorschau", EditorStyles.boldLabel);
        if (previewDirty)
        {
            RefreshPreview();
        }
        if (previewError.Length > 0)
        {
            EditorGUILayout.HelpBox(previewError, MessageType.Error);
        }
        else
        {
            EditorGUILayout.LabelField(title.stringValue, EditorStyles.boldLabel);
            if (preview.Draw(previewText))
            {
                Repaint();
            }
            EditorGUILayout.HelpBox("Die Vorschau verwendet Schrift und Grundgröße des Enzyklopädie-Prefabs. " +
                "Gelb unterstrichene Begriffe sind automatische Links. Explizite Textfarben bleiben erhalten.", MessageType.None);
        }
    }

    void DrawToolbar()
    {
        using (new EditorGUILayout.HorizontalScope())
        {
            if (GUILayout.Button(new GUIContent("B", "Fett"), EditorStyles.miniButton, GUILayout.Width(30f)))
            {
                WrapSelection("<b>", "</b>");
            }
            if (GUILayout.Button(new GUIContent("I", "Kursiv"), EditorStyles.miniButton, GUILayout.Width(30f)))
            {
                WrapSelection("<i>", "</i>");
            }
            if (GUILayout.Button(new GUIContent("U", "Unterstrichen"), EditorStyles.miniButton, GUILayout.Width(30f)))
            {
                WrapSelection("<u>", "</u>");
            }
            if (GUILayout.Button(new GUIContent("S", "Durchgestrichen"), EditorStyles.miniButton, GUILayout.Width(30f)))
            {
                WrapSelection("<s>", "</s>");
            }
            if (GUILayout.Button(new GUIContent("Alle Formate entfernen", "Entfernt die Formatierung im gesamten Artikel."), EditorStyles.miniButton))
            {
                RemoveFormatting();
            }
        }
        float originalLabelWidth = EditorGUIUtility.labelWidth;
        EditorGUIUtility.labelWidth = 48f;
        using (new EditorGUILayout.HorizontalScope())
        {
            chosenColor = EditorGUILayout.ColorField(new GUIContent("Farbe"), chosenColor, true, true, false);
            if (GUILayout.Button("Anwenden", GUILayout.Width(85f)))
            {
                WrapSelection("<color=#" + ColorUtility.ToHtmlStringRGBA(chosenColor) + ">", "</color>");
            }
        }
        using (new EditorGUILayout.HorizontalScope())
        {
            chosenSize = Mathf.Clamp(EditorGUILayout.FloatField("Größe", chosenSize), 1f, 1000f);
            sizeUnit = EditorGUILayout.Popup(sizeUnit, new[] { "%", "pt" }, GUILayout.Width(48f));
            if (GUILayout.Button("Anwenden", GUILayout.Width(85f)))
            {
                string value = chosenSize.ToString("0.##", CultureInfo.InvariantCulture) + (sizeUnit == 0 ? "%" : "");
                WrapSelection("<size=" + value + ">", "</size>");
            }
        }
        using (new EditorGUILayout.HorizontalScope())
        {
            chosenFont = (TMP_FontAsset)EditorGUILayout.ObjectField("Schrift", chosenFont, typeof(TMP_FontAsset), false);
            // An empty optional font chooser is valid; required asset references are not suppressed.
            using (new EditorGUI.DisabledScope(chosenFont == null))
            {
                if (GUILayout.Button("Anwenden", GUILayout.Width(85f)))
                {
                    AddFontReference();
                    WrapSelection("<font=\"" + chosenFont.name + "\">", "</font>");
                }
            }
        }
        EditorGUIUtility.labelWidth = originalLabelWidth;
    }

    void CaptureSelection()
    {
        if (GUI.GetNameOfFocusedControl() == BodyControl)
        {
            var editor = (TextEditor)GUIUtility.GetStateObject(typeof(TextEditor), GUIUtility.keyboardControl);
            selectionStart = Mathf.Min(editor.cursorIndex, editor.selectIndex);
            selectionEnd = Mathf.Max(editor.cursorIndex, editor.selectIndex);
        }
    }

    void RestoreSelection()
    {
        if (restoreSelection)
        {
            GUI.FocusControl(BodyControl);
            EditorGUIUtility.editingTextField = true;
            var editor = (TextEditor)GUIUtility.GetStateObject(typeof(TextEditor), GUIUtility.keyboardControl);
            editor.text = body.stringValue;
            editor.cursorIndex = selectionStart;
            editor.selectIndex = selectionEnd;
            restoreSelection = false;
        }
    }

    void WrapSelection(string opening, string closing)
    {
        string source = body.stringValue;
        int start = Mathf.Clamp(selectionStart, 0, source.Length);
        int end = Mathf.Clamp(selectionEnd, start, source.Length);
        string selected = start == end ? "Text" : source.Substring(start, end - start);
        body.stringValue = source.Substring(0, start) + opening + selected + closing + source.Substring(end);
        selectionStart = start + opening.Length;
        selectionEnd = selectionStart + selected.Length;
        restoreSelection = true;
    }

    void RemoveFormatting()
    {
        string source = body.stringValue;
        string clean = System.Text.RegularExpressions.Regex.Replace(source,
            @"</?(?:b|i|u|s|size|color|font)(?:=[^<>]+)?>", "", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        body.stringValue = clean;
        selectionStart = 0;
        selectionEnd = clean.Length;
        restoreSelection = true;
    }

    void AddFontReference()
    {
        if (chosenFont.name.IndexOfAny(new[] { '"', '<', '>' }) >= 0)
        {
            throw new ArgumentException("Der Schriftname darf keine Anführungszeichen oder spitzen Klammern enthalten.");
        }
        for (int i = 0; i < fonts.arraySize; i++)
        {
            if (fonts.GetArrayElementAtIndex(i).objectReferenceValue == chosenFont)
            {
                return;
            }
        }
        fonts.InsertArrayElementAtIndex(fonts.arraySize);
        fonts.GetArrayElementAtIndex(fonts.arraySize - 1).objectReferenceValue = chosenFont;
    }

    void RefreshPreview()
    {
        previewDirty = false;
        previewError = "";
        try
        {
            var entry = (EncyclopediaEntry)target;
            EncyclopediaEntry root = FindRoot(entry);
            var index = new EncyclopediaIndex(root);
            foreach (EncyclopediaEntry item in index.Entries)
            {
                foreach (TMP_FontAsset font in item.Fonts)
                {
                    MaterialReferenceManager.AddFontAsset(font);
                }
            }
            previewText = index.ArticleText(entry);
        }
        catch (Exception exception) when (exception is ArgumentException || exception is FormatException)
        {
            // Invalid drafts stay editable; the exact error is visible instead of hiding it.
            previewError = exception.Message;
        }
    }

    static EncyclopediaEntry FindRoot(EncyclopediaEntry entry)
    {
        var parents = new Dictionary<EncyclopediaEntry, EncyclopediaEntry>();
        foreach (string guid in AssetDatabase.FindAssets("t:EncyclopediaEntry", new[] { "Assets/Encyclopedia" }))
        {
            var candidate = AssetDatabase.LoadAssetAtPath<EncyclopediaEntry>(AssetDatabase.GUIDToAssetPath(guid));
            foreach (EncyclopediaEntry child in candidate.Children)
            {
                if (parents.ContainsKey(child))
                {
                    throw new ArgumentException("Ein Eintrag ist mehrfach einsortiert: " + child.Title);
                }
                parents.Add(child, candidate);
            }
        }
        var visited = new HashSet<EncyclopediaEntry>();
        while (parents.TryGetValue(entry, out EncyclopediaEntry parent))
        {
            if (!visited.Add(entry))
            {
                throw new ArgumentException("Kreisverweis in den Kategorien: " + entry.Title);
            }
            entry = parent;
        }
        return entry;
    }
}

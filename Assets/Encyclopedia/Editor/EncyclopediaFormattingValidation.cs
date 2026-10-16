using System;
using UnityEditor;
using UnityEngine;

public static class EncyclopediaFormattingValidation
{
    [MenuItem("Tools/Validate Encyclopedia Formatting")]
    public static void Run()
    {
        var root = ScriptableObject.CreateInstance<EncyclopediaEntry>();
        var article = ScriptableObject.CreateInstance<EncyclopediaEntry>();
        var drone = ScriptableObject.CreateInstance<EncyclopediaEntry>();
        try
        {
            SetEntry(root, "root", "Enzyklopädie");
            SetEntry(article, "article", "Testartikel");
            SetEntry(drone, "drone", "Hunter Drone");
            var rootData = new SerializedObject(root);
            SerializedProperty children = rootData.FindProperty("children");
            children.arraySize = 2;
            children.GetArrayElementAtIndex(0).objectReferenceValue = article;
            children.GetArrayElementAtIndex(1).objectReferenceValue = drone;
            rootData.ApplyModifiedPropertiesWithoutUndo();
            var index = new EncyclopediaIndex(root);

            string rich = Format(index, article, "Hunter <b>Drone</b>");
            Require(rich.Contains("<link=\"drone\">"), "A multiword name must link across formatting tags.");
            Require(rich.Contains("<b>Drone"), "Inline bold must survive link generation.");
            rich = Format(index, article, "<color=#FF5555>Hunter Drone</color>");
            Require(rich.Contains("<link=\"drone\">"), "Colored words must remain clickable.");
            Require(!rich.Contains("#FFD34D"), "Link styling must not override designer colors.");
            rich = Format(index, article, "<font=\"Hunter Drone\">Other text</font>");
            Require(!rich.Contains("<link="), "Font names must never generate article links.");
            Require(EncyclopediaRichText.Parse("<size=150%><i>Text</i></size>").VisibleText == "Text",
                "Size and italic must preserve visible text.");
            bool invalid = false;
            try
            {
                EncyclopediaRichText.Parse("<b>Unclosed");
            }
            catch (FormatException)
            {
                invalid = true;
            }
            Require(invalid, "Invalid drafts must report a formatting error.");
            Debug.Log("ENCYCLOPEDIA_FORMATTING_VALIDATION_PASSED");
        }
        finally
        {
            UnityEngine.Object.DestroyImmediate(root);
            UnityEngine.Object.DestroyImmediate(article);
            UnityEngine.Object.DestroyImmediate(drone);
        }
    }

    static void SetEntry(EncyclopediaEntry entry, string id, string title)
    {
        var data = new SerializedObject(entry);
        data.FindProperty("id").stringValue = id;
        data.FindProperty("title").stringValue = title;
        data.ApplyModifiedPropertiesWithoutUndo();
    }

    static string Format(EncyclopediaIndex index, EncyclopediaEntry entry, string text)
    {
        var data = new SerializedObject(entry);
        data.FindProperty("body").stringValue = text;
        data.ApplyModifiedPropertiesWithoutUndo();
        return index.ArticleText(entry);
    }

    static void Require(bool condition, string message)
    {
        if (!condition)
        {
            throw new Exception(message);
        }
    }
}

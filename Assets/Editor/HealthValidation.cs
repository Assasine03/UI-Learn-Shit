using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public static class HealthValidation
{
    [MenuItem("Tools/Validate Health UI")]
    public static void Run()
    {
        GameObject root = PrefabUtility.LoadPrefabContents("Assets/UI.prefab");
        try
        {
            Character character = root.transform.Find("PlayerCharacter").GetComponent<Character>();
            HealthBar bar = root.transform.Find("Canvas/Maske").GetComponent<HealthBar>();
            Require(character != null && bar != null, "Health components must be assigned.");
            var settings = new SerializedObject(bar);
            Require(settings.FindProperty("character").objectReferenceValue == character,
                "HealthBar must reference the character.");
            var fill = (RectTransform)settings.FindProperty("fill").objectReferenceValue;
            Require(fill != null && fill.parent == bar.transform, "Fill must be inside mask.");
            var mask = bar.GetComponent<UnityEngine.UI.Mask>();
            Require(mask != null && !mask.showMaskGraphic, "Mask should clip without drawing itself.");
            var outline = bar.transform.parent.Find("HealthOutline");
            Require(outline != null && outline.GetSiblingIndex() > bar.transform.GetSiblingIndex(),
                "Outline must be outside the mask and drawn afterwards.");

            var refresh = typeof(HealthBar).GetMethod("Refresh", BindingFlags.NonPublic | BindingFlags.Instance);
            character.SetHealth(100f);
            for (int i = 0; i <= 10; i++)
            {
                float expectedHealth = 100f - i * 10f;
                Require(Mathf.Approximately(character.Health, expectedHealth), "Damage increments must be 10.");
                refresh.Invoke(bar, new object[] { character.Health });
                float width = ((RectTransform)bar.transform).rect.width;
                Require(Mathf.Approximately(fill.rect.width, width),
                    "Actual fill width must equal the mask width, including stretch offsets.");
                Require(Mathf.Approximately(fill.rect.height, ((RectTransform)bar.transform).rect.height),
                    "Actual fill height must equal the mask height.");
                Require(Mathf.Approximately(fill.anchoredPosition.x, -width * i / 10f),
                    "Fill must move left by the missing health fraction.");
                character.TakeDamage(10f);
            }
            Require(character.Health == 0f, "Health cannot become negative.");
            var maskRect = (RectTransform)bar.transform;
            Vector3[] corners = new Vector3[4];
            fill.GetWorldCorners(corners);
            float rightEdge = maskRect.InverseTransformPoint(corners[2]).x;
            Require(rightEdge <= maskRect.rect.xMin + 0.01f,
                "At zero health the entire fill must lie to the left of the mask.");
            character.SetHealth(150f);
            Require(character.Health == 100f, "Health cannot exceed 100.");
            character.TakeDamage(-10f);
            Require(character.Health == 100f, "Negative damage cannot heal.");
            Debug.Log("HEALTH_UI_VALIDATION_PASSED");
        }
        finally
        {
            PrefabUtility.UnloadPrefabContents(root);
        }
    }

    static void Require(bool condition, string message)
    {
        if (!condition)
        {
            throw new Exception(message);
        }
    }
}

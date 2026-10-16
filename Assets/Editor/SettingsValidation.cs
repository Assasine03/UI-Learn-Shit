using System;
using System.Reflection;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public static class SettingsValidation
{
    [MenuItem("Tools/Validate Settings Menu")]
    public static void Run()
    {
        GameObject root = PrefabUtility.LoadPrefabContents("Assets/UI.prefab");
        try
        {
            var controller = root.GetComponent<SettingsMenuController>();
            Require(controller != null, "Settings controller must be on active UI root.");
            var data = new SerializedObject(controller);
            var window = (GameObject)data.FindProperty("settingsWindow").objectReferenceValue;
            var open = (Button)data.FindProperty("settingsButton").objectReferenceValue;
            var back = (Button)data.FindProperty("backButton").objectReferenceValue;
            Require(window != null && open != null && back != null, "Window buttons must be wired.");
            Require(!window.activeSelf, "Settings window starts closed.");
            var buttons = data.FindProperty("tabButtons");
            var panels = data.FindProperty("tabPanels");
            Require(buttons.arraySize == 4 && panels.arraySize == 4, "Exactly four tabs are required.");
            typeof(SettingsMenuController).GetMethod("Awake", BindingFlags.Instance | BindingFlags.NonPublic)
                .Invoke(controller, null);
            open.onClick.Invoke();
            Require(window.activeSelf, "Settings button must open the window.");
            for (int tab = 0; tab < 4; tab++)
            {
                ((Button)buttons.GetArrayElementAtIndex(tab).objectReferenceValue).onClick.Invoke();
                for (int panel = 0; panel < 4; panel++)
                {
                    var content = (GameObject)panels.GetArrayElementAtIndex(panel).objectReferenceValue;
                    Require(content.activeSelf == (panel == tab), "Only the selected panel can be active.");
                    var button = (Button)buttons.GetArrayElementAtIndex(panel).objectReferenceValue;
                    var label = (TMP_Text)data.FindProperty("tabLabels").GetArrayElementAtIndex(panel).objectReferenceValue;
                    Require(label.color == (panel == tab ? Color.red : Color.white), "Tab highlight must follow selection.");
                }
                back.onClick.Invoke();
                Require(!window.activeSelf, "Back must close Settings from every tab.");
                open.onClick.Invoke();
                Require(((GameObject)panels.GetArrayElementAtIndex(0).objectReferenceValue).activeSelf,
                    "Reopening must select Spiel.");
            }
            Debug.Log("SETTINGS_MENU_VALIDATION_PASSED");
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

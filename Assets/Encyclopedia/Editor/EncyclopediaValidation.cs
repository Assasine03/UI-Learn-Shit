using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public static class EncyclopediaValidation
{
    [MenuItem("Tools/Validate Encyclopedia")]
    public static void Run()
    {
        GameObject ui = PrefabUtility.LoadPrefabContents("Assets/UI.prefab");
        CursorLockMode cursorLock = Cursor.lockState;
        bool cursorVisible = Cursor.visible;
        float timeScale = Time.timeScale;
        try
        {
            var encyclopedia = ui.transform.Find("Encyclopedia").GetComponent<EncyclopediaController>();
            var pause = ui.GetComponent<EscapeMenuController>();
            var data = new SerializedObject(encyclopedia);
            var root = (EncyclopediaEntry)data.FindProperty("rootEntry").objectReferenceValue;
            Require(data.FindProperty("pauseMenu").objectReferenceValue == pause, "Pause menu reference must be explicit.");
            Require(new SerializedObject(pause).FindProperty("encyclopedia").objectReferenceValue == encyclopedia,
                "ESC must reference the encyclopedia.");
            Require(!encyclopedia.IsOpen, "Encyclopedia starts closed.");

            var index = new EncyclopediaIndex(root);
            var drone = index.Find("hunter-drone");
            Require(index.PathTo(drone).Count == 4, "Drone path must include root, enemies and robots.");
            Require(index.Breadcrumb(drone).Contains("<link=\"gegner\">"), "Category breadcrumb must be clickable.");
            Require(index.ArticleText(drone).Contains("<link=\"imperium\">"), "Keywords must link to other articles.");
            Require(!index.ArticleText(drone).Contains("<link=\"hunter-drone\">"), "An article should not link to itself.");

            Invoke(pause, "Awake");
            Invoke(encyclopedia, "Awake");
            encyclopedia.Open();
            Require(encyclopedia.IsOpen && Time.timeScale == timeScale, "J window must open without changing game speed.");
            pause.ToggleMenu();
            Require(!encyclopedia.IsOpen && !pause.IsOpen, "First ESC must close only the encyclopedia.");
            pause.ToggleMenu();
            Require(pause.IsOpen && Time.timeScale == 0f, "Next ESC opens the existing pause menu.");
            pause.CloseMenu();
            Require(Time.timeScale == timeScale, "Resume must restore game speed.");
            Debug.Log("ENCYCLOPEDIA_VALIDATION_PASSED");
        }
        finally
        {
            PrefabUtility.UnloadPrefabContents(ui);
            Time.timeScale = timeScale;
            Cursor.lockState = cursorLock;
            Cursor.visible = cursorVisible;
        }
    }

    static void Invoke(MonoBehaviour component, string method)
    {
        component.GetType().GetMethod(method, BindingFlags.NonPublic | BindingFlags.Instance).Invoke(component, null);
    }

    static void Require(bool condition, string message)
    {
        if (!condition)
        {
            throw new Exception(message);
        }
    }
}

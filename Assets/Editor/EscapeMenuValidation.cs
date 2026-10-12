using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public static class EscapeMenuValidation
{
    [MenuItem("Tools/Validate Escape Menu")]
    public static void Run()
    {
        GameObject root = PrefabUtility.LoadPrefabContents("Assets/UI.prefab");
        float originalScale = Time.timeScale;
        CursorLockMode originalLock = Cursor.lockState;
        bool originalVisible = Cursor.visible;
        try
        {
            var controller = root.GetComponent<EscapeMenuController>();
            Require(controller != null, "Controller must be on the active prefab root.");
            var serialized = new SerializedObject(controller);
            var menu = (GameObject)serialized.FindProperty("menuCanvas").objectReferenceValue;
            var resume = (Button)serialized.FindProperty("resumeButton").objectReferenceValue;
            Require(menu != null && resume != null, "Canvas and resume button must be assigned.");
            Require(!menu.activeSelf && root.activeSelf, "Menu starts closed; controller stays active.");
            Require(resume.transform.IsChildOf(menu.transform), "Resume button must belong to menu.");
            Require(menu.GetComponent<Canvas>().sortingOrder > 0, "Menu must render above gameplay UI.");

            // Exercise lifecycle explicitly because this is an Edit Mode validation.
            Invoke(controller, "Awake");
            Time.timeScale = 0.75f;
            controller.ToggleMenu();
            Require(menu.activeSelf && Time.timeScale == 0f, "First toggle opens and pauses.");
            controller.OpenMenu();
            resume.onClick.Invoke();
            Require(!menu.activeSelf && Time.timeScale == 0.75f, "Resume restores previous game speed.");
            controller.ToggleMenu();
            controller.ToggleMenu();
            Require(!menu.activeSelf && Time.timeScale == 0.75f, "Second toggle closes and resumes.");
            controller.OpenMenu();
            Invoke(controller, "OnDisable");
            Require(!menu.activeSelf && Time.timeScale == 0.75f, "Disabling cleans up pause state.");
            Debug.Log("ESC_MENU_VALIDATION_PASSED");
        }
        finally
        {
            PrefabUtility.UnloadPrefabContents(root);
            Time.timeScale = originalScale;
            Cursor.lockState = originalLock;
            Cursor.visible = originalVisible;
        }
    }

    static void Invoke(EscapeMenuController controller, string method)
    {
        typeof(EscapeMenuController).GetMethod(method, BindingFlags.Instance | BindingFlags.NonPublic)
            .Invoke(controller, null);
    }

    static void Require(bool condition, string message)
    {
        if (!condition)
            throw new Exception(message);
    }
}

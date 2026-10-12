using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

// Keep this component on the active UI root, outside the canvas it switches.
public class EscapeMenuController : MonoBehaviour
{
    [SerializeField] GameObject menuCanvas;
    [SerializeField] Button resumeButton;
    [SerializeField] Button closeGameButton;
    [SerializeField] SettingsMenuController settings;
    [SerializeField] EscapeMenuBackdrop backdrop;
    [SerializeField] bool pauseGame = true;

    bool isOpen;
    float previousTimeScale;
    CursorLockMode previousCursorLock;
    bool previousCursorVisible;

    void Awake()
    {
        if (menuCanvas == null)
        {
            Debug.LogError("Assign the menu canvas to EscapeMenuController.", this);
            enabled = false;
            return;
        }

        menuCanvas.SetActive(false);
        if (resumeButton != null)
            resumeButton.onClick.AddListener(CloseMenu);
        if (closeGameButton != null)
            closeGameButton.onClick.AddListener(CloseGame);
    }

    void Update()
    {
        // A press is reported once, rather than on every frame while held.
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
            ToggleMenu();
    }

    public void ToggleMenu()
    {
        // ESC goes back one level; leave the pause menu and game state intact.
        if (isOpen && settings != null && settings.IsOpen)
        {
            settings.CloseSettings();
            return;
        }

        if (isOpen)
            CloseMenu();
        else
            OpenMenu();
    }

    public void OpenMenu()
    {
        if (isOpen || menuCanvas == null)
            return;

        previousTimeScale = Time.timeScale;
        previousCursorLock = Cursor.lockState;
        previousCursorVisible = Cursor.visible;
        isOpen = true;
        if (settings != null)
            settings.CloseSettings();
        if (backdrop != null)
            backdrop.Show();
        else
            menuCanvas.SetActive(true);

        if (pauseGame)
            Time.timeScale = 0f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void CloseMenu()
    {
        if (!isOpen)
            return;

        isOpen = false;
        if (settings != null)
            settings.CloseSettings();
        if (backdrop != null)
            backdrop.Hide();
        if (menuCanvas != null)
            menuCanvas.SetActive(false);

        if (pauseGame)
            Time.timeScale = previousTimeScale;

        Cursor.lockState = previousCursorLock;
        Cursor.visible = previousCursorVisible;
    }

    void OnDisable()
    {
        // Do not leave the game paused if the controller is disabled or removed.
        CloseMenu();
    }

    public void CloseGame()
    {
        CloseMenu();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    void OnDestroy()
    {
        if (resumeButton != null)
            resumeButton.onClick.RemoveListener(CloseMenu);
        if (closeGameButton != null)
            closeGameButton.onClick.RemoveListener(CloseGame);
    }
}

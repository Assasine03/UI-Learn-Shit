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
    [SerializeField] EncyclopediaController encyclopedia;
    [SerializeField] bool pauseGame = true;

    bool isOpen;
    float previousTimeScale;
    CursorLockMode previousCursorLock;
    bool previousCursorVisible;

    public bool IsOpen => isOpen;

    void Awake()
    {
        menuCanvas.SetActive(false);
        resumeButton.onClick.AddListener(CloseMenu);
        closeGameButton.onClick.AddListener(CloseGame);
    }

    void Update()
    {
        // A press is reported once, rather than on every frame while held.
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            ToggleMenu();
        }
    }

    public void ToggleMenu()
    {
        if (encyclopedia.IsOpen)
        {
            encyclopedia.Close();
            return;
        }

        // ESC goes back one level; leave the pause menu and game state intact.
        if (isOpen && settings.IsOpen)
        {
            settings.CloseSettings();
            return;
        }

        if (isOpen)
        {
            CloseMenu();
        }
        else
        {
            OpenMenu();
        }
    }

    public void OpenMenu()
    {
        if (isOpen)
        {
            return;
        }

        encyclopedia.Close();
        previousTimeScale = Time.timeScale;
        previousCursorLock = Cursor.lockState;
        previousCursorVisible = Cursor.visible;
        isOpen = true;
        settings.CloseSettings();
        backdrop.Show();

        if (pauseGame)
        {
            Time.timeScale = 0f;
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void CloseMenu()
    {
        if (!isOpen)
        {
            return;
        }

        isOpen = false;
        settings.CloseSettings();
        backdrop.Hide();
        menuCanvas.SetActive(false);

        if (pauseGame)
        {
            Time.timeScale = previousTimeScale;
        }

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
        resumeButton.onClick.RemoveListener(CloseMenu);
        closeGameButton.onClick.RemoveListener(CloseGame);
    }
}

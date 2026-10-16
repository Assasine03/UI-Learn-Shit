using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class SettingsMenuController : MonoBehaviour
{
    [SerializeField] GameObject settingsWindow;
    [SerializeField] Button settingsButton;
    [SerializeField] Button backButton;
    [SerializeField] Button[] tabButtons;
    [SerializeField] TMP_Text[] tabLabels;
    [SerializeField] GameObject[] tabPanels;

    readonly Color normalColor = new Color(0.09f, 0.36f, 0.46f);
    readonly Color selectedColor = new Color(0.15f, 0.48f, 0.60f);
    UnityAction[] tabHandlers;

    public bool IsOpen => settingsWindow.activeSelf;

    void Awake()
    {
        settingsButton.onClick.AddListener(OpenSettings);
        backButton.onClick.AddListener(CloseSettings);
        tabHandlers = new UnityAction[tabButtons.Length];
        for (int i = 0; i < tabButtons.Length; i++)
        {
            int tabIndex = i; // Each callback keeps its own index.
            tabHandlers[i] = () => SelectTab(tabIndex);
            tabButtons[i].onClick.AddListener(tabHandlers[i]);
        }
        CloseSettings();
    }

    public void OpenSettings()
    {
        settingsWindow.SetActive(true);
        SelectTab(0); // Always start on Spiel.
    }

    public void CloseSettings()
    {
        settingsWindow.SetActive(false);
    }

    public void SelectTab(int index)
    {
        if (index < 0 || index >= tabPanels.Length)
        {
            throw new System.ArgumentOutOfRangeException(nameof(index));
        }

        for (int i = 0; i < tabPanels.Length; i++)
        {
            bool selected = i == index;
            tabPanels[i].SetActive(selected);
            tabButtons[i].targetGraphic.color = selected ? selectedColor : normalColor;
            tabLabels[i].color = selected ? Color.red : Color.white;
        }
    }

    void OnDestroy()
    {
        settingsButton.onClick.RemoveListener(OpenSettings);
        backButton.onClick.RemoveListener(CloseSettings);
        for (int i = 0; i < tabHandlers.Length; i++)
        {
            tabButtons[i].onClick.RemoveListener(tabHandlers[i]);
        }
    }
}

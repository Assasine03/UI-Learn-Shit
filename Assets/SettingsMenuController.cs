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
    [SerializeField] GameObject[] tabPanels;

    readonly Color normalColor = new Color(0.09f, 0.36f, 0.46f);
    readonly Color selectedColor = new Color(0.15f, 0.48f, 0.60f);
    UnityAction[] tabHandlers;

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
            return;

        for (int i = 0; i < tabPanels.Length; i++)
        {
            bool selected = i == index;
            tabPanels[i].SetActive(selected);
            tabButtons[i].targetGraphic.color = selected ? selectedColor : normalColor;
            var label = tabButtons[i].GetComponentInChildren<TMP_Text>(true);
            label.color = selected ? Color.red : Color.white;
        }
    }

    void OnDestroy()
    {
        if (settingsButton != null)
            settingsButton.onClick.RemoveListener(OpenSettings);
        if (backButton != null)
            backButton.onClick.RemoveListener(CloseSettings);
        if (tabHandlers != null)
            for (int i = 0; i < tabHandlers.Length; i++)
                if (tabButtons[i] != null)
                    tabButtons[i].onClick.RemoveListener(tabHandlers[i]);
    }
}

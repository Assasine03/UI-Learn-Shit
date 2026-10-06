using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

enum ButtonDefinitions
{
    ToggleButton,
    ChangeText1,
    ChangeText2,
    ChangeText3,
    ChangeText4
}

public class UIManager : MonoBehaviour
{

    [SerializeField] OurMainCanvas mainUI;
    [SerializeField] OurTextContainer textContainer;

    string text1 = "Test1";
    string text2 = "Test2";
    string text3 = "Test3";
    string text4 = "Test4";

    [SerializeField] Button button1;
    [SerializeField] Button button2;
    [SerializeField] Button button3;

    [SerializeField] List<Button> buttonList;

    void Start()
    {
        // button1.onClick.AddListener(() => ShowHideCanvas());

        int co = 0; // counter
        foreach (Button button in buttonList)
        {
            if (co == (int)ButtonDefinitions.ToggleButton)
            {
                button.onClick.AddListener(() => ShowHideCanvas());
            }
            else if (co == (int)ButtonDefinitions.ChangeText1)
            {
                button.onClick.AddListener(() => ChangeText(0, text1));
            }
            else if (co == (int)ButtonDefinitions.ChangeText2)
            {
                button.onClick.AddListener(() => ChangeText(1, text2));
            }
            else if (co == (int)ButtonDefinitions.ChangeText3)
            {
                button.onClick.AddListener(() => ChangeText(0, text3));
            }
            else if (co == (int)ButtonDefinitions.ChangeText4)
            {
                button.onClick.AddListener(() => ChangeText(1, text4));
            }

            co++;
        }
    }

    void ShowHideCanvas()
    {
        mainUI.ToggleCanvas();
    }

    void ChangeText(int index, string text)
    {
        textContainer.SetText(index, text);
    }
}

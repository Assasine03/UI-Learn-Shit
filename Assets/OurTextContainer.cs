using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class OurTextContainer : MonoBehaviour
{
    [SerializeField] List<TextMeshProUGUI> textComp;

    public void SetText(int index, string text)
    {
        textComp[index].text = text;
    }
}

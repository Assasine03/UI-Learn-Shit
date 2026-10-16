using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EncyclopediaRow : MonoBehaviour
{
    [SerializeField] Button button;
    [SerializeField] TMP_Text label;
    [SerializeField] LayoutElement rowLayout;
    [SerializeField] float indent = 24f;

    public void Configure(EncyclopediaEntry entry, int depth, bool expanded, bool selected, Action clicked)
    {
        bool category = entry.Children.Count > 0;
        string marker = category ? (expanded ? "v  " : ">  ") : "    ";
        label.text = marker + EncyclopediaIndex.Plain(entry.Title);
        label.margin = new Vector4(12f + depth * indent, 0f, 12f, 0f);
        label.fontStyle = category ? FontStyles.Bold : FontStyles.Underline;
        label.color = selected ? new Color(0.45f, 0.85f, 1f) :
            (category ? Color.white : new Color(1f, 0.83f, 0.3f));
        // Accommodate wrapped titles as well as the minimum row height.
        float width = ((RectTransform)transform.parent).rect.width;
        rowLayout.preferredHeight = Mathf.Max(42f, label.GetPreferredValues(label.text,
            Mathf.Max(60f, width - label.margin.x - label.margin.z), 0f).y + 12f);
        button.onClick.AddListener(() => clicked());
    }
}

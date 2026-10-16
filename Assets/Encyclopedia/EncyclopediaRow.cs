using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class EncyclopediaRow : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
{
    [SerializeField] Button button;
    [SerializeField] TMP_Text label;
    [SerializeField] LayoutElement rowLayout;
    [SerializeField] EncyclopediaTerminalGraphic background;
    [SerializeField] EncyclopediaTerminalGraphic leadingIcon;
    [SerializeField] Image selectionBar;
    [SerializeField] Image hierarchyGuide;
    [SerializeField] TMP_Text childCount;
    [SerializeField] float indent = 24f;

    bool isSelected;
    bool isCategory;

    public void Configure(EncyclopediaEntry entry, int depth, bool expanded, bool selected, Action clicked)
    {
        isCategory = entry.Children.Count > 0;
        isSelected = selected;
        label.text = EncyclopediaIndex.Plain(entry.Title);
        label.margin = Vector4.zero;
        label.fontStyle = isCategory ? FontStyles.Bold : FontStyles.Normal;
        float iconPosition = 20f + depth * indent;
        leadingIcon.Shape = isCategory ? EncyclopediaTerminalGraphic.Symbol.Chevron : EncyclopediaTerminalGraphic.Symbol.Diamond;
        leadingIcon.rectTransform.anchoredPosition = new Vector2(iconPosition, 0f);
        leadingIcon.rectTransform.sizeDelta = isCategory ? new Vector2(18f, 18f) : new Vector2(8f, 8f);
        leadingIcon.rectTransform.localRotation = Quaternion.Euler(0f, 0f, isCategory && expanded ? -90f : 0f);
        label.rectTransform.offsetMin = new Vector2(iconPosition + 18f, 6f);
        label.rectTransform.offsetMax = new Vector2(-50f, -6f);
        childCount.text = isCategory ? entry.Children.Count.ToString("00") : "";
        hierarchyGuide.gameObject.SetActive(depth > 0);
        hierarchyGuide.rectTransform.anchoredPosition = new Vector2(20f + (depth - 1) * indent, 0f);
        selectionBar.gameObject.SetActive(selected);
        button.transition = Selectable.Transition.None;
        ApplyStyle(false);
        // Accommodate wrapped titles as well as the minimum row height.
        float width = ((RectTransform)transform.parent).rect.width;
        rowLayout.preferredHeight = Mathf.Max(isCategory ? 46f : 42f, label.GetPreferredValues(label.text,
            Mathf.Max(60f, width - iconPosition - 68f), 0f).y + 14f);
        button.onClick.AddListener(() => clicked());
    }

    void ApplyStyle(bool highlighted)
    {
        background.color = isSelected ? new Color(0.10f, 0.23f, 0.25f) :
            (highlighted ? new Color(0.075f, 0.17f, 0.19f) :
                (isCategory ? new Color(0.045f, 0.105f, 0.12f) : new Color(0.035f, 0.075f, 0.085f)));
        background.OutlineColor = isSelected ? new Color(0.36f, 0.68f, 0.68f) :
            (highlighted ? new Color(0.20f, 0.43f, 0.45f) : new Color(0.09f, 0.20f, 0.22f));
        label.color = isSelected ? new Color(1f, 0.85f, 0.65f) :
            (highlighted ? new Color(0.88f, 0.97f, 0.95f) :
                (isCategory ? new Color(0.77f, 0.87f, 0.85f) : new Color(0.58f, 0.75f, 0.74f)));
        leadingIcon.color = isSelected ? new Color(0.94f, 0.63f, 0.35f) : new Color(0.39f, 0.69f, 0.69f);
        childCount.color = isSelected ? new Color(0.89f, 0.76f, 0.58f) : new Color(0.33f, 0.53f, 0.55f);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        ApplyStyle(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ApplyStyle(false);
    }

    public void OnSelect(BaseEventData eventData)
    {
        ApplyStyle(true);
    }

    public void OnDeselect(BaseEventData eventData)
    {
        ApplyStyle(false);
    }
}

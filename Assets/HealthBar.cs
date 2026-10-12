using UnityEngine;

public class HealthBar : MonoBehaviour
{
    [SerializeField] Character character;
    [SerializeField] RectTransform fill;
    float displayedHealth = -1f;

    void OnEnable()
    {
        if (character != null)
            character.HealthChanged += Refresh;

        Refresh(character != null ? character.Health : Character.MaxHealth);
    }

    void OnDisable()
    {
        if (character != null)
            character.HealthChanged -= Refresh;
    }

    void OnRectTransformDimensionsChange()
    {
        Refresh(character != null ? character.Health : Character.MaxHealth);
    }

    void LateUpdate()
    {
        // Inspector edits bypass SetHealth, so keep those visible during Play Mode.
        if (character != null && !Mathf.Approximately(displayedHealth, character.Health))
            Refresh(character.Health);
    }

    void Refresh(float health)
    {
        if (fill == null)
            return;

        // Stretch adds sizeDelta to the parent's size. An old explicit width
        // would therefore make the fill wider than its mask.
        fill.anchorMin = Vector2.zero;
        fill.anchorMax = Vector2.one;
        fill.pivot = new Vector2(0.5f, 0.5f);
        fill.sizeDelta = Vector2.zero;
        fill.localScale = Vector3.one;

        float fraction = Mathf.Clamp01(health / Character.MaxHealth);
        float width = ((RectTransform)transform).rect.width;
        fill.anchoredPosition = new Vector2(-width * (1f - fraction), 0f);
        displayedHealth = health;
    }
}

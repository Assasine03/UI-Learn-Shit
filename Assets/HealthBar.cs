using UnityEngine;

public class HealthBar : MonoBehaviour
{
    [SerializeField] Character character;
    [SerializeField] RectTransform fill;
    float displayedHealth = -1f;
    float displayedWidth = -1f;

    void OnEnable()
    {
        character.HealthChanged += Refresh;

        Refresh(character.Health);
    }

    void OnDisable()
    {
        character.HealthChanged -= Refresh;
    }

    void LateUpdate()
    {
        // Inspector edits bypass SetHealth, so keep those visible during Play Mode.
        if (!Mathf.Approximately(displayedHealth, character.Health) ||
            !Mathf.Approximately(displayedWidth, ((RectTransform)transform).rect.width))
        {
            Refresh(character.Health);
        }
    }

    void Refresh(float health)
    {
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
        displayedWidth = width;
    }
}

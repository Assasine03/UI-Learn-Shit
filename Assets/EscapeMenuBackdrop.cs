using UnityEngine;
using UnityEngine.UI;

// The renderer blurs the live camera; this component only controls the overlay.
public class EscapeMenuBackdrop : MonoBehaviour
{
    [SerializeField] GameObject menuCanvas;
    [SerializeField, Range(0f, 1f)] float darkness = 0.4f;
    public static bool BlurActive { get; private set; }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void ResetState() => BlurActive = false;

    public void Show()
    {
        Image overlay = menuCanvas.GetComponent<Image>();
        if (overlay != null)
            overlay.color = new Color(0f, 0f, 0f, darkness);
        BlurActive = true;
        menuCanvas.SetActive(true);
    }

    public void Hide() => BlurActive = false;
    void OnDisable() => Hide();
}

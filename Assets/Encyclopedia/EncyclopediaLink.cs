using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class EncyclopediaLink : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] TMP_Text text;
    [SerializeField] EncyclopediaController encyclopedia;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
        {
            return;
        }
        int link = TMP_TextUtilities.FindIntersectingLink(text, eventData.position, eventData.pressEventCamera);
        if (link >= 0)
        {
            encyclopedia.OpenArticle(text.textInfo.linkInfo[link].GetLinkID());
        }
    }
}

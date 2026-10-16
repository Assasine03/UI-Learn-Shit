using System.Collections.Generic;
using TMPro;
using UnityEngine;

[CreateAssetMenu(menuName = "Encyclopedia/Eintrag", fileName = "Neuer Eintrag")]
public class EncyclopediaEntry : ScriptableObject
{
    [Tooltip("Eindeutige, dauerhafte Adresse, z.B. hunter-drone. Nicht den Titel als ID verwenden.")]
    [SerializeField] string id = "";
    [SerializeField] string title = "";
    [Tooltip("Artikeltext mit Formatierung. Titel und Suchbegriffe anderer Einträge werden automatisch verlinkt.")]
    [SerializeField, TextArea(8, 30)] string body = "";
    [Tooltip("Weitere Schreibweisen, die im Text zu diesem Artikel verlinken.")]
    [SerializeField] List<string> keywords = new List<string>();
    [Tooltip("Artikel oder Unterkategorien in der gewünschten Reihenfolge. Leer = einzelner Artikel.")]
    [SerializeField] List<EncyclopediaEntry> children = new List<EncyclopediaEntry>();
    [Tooltip("Schriften für einzelne Textstellen. Wird von der Formatierungsleiste ergänzt.")]
    [SerializeField] List<TMP_FontAsset> fonts = new List<TMP_FontAsset>();

    public string Id => id;
    public string Title => title;
    public string Body => body;
    public IReadOnlyList<string> Keywords => keywords;
    public IReadOnlyList<EncyclopediaEntry> Children => children;
    public IReadOnlyList<TMP_FontAsset> Fonts => fonts;
}

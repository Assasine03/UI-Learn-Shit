using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

// Content rules and link generation are independent of the UI hierarchy.
public class EncyclopediaIndex
{
    readonly Dictionary<string, EncyclopediaEntry> entries = new Dictionary<string, EncyclopediaEntry>();
    readonly Dictionary<EncyclopediaEntry, List<EncyclopediaEntry>> paths = new Dictionary<EncyclopediaEntry, List<EncyclopediaEntry>>();
    readonly Dictionary<string, EncyclopediaEntry> terms = new Dictionary<string, EncyclopediaEntry>(StringComparer.OrdinalIgnoreCase);
    readonly Regex links;

    public IEnumerable<EncyclopediaEntry> Entries => entries.Values;

    public EncyclopediaIndex(EncyclopediaEntry root)
    {
        Add(root, new List<EncyclopediaEntry>());
        string alternatives = string.Join("|", terms.Keys.OrderByDescending(term => term.Length).Select(Regex.Escape));
        links = new Regex(@"(?<![\p{L}\p{N}_])(?:" + alternatives + @")(?![\p{L}\p{N}_])",
            RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
    }

    void Add(EncyclopediaEntry entry, List<EncyclopediaEntry> ancestors)
    {
        // Duplicate IDs, repeated children and cycles are configuration errors.
        if (string.IsNullOrWhiteSpace(entry.Id) || string.IsNullOrWhiteSpace(entry.Title))
        {
            throw new ArgumentException("Encyclopedia entries require an ID and a title: " + entry.name);
        }
        if (!Regex.IsMatch(entry.Id, @"^[a-zA-Z0-9_-]+$"))
        {
            throw new ArgumentException("Encyclopedia ID may only contain letters, digits, '-' and '_': " + entry.Id);
        }
        entries.Add(entry.Id, entry);
        var path = new List<EncyclopediaEntry>(ancestors) { entry };
        paths.Add(entry, path);
        AddTerm(entry.Title, entry);
        foreach (string keyword in entry.Keywords)
        {
            AddTerm(keyword, entry);
        }
        foreach (EncyclopediaEntry child in entry.Children)
        {
            Add(child, path);
        }
    }

    void AddTerm(string term, EncyclopediaEntry entry)
    {
        if (string.IsNullOrWhiteSpace(term))
        {
            throw new ArgumentException("Empty encyclopedia keyword: " + entry.Id);
        }
        if (terms.TryGetValue(term, out EncyclopediaEntry existing))
        {
            if (existing != entry)
            {
                throw new ArgumentException("Ambiguous encyclopedia title or keyword: " + term);
            }
            return;
        }
        terms.Add(term, entry);
    }

    public EncyclopediaEntry Find(string id) => entries[id];
    public IReadOnlyList<EncyclopediaEntry> PathTo(EncyclopediaEntry entry) => paths[entry];

    public string ArticleText(EncyclopediaEntry current)
    {
        // Search visible words, never tag names/attributes. Map matches back to
        // formatted text, including phrases split by a bold/size/font tag.
        EncyclopediaRichText.Parsed parsed = EncyclopediaRichText.Parse(current.Body);
        var result = new System.Text.StringBuilder(parsed.FormattedText);
        MatchCollection matches = links.Matches(parsed.VisibleText);
        for (int i = matches.Count - 1; i >= 0; i--)
        {
            Match match = matches[i];
            EncyclopediaEntry target = terms[match.Value];
            if (target == current)
            {
                continue;
            }
            bool customColor = false;
            for (int character = match.Index; character < match.Index + match.Length; character++)
            {
                customColor |= parsed.ExplicitColor[character];
            }
            int start = parsed.CharacterOffsets[match.Index];
            int end = parsed.CharacterOffsets[match.Index + match.Length - 1] + 1;
            string opening = "<link=\"" + target.Id + "\"><u>" + (customColor ? "" : "<color=#FFD34D>");
            string closing = (customColor ? "" : "</color>") + "</u></link>";
            result.Insert(end, closing);
            result.Insert(start, opening);
        }
        if (current.Children.Count > 0)
        {
            result.Append("\n\n<b>In dieser Kategorie</b>\n");
            foreach (EncyclopediaEntry child in current.Children)
            {
                result.Append("\n• " + Link(child, child.Title));
            }
        }
        return result.ToString();
    }

    public string Breadcrumb(EncyclopediaEntry entry)
    {
        return string.Join("  >  ", paths[entry].Select(part => Link(part, part.Title)));
    }

    public static string Link(EncyclopediaEntry entry, string label)
    {
        return "<link=\"" + entry.Id + "\"><color=#FFD34D><u>" + Plain(label) + "</u></color></link>";
    }

    // Designer text cannot accidentally inject TMP markup or nested links.
    public static string Plain(string text) => text.Replace("<", "‹").Replace(">", "›");
}

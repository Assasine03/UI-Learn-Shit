using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

// Only the formatting supported by the article editor is accepted in designer text.
public static class EncyclopediaRichText
{
    public sealed class Parsed
    {
        public string VisibleText;
        public string FormattedText;
        public readonly List<int> CharacterOffsets = new List<int>();
        public readonly List<bool> ExplicitColor = new List<bool>();
    }

    public static Parsed Parse(string source)
    {
        var result = new Parsed();
        var formatted = new StringBuilder();
        var visible = new StringBuilder();
        var stack = new Stack<string>();
        int colorDepth = 0;
        int position = 0;
        foreach (Match token in Regex.Matches(source, "<[^>]*>"))
        {
            AppendText(source.Substring(position, token.Index - position), result, formatted, visible, colorDepth);
            Match tag = Regex.Match(token.Value,
                @"^<(?<closing>/)?(?<name>b|i|u|s|size|color|font)(?:=(?<value>[^<>]+))?>$", RegexOptions.IgnoreCase);
            if (!tag.Success)
            {
                AppendText(token.Value, result, formatted, visible, colorDepth);
            }
            else
            {
                string name = tag.Groups["name"].Value.ToLowerInvariant();
                bool closing = tag.Groups["closing"].Success;
                string value = tag.Groups["value"].Value;
                ValidateValue(name, value, closing);
                if (closing)
                {
                    if (stack.Count == 0 || stack.Pop() != name)
                    {
                        throw new FormatException("Formatierung ist falsch verschachtelt: " + token.Value);
                    }
                    if (name == "color")
                    {
                        colorDepth--;
                    }
                }
                else
                {
                    stack.Push(name);
                    if (name == "color")
                    {
                        colorDepth++;
                    }
                }
                formatted.Append("<" + (closing ? "/" : "") + name + (value.Length > 0 ? "=" + value : "") + ">");
            }
            position = token.Index + token.Length;
        }
        AppendText(source.Substring(position), result, formatted, visible, colorDepth);
        if (stack.Count > 0)
        {
            throw new FormatException("Formatierung nicht geschlossen: <" + stack.Peek() + ">");
        }
        result.VisibleText = visible.ToString();
        result.FormattedText = formatted.ToString();
        return result;
    }

    static void AppendText(string text, Parsed result, StringBuilder formatted, StringBuilder visible, int colorDepth)
    {
        foreach (char character in text)
        {
            char display = character == '<' ? '‹' : (character == '>' ? '›' : character);
            result.CharacterOffsets.Add(formatted.Length);
            result.ExplicitColor.Add(colorDepth > 0);
            formatted.Append(display);
            visible.Append(display);
        }
    }

    static void ValidateValue(string name, string value, bool closing)
    {
        bool parameter = name == "size" || name == "color" || name == "font";
        if (closing || !parameter)
        {
            if (value.Length > 0)
            {
                throw new FormatException("Dieser Tag darf keinen Wert enthalten: " + name);
            }
            return;
        }
        string unquoted = value.Trim('"');
        bool valid = name == "size" ? Regex.IsMatch(unquoted, @"^[0-9]+(?:\.[0-9]+)?%?$") :
            (name == "color" ? Regex.IsMatch(unquoted, @"^(?:#[0-9a-fA-F]{6}(?:[0-9a-fA-F]{2})?|[a-zA-Z]+)$") :
                Regex.IsMatch(value, "^\"[^\"<>]+\"$"));
        if (!valid)
        {
            throw new FormatException("Ungültiger Wert für <" + name + ">: " + value);
        }
    }
}

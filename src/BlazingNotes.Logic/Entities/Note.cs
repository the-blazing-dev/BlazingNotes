using System.Text.Encodings.Web;
using System.Text.RegularExpressions;
using BlazingNotes.Logic.Search;

namespace BlazingNotes.Logic.Entities;

public class Note
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Text { get; set; } = "";

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ModifiedAt { get; set; }
    public DateTime? RelevantAt { get; set; }
    public DateTime? ArchivedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
    public DateTime? HiddenUntil { get; set; }

    public ICollection<string> GetTags()
    {
        return Constants.TagRegex.Matches(Text).Select(x => x.Value).ToList();
    }

    public string GetTextWithTagsMarked()
    {
        var result = Constants.TagRegex.Replace(Text, x => $"<bz-tag>{x.Value}</bz-tag>");
        return result;
    }

    public bool IsHidden()
    {
        // use DateTime.Now (and not UtcNow) because the user wants it to be hidden from his point of view
        // we could use TimeProvider somewhen
        return HiddenUntil.HasValue && HiddenUntil.Value > DateTime.Now;
    }

    public string GetXssSafeText(BzSearchQuery? search)
    {
        // use ASCII DC1-DC4 as special character the user hopefully never searches for
        // this way we fix issues when users are searching for "tag" or "search" which occurs in the markup
        // an alternative would be to search for tags and "user search" at the same time and hop through the matches simultaneously
        // maybe using a StringBuilder
        var dcSearchStart = (char)17; // Device Control 1 (DC1) or XON
        var dcSearchEnd = (char)18; // Device Control 2 (DC2)
        var dcTagStart = (char)19; // Device Control 3 (DC3) or XOFF
        var dcTagEnd = (char)20; // Device Control 4 (DC4)

        var xmlSearchStart = "<bz-search>";
        var xmlSearchEnd = "</bz-search>";
        var xmlTagStart = "<bz-tag>";
        var xmlTagEnd = "</bz-tag>";

        var result = Text;
        var htmlEncoder = HtmlEncoder.Default;
        result = htmlEncoder.Encode(result);

        if (search != null)
        {
            foreach (var term in search.Value.RequiredTerms)
            {
                var encodedTerm = htmlEncoder.Encode(term);
                var termRegex = Regex.Escape(encodedTerm);
                result = Regex.Replace(result, termRegex,
                    x => $"{dcSearchStart}{x.Value}{dcSearchEnd}", RegexOptions.IgnoreCase);
            }
        }

        result = Constants.TagRegex.Replace(result, x => $"{dcTagStart}{x.Value}{dcTagEnd}");

        result = result.Replace(dcSearchStart.ToString(), xmlSearchStart);
        result = result.Replace(dcSearchEnd.ToString(), xmlSearchEnd);
        result = result.Replace(dcTagStart.ToString(), xmlTagStart);
        result = result.Replace(dcTagEnd.ToString(), xmlTagEnd);

        return result;
    }
}
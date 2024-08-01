namespace BlazingNotes.Logic.Entities;

public class Note
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public required string Text { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ModifiedAt { get; set; }
    public DateTime? RelevantAt { get; set; }
    public DateTime? ArchivedAt { get; set; }
    public DateTime? DeletedAt { get; set; }

    public string GetTextWithTagsMarked()
    {
        var result = Constants.TagRegex.Replace(Text, x => $"<bz-tag>{x.Value}</bz-tag>");
        return result;
    }

    public ICollection<string> GetTags()
    {
        return Constants.TagRegex.Matches(Text).Select(x => x.Value).ToList();
    }
}
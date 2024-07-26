using System.Text.RegularExpressions;

namespace BlazingNotes.Logic.State;

[FeatureState]
public record AppState
{
    public required ImmutableList<Note> Notes { get; init; } = ImmutableList<Note>.Empty;
    public Note? CurrentlyEditingNote { get; init; }
    public bool ShowCreateNoteDialog { get; set; }

    public ICollection<Note> GetHomePageNotes()
    {
        return Notes.Where(x => !x.IsArchived)
            .OrderByDescending(x => x.CreatedAt)
            .Take(10)
            .ToList();
    }

    public ICollection<Note> GetArchivedNotes()
    {
        return Notes.Where(x => x.IsArchived)
            .OrderByDescending(x => x.ArchivedAt)
            .ToList();
    }

    public HashSet<string> GetTags()
    {
        // a short test in LinqPad resulted in 10ms for 10_000 notes which is fast enough for now
        // of course we need some better implementation / caching of notes as soon as the app is used more frequently
        var result = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var note in Notes)
        {
            var foundTags = Constants.TagRegex.Matches(note.Text);
            foreach (Match foundTag in foundTags) result.Add(foundTag.Value);
        }

        return result;
    }
}
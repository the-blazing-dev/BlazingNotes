namespace BlazingNotes.Logic.Services;

public interface IDownloadFileService
{
    public Task ProvideFileAsync(Stream stream, string fileName);
}
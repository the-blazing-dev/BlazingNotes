using BlazingNotes.Logic.Services;

namespace BlazingNotes.Logic.Tests.TestSupport;

public class NoopDownloadFileService : IDownloadFileService
{
    public Task ProvideFileAsync(Stream stream, string fileName)
    {
        return Task.CompletedTask;
    }
}
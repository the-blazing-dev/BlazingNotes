using BlazingNotes.Logic.Services;
using Microsoft.JSInterop;

namespace BlazingNotes.UI.Services;

public class DownloadFileService(IJSRuntime js) : IDownloadFileService
{
    public async Task ProvideFileAsync(Stream stream, string fileName)
    {
        using var streamRef = new DotNetStreamReference(stream);
        await js.InvokeVoidAsync("downloadFileFromStream", fileName, streamRef);
    }
}
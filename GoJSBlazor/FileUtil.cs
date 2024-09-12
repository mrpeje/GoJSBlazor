using Microsoft.JSInterop;

public static class FileUtil
{
    public async static Task SaveAs(IJSRuntime js, string filename, byte[] data)
    {
        await js.InvokeAsync<object>(
            "saveAsFile",
            filename,
            Convert.ToBase64String(data));
    }
    public async static Task<byte[]> LoadFile(IJSRuntime js)
    {
        return await js.InvokeAsync<byte[]>("loadFile");
    }
}
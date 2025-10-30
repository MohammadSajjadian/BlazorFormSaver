using Microsoft.JSInterop;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace MSJD.FormSaver;

internal class FormSaver : IFormSaver, IAsyncDisposable
{
    private readonly IJSRuntime _js;
    private IJSObjectReference? _localStorageModule;
    private IJSObjectReference? _encryptionModule;
    public FormSaver(IJSRuntime js)
    {
        _js = js;
    }

    public async Task InitializeAsync()
    {
        _localStorageModule ??= await _js.InvokeAsync<IJSObjectReference>("import", "./_content/MSJD.FormSaver/localStorage.js");

        _encryptionModule ??= await _js.InvokeAsync<IJSObjectReference>("import", "./_content/MSJD.FormSaver/encryptionHelper.js");

        await _encryptionModule.InvokeVoidAsync("initKey");
    }

    public async Task<bool> SaveAsync<TModel>(string key, TModel model) where TModel : class
    {
        try
        {
            var json = JsonSerializer.Serialize(model);
            var encrypted = await _encryptionModule!.InvokeAsync<string>("encryptData", json);
            return await _localStorageModule!.InvokeAsync<bool>("saveItem", key, encrypted);
        }
        catch
        {
            return false;
        }
    }

    public async Task<TModel?> LoadAsync<TModel>(string key)
    {
        try
        {
            var encrypted = await _localStorageModule!.InvokeAsync<string?>("loadItem", key);
            if (string.IsNullOrEmpty(encrypted)) return default;

            var decrypted = await _encryptionModule!.InvokeAsync<string?>("decryptData", encrypted);
            return string.IsNullOrEmpty(decrypted) ? default : JsonSerializer.Deserialize<TModel>(decrypted);
        }
        catch
        {
            return default;
        }
    }

    public async Task<bool> RemoveAsync(string key)
    {
        try
        {
            return await _localStorageModule!.InvokeAsync<bool>("removeItem", key);
        }
        catch
        {
            return false;
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_localStorageModule is not null)
            await _localStorageModule.DisposeAsync();

        if (_encryptionModule is not null)
            await _encryptionModule.DisposeAsync();
    }
}
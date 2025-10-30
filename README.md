# FormSaver for Blazor

[![NuGet](https://img.shields.io/nuget/v/MSJD.FormSaver.svg?style=flat-square)](https://www.nuget.org/packages/MSJD.FormSaver/)

**Advanced FormSaver Demo | LocalStorage + Encryption**

FormSaver is a Blazor service that enables automatic saving, restoring, and encryption of complex form data in the browser's local storage. It supports nested objects, offline draft saving, and secure storage without requiring a backend.

---

## Features

- 💾 **Form Saving** – Save form state locally using a custom button click.  
- 🔄 **Restore Forms Instantly** – Load previously saved form data anytime.  
- 🔐 **Built-in Encryption** – All data stored locally is encrypted for security.  
- 🏗 **Supports Nested Objects** – Handles complex forms with nested structures.  
- 🌐 **Offline Friendly** – No backend required; perfect for offline forms or drafts.  
- 🧰 **Blazor Integration** – Works seamlessly with `EditForm`, `InputText`, `InputSelect`, `InputCheckbox`, and `InputDate`.  
- ⚡ **Compatible with All Render Modes** – Works with `InteractiveServer`, `InteractiveWebAssembly`, and standard Blazor server/client render modes.  

---

## Installation

Install the package via NuGet:

```.NET CLI
dotnet add package MSJD.FormSaver --version 1.0.5
```

## Demo Preview
Try it live: [FormSaver Demo](https://mohammadsajjadian.ir/demo/form-saver)

---

## Simple Example

Inject the service in `Program.cs`:

```csharp
builder.Services.AddFormSaver();
```
Razor component example:
```razor
@page "/simple-form"
@inject IFormSaver FormSaver

<EditForm Model="@User">
    <DataAnnotationsValidator />
    <ValidationSummary />

    <label>Full Name:</label>
    <InputText @bind-Value="User.FullName" class="form-control" />

    <label>Email:</label>
    <InputText @bind-Value="User.Email" class="form-control" />

    <label>Comment:</label>
    <InputTextArea @bind-Value="User.Comment" class="form-control" rows="3" />

    <div class="mt-2">
        <button type="button" class="btn btn-primary" @onclick="SaveForm">💾 Save</button>
        <button type="button" class="btn btn-secondary" @onclick="LoadForm">📂 Load</button>
        <button type="button" class="btn btn-danger" @onclick="DeleteForm">🗑 Delete</button>
    </div>
</EditForm>

@code {
    private string Key = "simpleForm";
    private UserModel User = new();

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await FormSaver.InitializeAsync();
            var saved = await FormSaver.LoadAsync<UserModel>(Key);
            if (saved != null) User = saved;
            StateHasChanged();
        }
    }

    private async Task SaveForm() => await FormSaver.SaveAsync(Key, User);
    private async Task LoadForm() => User = await FormSaver.LoadAsync<UserModel>(Key) ?? new UserModel();
    private async Task DeleteForm() => await FormSaver.RemoveAsync(Key);

    public class UserModel
    {
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Comment { get; set; } = string.Empty;
    }
}
```

## API

### Initialize FormSaver
```csharp
await FormSaver.InitializeAsync();
```
### Save Form
```csharp
bool success = await FormSaver.SaveAsync("formKey", User);
```
### Load Form
```csharp
var model = await FormSaver.LoadAsync<UserModel>("formKey");
```
### Delete Form
```csharp
bool success = await FormSaver.RemoveAsync("formKey");
```
## Important Note

FormSaver stores **form data in browser local storage**.  
Do **not** try to save forms that include file uploads (images, PDFs, etc.), because files cannot be stored in local storage.  
If your form contains files, exclude them or handle file uploads separately.

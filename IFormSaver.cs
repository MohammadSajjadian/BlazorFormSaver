using System.Threading.Tasks;

namespace MSJD.FormSaver;

public interface IFormSaver
{
    Task InitializeAsync();
    Task<bool> SaveAsync<TModel>(string key, TModel model) where TModel : class;
    Task<TModel?> LoadAsync<TModel>(string key);
    Task<bool> RemoveAsync(string key);
}

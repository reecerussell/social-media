using Core.Models;

namespace Core
{
    public interface IContext
    {
        void SetValue(string key, object value);
        T GetValue<T>(string key);
        object GetValue(string key);
        string GetUserId();
    }
}

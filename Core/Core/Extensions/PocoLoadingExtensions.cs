using System;
using System.Runtime.CompilerServices;

namespace Core.Extensions
{
    /// <summary>
    /// An extensions class providing a method used to lazy load related entity data.
    /// https://docs.microsoft.com/en-us/ef/core/querying/related-data
    /// </summary>
    public static class PocoLoadingExtensions
    {
        public static TRelated Load<TRelated>(
            this Action<object, string> loader,
            object entity,
            ref TRelated navigationField,
            [CallerMemberName] string navigationName = null) 
            where TRelated : class
        {
            loader?.Invoke(entity, navigationName);
            return navigationField;
        }
    }
}

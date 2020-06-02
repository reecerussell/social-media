using Microsoft.Extensions.Logging;
using System;

namespace SocialMedia.Core.Logging
{
    public class Logger : ILogger
    {
        private readonly Func<string, bool> _sender;
        private readonly string _categoryName;

        public Logger(string categoryName, Func<string, bool> sender)
        {
            _sender = sender;
            _categoryName = categoryName;
        }

        public void Log<TState>(
            LogLevel logLevel, 
            EventId eventId, 
            TState state, 
            Exception exception, 
            Func<TState, Exception, string> formatter)
        {
            var message = $"--- {_categoryName} ---\nDate: {DateTime.UtcNow:u}\nLog Level: {logLevel.ToString()}\nEvent Id: {eventId.Id}, {eventId.Name}\n\n{formatter(state, exception)}";
            
            _sender.Invoke(message);
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            // TODO: do something with this.
            return true;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }
    }
}

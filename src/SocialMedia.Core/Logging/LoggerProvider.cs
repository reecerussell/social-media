using Amazon.SQS;
using Amazon.SQS.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Net;
using System.Text;
using MD5 = System.Security.Cryptography.MD5;

namespace SocialMedia.Core.Logging
{
    public class LoggerProvider : ILoggerProvider
    {
        private readonly IConfiguration _configuration;
        private readonly IAmazonSQS _queueClient;
        private readonly ConcurrentDictionary<string, Logger> _loggers;

        public LoggerProvider(
            IConfiguration configuration,
            IAmazonSQS queueClient)
        {
            _configuration = configuration;
            _queueClient = queueClient;
            _loggers = new ConcurrentDictionary<string, Logger>();
        }

        public ILogger CreateLogger(string categoryName)
        {
            var key = $"{DateTime.UtcNow:d-M-yyyy}";
            if (_loggers.ContainsKey(key))
            {
                return _loggers[key];
            }

            var url = _configuration[Constants.LogQueueUrl];
            if (string.IsNullOrEmpty(url)) throw new ArgumentException("log queue url is not configured");

            var logger = new Logger(categoryName, msg => 
                _queueClient.SendMessageAsync(new SendMessageRequest
                {
                    QueueUrl = url,
                    MessageBody = msg,
                    MessageGroupId = categoryName,
                    MessageDeduplicationId = Guid.NewGuid().ToString()
                }).Result.HttpStatusCode == HttpStatusCode.OK);

            _loggers[key] = logger;
            return logger;
        }

        public void Dispose()
        {
            _loggers.Clear();
        }
    }
}

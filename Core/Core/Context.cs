using Amazon.Lambda.APIGatewayEvents;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace Core
{
    public class Context : IContext, IDisposable
    {
        private IDictionary<string, string> _stageVariables;
        private IDictionary<string, string> _pathParameters;
        private IDictionary<string, string> _queueStrings;
        private IDictionary<string, string> _headers;
        private IDictionary<string, object> _values;

        private bool _disposed = false;

        public Context(APIGatewayProxyRequest request, bool parseAuthPayload = false)
        {
            _stageVariables = new Dictionary<string, string>();
            _pathParameters = new Dictionary<string, string>();
            _queueStrings = new Dictionary<string, string>();
            _headers = new Dictionary<string, string>();
            _values = new Dictionary<string, object>();

            if (request.StageVariables != null)
            {
                foreach (var (key, value) in request.StageVariables)
                {
                    _stageVariables[key] = value;
                }
            }

            if (request.PathParameters != null)
            {
                foreach (var (key, value) in request.PathParameters)
                {
                    _pathParameters[key] = value;
                }
            }

            if (request.QueryStringParameters != null)
            {
                foreach (var (key, value) in request.QueryStringParameters)
                {
                    _queueStrings[key] = value;
                }
            }

            if (request.Headers != null)
            {
                foreach (var (key, value) in request.Headers)
                {
                    _headers[key] = value;
                }
            }

            if (parseAuthPayload)
            {
                ParseAuthPayload();
            }
        }

        private void ParseAuthPayload()
        {
            if (!_headers.TryGetValue("Authorization", out var token))
            {
                return;
            }

            if (!token.StartsWith("Bearer "))
            {
                return;
            }

            var parts = token[7..].Split('.');
            if (parts.Length != 3)
            {
                return;
            }

            var mod4 = parts[1].Length % 4;
            if (mod4 > 0)
            {
                parts[1] += new string('=', 4 - mod4);
            }

            var payload = JsonConvert.DeserializeObject<IDictionary<string, object>>(
                Encoding.UTF8.GetString(Convert.FromBase64String(parts[1])));

            foreach (var (key, value) in payload)
            {
                _values[key] = value;
            }
        }

        public void SetValue(string key, object value)
        {
            if (_disposed)
            {
                throw new ObjectDisposedException("Context has been disposed.");
            }

            _values[key] = value;
        }

        public T GetValue<T>(string key)
        {
            if (_disposed)
            {
                throw new ObjectDisposedException("Context has been disposed.");
            }

            if (_values.TryGetValue(key, out var value))
            {
                return (T)value;
            }

            if (_pathParameters.TryGetValue(key, out var value2))
            {
                return (T) Convert.ChangeType(value2, typeof(T));
            }

            if (_queueStrings.TryGetValue(key, out var value3))
            {
                return (T)Convert.ChangeType(value3, typeof(T));
            }

            if (_stageVariables.TryGetValue(key, out var value4))
            {
                return (T)Convert.ChangeType(value4, typeof(T));
            }

            if (_headers.TryGetValue(key, out var value5))
            {
                return (T)Convert.ChangeType(value5, typeof(T));
            }

            if (typeof(T) == typeof(string))
            {
                return (T)Convert.ChangeType(Environment.GetEnvironmentVariable(key), typeof(T));
            }

            return default;
        }

        public object GetValue(string key)
        {
            if (_disposed)
            {
                throw new ObjectDisposedException("Context has been disposed.");
            }

            if (_values.TryGetValue(key, out var value))
            {
                return value;
            }

            if (_pathParameters.TryGetValue(key, out var value2))
            {
                return value2;
            }

            if (_queueStrings.TryGetValue(key, out var value3))
            {
                return value3;
            }

            if (_stageVariables.TryGetValue(key, out var value4))
            {
                return value4;
            }

            if (_headers.TryGetValue(key, out var value5))
            {
                return value5;
            }

            return Environment.GetEnvironmentVariable(key);
        }

        public string GetUserId()
        {
            return GetValue<string>("user_id");
        }

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;

            _stageVariables = null;
            _pathParameters = null;
            _queueStrings = null;
            _headers = null;
            _values = null;
        }
    }
}

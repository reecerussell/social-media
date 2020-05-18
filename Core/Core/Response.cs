using Amazon.Lambda.APIGatewayEvents;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;

namespace Core
{
    public static class Response
    {
        public static APIGatewayProxyResponse Ok(this APIGatewayProxyRequest request, IContext context, object data)
        {
            return Build(context, request, HttpStatusCode.OK, data);
        }

        public static APIGatewayProxyResponse BadRequest(this APIGatewayProxyRequest request, IContext context, string error)
        {
            IDictionary<string, string> payload = null;
            if (!string.IsNullOrEmpty(error))
            {
                payload = new Dictionary<string, string>
                {
                    {"message", error }
                };
            }

            return Build(context, request, HttpStatusCode.BadRequest, payload);
        }

        public static APIGatewayProxyResponse Error(this APIGatewayProxyRequest request, IContext context, Exception exception)
        {
            IDictionary<string, object> payload = null;
            if (exception != null)
            {
                payload = new Dictionary<string, object>
                {
                    {"message", exception.Message }
                };

                var exceptionMode = context.GetValue<string>("EXCEPTION_MODE") ?? string.Empty;
                if (exceptionMode.Equals("detailed", StringComparison.InvariantCultureIgnoreCase))
                {
                    payload.Add("stack", exception.StackTrace);
                    payload.Add("source", exception.StackTrace);

                    if (exception.InnerException != null)
                    {
                        payload.Add("innerException", new Dictionary<string, string>
                        {
                            {"message", exception.InnerException.Message },
                            {"stack", exception.InnerException.StackTrace },
                            {"source", exception.InnerException.Source }
                        });
                    }
                }
            }

            return Build(context, request, HttpStatusCode.InternalServerError, payload);
        }

        private static APIGatewayProxyResponse Build(IContext context, APIGatewayProxyRequest request, HttpStatusCode status, object body)
        {
            var corsOrigin = context.GetValue<string>("CORS_ORIGIN");
            corsOrigin = string.IsNullOrEmpty(corsOrigin) ? "*" : corsOrigin;
            var corsHeaders = context.GetValue<string>("CORS_HEADERS");
            corsHeaders = string.IsNullOrEmpty(corsHeaders) ? "Content-Type,Authorization" : corsHeaders;

            string json = null;
            if (body != null)
            {
                json = JsonConvert.SerializeObject(body);
            }

            return new APIGatewayProxyResponse
            {
                Headers = new Dictionary<string, string>
                {
                    { "Content-Type", "application/json; charset=utf-8" },
                    { "Access-Control-Allow-Origin", corsOrigin },
                    { "Access-Control-Allow-Headers", corsHeaders },
                    { "Access-Control-Allow-Methods", request.HttpMethod },
                },
                StatusCode = (int)status,
                Body = json,
            };
        }
    }
}

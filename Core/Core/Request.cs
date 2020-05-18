using Amazon.Lambda.APIGatewayEvents;
using Newtonsoft.Json;
using System;
using System.Text;

namespace Core
{
    public static class Request
    {
        public static T ParseBody<T>(this APIGatewayProxyRequest request)
        {
            var json = request.Body;

            if (request.IsBase64Encoded)
            {
                var data = Convert.FromBase64String(request.Body);
                json = Encoding.UTF8.GetString(data);
            }

            return JsonConvert.DeserializeObject<T>(json);
        }
    }
}

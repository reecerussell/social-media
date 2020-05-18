using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Core;
using Core.Dtos;
using CSharpFunctionalExtensions;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace RegisterUser
{
    public class Functions
    {
        private readonly IUserManager _userManager;
        
        public Functions()
        {
            var services = Container.Build();
            _userManager = (IUserManager)services.GetService(typeof(IUserManager));
        }

        public async Task<APIGatewayProxyResponse> RegisterUser(APIGatewayProxyRequest request, ILambdaContext lambdaContext)
        {
            using var context = new Context(request);

            RegisterUserDto dto;
            try
            {
                dto = request.ParseBody<RegisterUserDto>();
            }
            catch (JsonSerializationException)
            {
                return request.BadRequest(context, "invalid request body");
            }

            try
            {
                var (success, _, error) = await _userManager.RegisterAsync(dto);
                if (!success)
                {
                    return request.BadRequest(context, error);
                }

                return request.Ok(context, null);
            }
            catch (Exception e)
            {
                return request.Error(context, e);
            }
        }
    }
}

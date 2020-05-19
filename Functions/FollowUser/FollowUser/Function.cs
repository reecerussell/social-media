using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Core;
using CSharpFunctionalExtensions;
using System;
using System.Threading.Tasks;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace FollowUser
{
    public class Functions
    {
        private readonly IUserManager _userManager;

        public Functions()
        {
            var services = Container.Build();
            _userManager = (IUserManager)services.GetService(typeof(IUserManager));
        }

        public async Task<APIGatewayProxyResponse> FollowUser(APIGatewayProxyRequest request, ILambdaContext lambdaContext)
        {
            using var context = new Context(request, true);

            try
            {
                var (success, _, error) = await _userManager.FollowAsync(context, context.GetValue<string>("id"));
                if (!success)
                {
                    if (CommonErrors.IsNotFound(error))
                    {
                        return request.NotFound(context, error);
                    }

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

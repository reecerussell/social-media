using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Core;
using Core.Dtos;
using CSharpFunctionalExtensions;
using System;
using System.Threading.Tasks;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace ChangeProfilePicture
{
    public class Functions
    {
        private readonly IUserManager _userManager;

        public Functions()
        {
            _userManager = (IUserManager) Container.Build().GetService(typeof(IUserManager));
        }

        public async Task<APIGatewayProxyResponse> ChangeProfilePicture(APIGatewayProxyRequest request,
            ILambdaContext lambdaContext)
        {
            using var context = new Context(request, true);

            try
            {
                var (stream, contentType) = request.ParseFile("file");
                var dto = new UpdateMediaDto {ContentType = contentType, File = stream};

                var (success, _, error) = await _userManager.UpdateProfilePictureAsync(context, dto);
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

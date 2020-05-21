using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Core;
using CSharpFunctionalExtensions;
using System;
using System.Threading.Tasks;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace DownloadMedia
{
    public class Functions
    {
        private readonly IMediaDownloader _downloader;

        public Functions()
        {
            _downloader = (IMediaDownloader) Container.Build().GetService(typeof(IMediaDownloader));
        }

        public async Task<APIGatewayProxyResponse> DownloadMedia(APIGatewayProxyRequest request, ILambdaContext lambdaContext)
        {
            using var context = new Context(request);

            try
            {
                var (success, _, media, error) =
                    await _downloader.DownloadAsync(context, context.GetValue<string>("id"));
                if (!success)
                {
                    if (CommonErrors.IsNotFound(error))
                    {
                        return request.NotFound(context, error);
                    }

                    return request.BadRequest(context, error);
                }

                return request.Ok(context, media);
            }
            catch (Exception e)
            {
                return request.Error(context, e);
            }
        }
    }
}

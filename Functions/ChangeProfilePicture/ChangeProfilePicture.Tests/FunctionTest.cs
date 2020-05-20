using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.TestUtilities;
using Core;
using Core.Dtos;
using CSharpFunctionalExtensions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace ChangeProfilePicture.Tests
{
    public class FunctionTest : IDisposable
    {
        private readonly ITestOutputHelper _testOutputHelper;
        private string _userId;

        public FunctionTest(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "test", EnvironmentVariableTarget.Process);
            Environment.SetEnvironmentVariable("EXCEPTION_MODE", "detailed", EnvironmentVariableTarget.Process);
            Environment.SetEnvironmentVariable("AWS_REGION", "eu-west-2", EnvironmentVariableTarget.Process);
            Environment.SetEnvironmentVariable("AWS_PROFILE", "Reece Russell", EnvironmentVariableTarget.Process);

            SetupAsync().Wait();
        }

        private async Task SetupAsync()
        {
            var userManager = (IUserManager)Container.Build().GetService(typeof(IUserManager));

            var (success, _, userId, error) = await userManager.RegisterAsync(new RegisterUserDto
                { Username = "test_user_1", Password = "test_password_1" });
            if (!success)
            {
                throw new Exception(error);
            }

            _userId = userId;
        }

        [Fact]
        public async Task TetGetMethod()
        {
            var context = new TestLambdaContext();
            var request = new APIGatewayProxyRequest
            {
                Headers = MockHeaders(_userId),
                Body = BuildRequestBody(),
                StageVariables = new Dictionary<string, string>
                {
                    { "MEDIA_BUCKET_NAME", "dev-sm-media-1"},
                    {"EXCEPTION_MODE", "detailed" }
                }
            };

            var functions = new Functions();
            var response = await functions.ChangeProfilePicture(request, context);

            if (response.StatusCode != 200)
            {
                _testOutputHelper.WriteLine(response.Body);
            }

            Assert.Equal(200, response.StatusCode);
            Assert.Null(response.Body);
        }

        private static IDictionary<string, string> MockHeaders(string userId)
        {
            var payload = new Dictionary<string, string>
            {
                {"user_id", userId}
            };

            var payloadJson = JsonConvert.SerializeObject(payload);
            var payloadData = Convert.ToBase64String(Encoding.UTF8.GetBytes(payloadJson));

            return new Dictionary<string, string>
            {
                {"Authorization", $"buffer.{payloadData}.buffer"}
            };
        }

        private static string BuildRequestBody()
        {
            // A base64 encoded multipart/form-data request body for test-media.png
            return
                @"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";
        }

        public void Dispose()
        {
            var request = new APIGatewayProxyRequest
            {
                Headers = MockHeaders(_userId),
                StageVariables = new Dictionary<string, string>
                {
                    { "MEDIA_BUCKET_NAME", "dev-sm-media-1"},
                    {"EXCEPTION_MODE", "detailed" }
                }
            };

            var userManager = (IUserManager)Container.Build().GetService(typeof(IUserManager));
            //userManager.DeleteAsync(new Context(request), _userId).Wait();
        }
    }
}

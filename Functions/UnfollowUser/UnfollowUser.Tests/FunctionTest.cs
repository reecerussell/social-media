using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.TestUtilities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Core;
using Core.Dtos;
using CSharpFunctionalExtensions;
using Newtonsoft.Json;
using Xunit;
using Xunit.Abstractions;

namespace UnfollowUser.Tests
{
    public class FunctionTest : IDisposable
    {
        private readonly ITestOutputHelper _testOutputHelper;

        private string _userId;
        private string _followerId;

        public FunctionTest(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;

            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "test", EnvironmentVariableTarget.Process);
            Environment.SetEnvironmentVariable("EXCEPTION_MODE", "detailed", EnvironmentVariableTarget.Process);
            Environment.SetEnvironmentVariable("AWS_REGION", "eu-west-2", EnvironmentVariableTarget.Process);

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

            (success, _, userId, error) = await userManager.RegisterAsync(new RegisterUserDto
            { Username = "test_user_2", Password = "test_password_2" });
            if (!success)
            {
                throw new Exception(error);
            }

            _followerId = userId;

            var request = new APIGatewayProxyRequest {Headers = MockHeaders(_followerId)};
            (success, _, error) = await userManager.FollowAsync(new Context(request, true), _userId);
            if (!success)
            {
                throw new Exception(error);
            }
        }

        [Fact]
        public async Task TestUnfollowUser()
        {
            var context = new TestLambdaContext();
            var request = new APIGatewayProxyRequest
            {
                Headers = MockHeaders(_followerId),
                PathParameters = new Dictionary<string, string> { { "id", _userId } }
            };

            var functions = new Functions();
            var response = await functions.UnfollowUser(request, context);

            _testOutputHelper.WriteLine("Body: " + response.Body);
            
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

        public void Dispose()
        {
            var userManager = (IUserManager)Container.Build().GetService(typeof(IUserManager));
            userManager.DeleteAsync(_userId).Wait();
            userManager.DeleteAsync(_followerId).Wait();
        }
    }
}

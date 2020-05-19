using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.TestUtilities;
using Core;
using Core.Dtos;
using CSharpFunctionalExtensions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace FollowUser.Tests
{
    public class FunctionTest : IDisposable
    {
        private readonly ITestOutputHelper _testOutputHelper;
        private readonly IUserManager _userManager;

        private string _userId;
        private string _followerId;

        public FunctionTest(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;

            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "test", EnvironmentVariableTarget.Process);
            Environment.SetEnvironmentVariable("EXCEPTION_MODE", "detailed", EnvironmentVariableTarget.Process);
            Environment.SetEnvironmentVariable("AWS_REGION", "eu-west-2", EnvironmentVariableTarget.Process);

            _userManager = (IUserManager)Container.Build().GetService(typeof(IUserManager));

            SetupAsync().Wait();
        }

        private async Task SetupAsync()
        {
            var (success, _, userId, error) = await _userManager.RegisterAsync(new RegisterUserDto
                {Username = "test_user_1", Password = "test_password_1"});
            if (!success)
            {
                throw new Exception(error);
            }

            _userId = userId;

            (success, _, userId, error) = await _userManager.RegisterAsync(new RegisterUserDto
                { Username = "test_user_2", Password = "test_password_2" });
            if (!success)
            {
                throw new Exception(error);
            }

            _followerId = userId;
        }

        [Fact]
        public async Task TestFollowUser()
        {
            var context = new TestLambdaContext();
            var request = new APIGatewayProxyRequest
            {
                Headers = MockHeaders(_followerId),
                PathParameters = new Dictionary<string, string> { { "id", _userId} }
            };

            var functions = new Functions();
            var response = await functions.FollowUser(request, context);

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
            _userManager.DeleteAsync(_userId).Wait();
            _userManager.DeleteAsync(_followerId).Wait();
        }
    }
}
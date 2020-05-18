using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.TestUtilities;
using Core.Dtos;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace LoginUser.Tests
{
    public class FunctionTest
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public FunctionTest(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;

            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "test", EnvironmentVariableTarget.Process);
            Environment.SetEnvironmentVariable("EXCEPTION_MODE", "DETAILED", EnvironmentVariableTarget.Process);
            Environment.SetEnvironmentVariable("AWS_REGION", "eu-west-2", EnvironmentVariableTarget.Process);
            Environment.SetEnvironmentVariable("AWS_PROFILE", "Reece Russell", EnvironmentVariableTarget.Process);
        }

        [Fact]
        public async Task TestLoginUser()
        {
            var context = new TestLambdaContext();
            var request = new APIGatewayProxyRequest
            {
                StageVariables = new Dictionary<string, string> {{"AUTH_SECRET_NAME", "dev/SocialMedia/Auth"}}
            };

            var data = new UserCredentialsDto
            {
                Username = "1test_user",
                Password = "MySecurePassword"
            };
            request.Body = JsonConvert.SerializeObject(data);

            var functions = new Functions();
            var response = await functions.LoginUser(request, context);

            _testOutputHelper.WriteLine("Body: " + response.Body);

            Assert.Equal(200, response.StatusCode);
            Assert.NotNull(response.Body);
        }

        [Fact]
        public async Task TestLoginUserWithInvalidCredentials()
        {
            var context = new TestLambdaContext();
            var request = new APIGatewayProxyRequest
            {
                StageVariables = new Dictionary<string, string> { { "AUTH_SECRET_NAME", "dev/SocialMedia/Auth" } }
            };

            var data = new UserCredentialsDto
            {
                Username = "1test_user",
                Password = "random incorrect password"
            };
            request.Body = JsonConvert.SerializeObject(data);

            var functions = new Functions();
            var response = await functions.LoginUser(request, context);

            _testOutputHelper.WriteLine("Body: " + response.Body);

            Assert.Equal(400, response.StatusCode);
            Assert.NotNull(response.Body);
        }
    }
}

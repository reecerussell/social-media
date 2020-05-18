using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.TestUtilities;
using Core.Dtos;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace RegisterUser.Tests
{
    public class FunctionTest
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public FunctionTest(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;

            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "test", EnvironmentVariableTarget.Process);
            Environment.SetEnvironmentVariable("EXCEPTION_MODE", "detailed", EnvironmentVariableTarget.Process);
            Environment.SetEnvironmentVariable("AWS_REGION", "eu-west-2", EnvironmentVariableTarget.Process);
        }

        [Fact]
        public async Task TestRegisterUser()
        {
            var context = new TestLambdaContext();
            var request = new APIGatewayProxyRequest();

            var data = new RegisterUserDto
            {
                Username = "1test_user",
                Password = "MySecurePassword"
            };
            request.Body = JsonConvert.SerializeObject(data);

            var functions = new Functions();
            var response = await functions.RegisterUser(request, context);

            _testOutputHelper.WriteLine("Body: " + response.Body);

            Assert.Equal(200, response.StatusCode);
            Assert.Null(response.Body);
        }
    }
}

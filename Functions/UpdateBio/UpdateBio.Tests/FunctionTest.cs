using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.TestUtilities;
using Core.Dtos;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace UpdateBio.Tests
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

        /// <summary>
        /// This is a specific text for a pre-existing user.
        /// </summary>
        [Theory]
        [InlineData("7f83a0af-e7f4-4679-b600-ec23ce861a9c", "My bio")]
        [InlineData("7f83a0af-e7f4-4679-b600-ec23ce861a9c", "My second bio")]
        [InlineData("7f83a0af-e7f4-4679-b600-ec23ce861a9c", "My third bio")]
        public async Task TestChangeUsername(string userId, string bio)
        {
            var context = new TestLambdaContext();
            var request = new APIGatewayProxyRequest { Headers = MockHeaders(userId) };

            var data = new UserBioDto
            {
                Bio = bio,
            };
            request.Body = JsonConvert.SerializeObject(data);

            var functions = new Functions();
            var response = await functions.UpdateBio(request, context);

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
    }
}
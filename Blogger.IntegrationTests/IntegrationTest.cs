using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Tweetbook.Contracts.Requests;
using Tweetbook.Contracts.Responses;
using Tweetbook.Contracts.V1;
using Tweetbook.Contracts.V1.Responses;
using Tweetbook.Data;

namespace Tweetbook.IntegrationTests
{
    /// <summary>
    /// Base class for each integration test class
    /// </summary>
    public class IntegrationTest
    {
        protected readonly HttpClient _testClient;

        protected IntegrationTest()
        {
            var appFactory = new WebApplicationFactory<Startup>()
                .WithWebHostBuilder(builder => 
                {
                    // Here we want to use inmemory database so we can avoid populating databse with new user with every test run
                    // Because we will authenticate through user registration in order to run integration tests
                    builder.ConfigureServices(services =>
                    {
                        services.RemoveAll(typeof(DataContext));
                        services.AddDbContext<DataContext>().AddEntityFrameworkInMemoryDatabase();
                    });
                });
            _testClient = appFactory.CreateClient();
        }

        protected async Task AuthenticateAsync()
        {
            _testClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", await GetJwtAsync());
        }

        protected async Task<PostResponse> CreatePostAsync(CreatePostRequest request)
        {
            var response = await _testClient.PostAsJsonAsync(ApiRoutes.Posts.Create, request);
            return await response.Content.ReadAsAsync<PostResponse>();
        }

        private async Task<string> GetJwtAsync()
        {
            var response = await _testClient.PostAsJsonAsync(ApiRoutes.Identity.Register, new RegisterUserRequest{
                Email = "testing@integration.com",
                Password = "integration12345"
            });

            var registrationResponse = await response.Content.ReadAsAsync<AuthSuccessResponse>();
            return registrationResponse.Token;
        }
    }
}

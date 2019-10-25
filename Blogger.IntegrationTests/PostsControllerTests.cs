using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Tweetbook.Contracts.Requests;
using Tweetbook.Contracts.V1;
using Tweetbook.Domain;
using Xunit;

namespace Tweetbook.IntegrationTests
{
    public class PostsControllerTests : IntegrationTest
    {
        [Fact]
        public async Task GetAll_NoPosts_EmptyResponse()
        {
            // Arrange
            await AuthenticateAsync();

            // Act
            var response = await _testClient.GetAsync(ApiRoutes.Posts.GetAll);

            // Assert
            response.Should().Be(HttpStatusCode.OK);
            (await response.Content.ReadAsAsync<List<Post>>()).Should().BeEmpty();
        }

        [Fact]
        public async Task Get_PostExists_ReturnPost()
        {
            // Arrange
            await AuthenticateAsync();
            var createdPost = await CreatePostAsync(new CreatePostRequest { Name = "Post newbie" });

            // Act
            var response = await _testClient.GetAsync(ApiRoutes.Posts.Get.Replace("{postId}", createdPost.Id.ToString()));

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var post = await response.Content.ReadAsAsync<Post>();
            post.Should().NotBeNull();
            post.Id.Should().Be(createdPost.Id);
            post.Name.Should().Be("Post newbie");
        }
    }
}

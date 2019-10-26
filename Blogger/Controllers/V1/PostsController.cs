using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Tweetbook.Contracts.Requests;
using Tweetbook.Contracts.V1;
using Tweetbook.Contracts.V1.Requests;
using Tweetbook.Contracts.V1.Responses;
using Tweetbook.Domain;
using Tweetbook.Extensions;
using Tweetbook.Services;

namespace Tweetbook.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Authorize(Policy = "BloggerEmployee")]
    public class PostsController : Controller
    {
        private readonly IPostService _postService;

        public PostsController(IPostService postService)
        {
            _postService = postService;
        }

        [HttpGet(ApiRoutes.Posts.GetAll)]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _postService.GetPostsAsync());
        }

        [HttpGet(ApiRoutes.Posts.Get)]
        public async Task<IActionResult> Get([FromRoute]Guid postId)
        {
            var post = await _postService.GetPostByIdAsync(postId);
            if (post == null)
                return NotFound();

            return Ok(post);
        }

        [HttpGet(ApiRoutes.Posts.Update)]
        public async Task<IActionResult> Update([FromRoute]Guid postId, [FromBody]UpdatePost postToUpdate)
        {
            var userOwnsPost = await _postService.UserOwnsPostAsync(postId, HttpContext.GetUserId());
            if(userOwnsPost == false)
                return BadRequest(new { Error = "You are not allowed to update this post." });
             
            var post = new Post 
            {
                Id = postId,
                Name = postToUpdate.Name
            };

            if (await _postService.UpdatePostAsync(post))
                return Ok();

            return NotFound(); 
        }

        [HttpGet(ApiRoutes.Posts.Create)]
        public async Task<IActionResult> Create([FromBody]CreatePostRequest postRequest)
        {
            var post = new Post
            {
                Name = postRequest.Name,
                UserId = this.HttpContext.GetUserId()
            };

            await _postService.CreatePostAsync(new Post { Name = post.Name });

            var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.ToUriComponent()}";
            var resourceUri = baseUrl + "/" + ApiRoutes.Posts.Get.Replace("{postId}", post.Id.ToString());
            return Created(resourceUri, new PostResponse { Id = post.Id });
        }

        [HttpDelete]
        public async Task<IActionResult> Delete([FromRoute]Guid postId)
        {
            var userOwnsPost = await _postService.UserOwnsPostAsync(postId, HttpContext.GetUserId());
            if (userOwnsPost == false)
                return BadRequest(new { Error = "You are not allowed to delete this post." });

            bool postDeleted = await _postService.DeletePostAsync(postId);

            if (postDeleted)
                return NoContent();

            return NotFound();
        }
    }
}

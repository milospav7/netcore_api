using AutoMapper;
using Blogger.Contracts.Responses;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
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
    [Produces("application/json")]
    public class PostsController : Controller
    {
        private readonly IPostService _postService;
        private readonly IMapper _mapper;

        public PostsController(IPostService postService, IMapper mapper)
        {
            _postService = postService;
            _mapper = mapper;
        }

        /// <summary>
        /// Returns list of post.
        /// </summary>
        [HttpGet(ApiRoutes.Posts.GetAll)]
        public async Task<IActionResult> GetAll()
        {
            var posts = await _postService.GetPostsAsync();
            return Ok(_mapper.Map<IEnumerable<PostResponse>>(posts));
        }

        /// <summary>
        /// Returns post with passed Id.
        /// </summary>
        /// <remarks>
        ///     Sample **request:**:
        ///     GET /api/v1/posts
        /// </remarks>
        /// <param name="postId"></param>
        [HttpGet(ApiRoutes.Posts.Get)]
        public async Task<IActionResult> Get([FromRoute]Guid postId)
        {
            var post = await _postService.GetPostByIdAsync(postId);
            if (post == null)
                return NotFound();

            return Ok(post);
        }

        /// <summary>
        /// Updates post that matches passed argument.
        /// </summary>
        [HttpGet(ApiRoutes.Posts.Update)]
        public async Task<IActionResult> Update([FromRoute]Guid postId, [FromBody]UpdatePost postToUpdate)
        {
            var userOwnsPost = await _postService.UserOwnsPostAsync(postId, this.HttpContext.GetUserId());
            if(userOwnsPost == false)
                return BadRequest(new { Error = "You are not allowed to update this post." });
             
            var post = new Post 
            {
                Id = postId,
                Name = postToUpdate.Name
            };

            if (await _postService.UpdatePostAsync(post))
                return Ok(_mapper.Map<PostResponse>(post));

            return NotFound(); 
        }

        /// <summary>
        /// Creates new post.
        /// </summary>
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
            return Created(resourceUri, _mapper.Map<PostResponse>(post));
        }

        /// <summary>
        /// Deletes post that matches passed argument.
        /// </summary>
        /// <param name="postId"></param>
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

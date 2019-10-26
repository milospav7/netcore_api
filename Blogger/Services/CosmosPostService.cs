using Cosmonaut;
using Cosmonaut.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tweetbook.Domain;

namespace Tweetbook.Services
{
    public class CosmosPostService : IPostService
    {
        private readonly ICosmosStore<CosmosPost> _cosmosStore;

        public CosmosPostService(ICosmosStore<CosmosPost> cosmoStore)
        {
            _cosmosStore = cosmoStore;
        }

        public async Task<bool> CreatePostAsync(Post post)
        {
            var cosmosPost = new CosmosPost
            {
                Id = post.Id.ToString(),
                Name = post.Name
            };

            var response = await _cosmosStore.AddAsync(cosmosPost);
            return response.IsSuccess;
        }

        public async Task<bool> DeletePostAsync(Guid postId)
        {
            var response = await _cosmosStore.RemoveByIdAsync(postId.ToString());
            return response.IsSuccess;
        }

        public Task<object> GetAllHashtagsAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<Post> GetPostByIdAsync(Guid postId)
        {
            var post = await _cosmosStore.FindAsync(postId.ToString(), postId.ToString());

            if (post == null)
                return null;

            return new Post { Id = Guid.Parse(post.Id), Name = post.Name };
        }

        public async Task<List<Post>> GetPostsAsync()
        {
            var posts = await _cosmosStore.Query().ToListAsync(); // this is cosmos extension, not EF one
            return posts.Select(p => new Domain.Post { Id = Guid.Parse(p.Id.ToString()), Name = p.Name }).ToList();
        }

        public async Task<bool> UpdatePostAsync(Post postToUpdate)
        {
            var cosmosPost = new CosmosPost
            {
                Id = postToUpdate.Id.ToString(),
                Name = postToUpdate.Name
            };

            var response = await _cosmosStore.UpdateAsync(cosmosPost);
            return response.IsSuccess;
        }

        public Task<bool> UserOwnsPostAsync(Guid postId, string userId)
        {
            throw new NotImplementedException();
        }
    }
}

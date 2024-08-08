using AzureMinimalApiEfCore.Infrastructure.Context;
using AzureMinimalApiEFCore.Domain;
using Microsoft.EntityFrameworkCore;

namespace AzureMinimalApiEFCore.Extensions
{
    public static class WebApplicationExtensions
    {
        public static void MapBlogsAndPosts(this WebApplication app)
        {
            var blogsAndPosts = app.MapGroup("/blogsandposts").WithOpenApi();

            blogsAndPosts.MapGet("/", GetAllBlogsAndPosts).WithName("GetAllBlogsAndPosts");
            blogsAndPosts.MapGet("/blog", GetAllBlogs).WithName("GetAllBlogs");
            blogsAndPosts.MapGet("/{id}", GetBlog).WithName("GetBlog");
            blogsAndPosts.MapGet("/{id}/post", GetPosts).WithName("GetPosts");
            blogsAndPosts.MapPost("/", CreateBlog).WithName("CreateBlog");
            blogsAndPosts.MapPost("/{id}/Post", CreatePost).WithName("CreatePost");
        }



        static async Task<IResult> GetAllBlogsAndPosts(BloggingContext context)
        {
            try
            {
                if (context.Posts == null)
                {
                    return TypedResults.NotFound();
                }
                var blogsWithPosts = context.Blogs.Include(b => b.Posts).SelectMany(b => b.Posts, (blog, post) => new { blog, blog.Posts });
                return TypedResults.Ok(await blogsWithPosts.ToListAsync());
            }
            catch (Exception)
            {
                return TypedResults.BadRequest();
            }

        }
        static async Task<IResult> GetAllBlogs(BloggingContext context)
        {
            try
            {
                return TypedResults.Ok(await context.Blogs.ToListAsync());
            }
            catch (Exception)
            {
                return TypedResults.BadRequest();
            }
        }

        static async Task<IResult> GetBlog(int blogId, BloggingContext context)
        {
            try
            {
                var blog = context.Blogs.Include(b => b.Posts).FirstOrDefault(b => b.Id == blogId);
                if (blog == null)
                {
                    return TypedResults.NotFound();
                }
                return TypedResults.Ok(blog);
            }
            catch (Exception)
            {
                return TypedResults.BadRequest();
            }
        }

        static async Task<IResult> GetPosts(int blogId, BloggingContext context)
        {
            try
            {
                var blog = context.Blogs.Include(b => b.Posts).FirstOrDefault(b => b.Id == blogId);
                if (blog == null)
                {
                    return TypedResults.NotFound();
                }
                return TypedResults.Ok(blog.Posts.ToList());
            }
            catch (Exception)
            {
                return TypedResults.BadRequest();
            }
        }
        static async Task<IResult> CreateBlog(Blog blog, BloggingContext context)
        {
            try
            {
                context.Add(blog);
                await context.SaveChangesAsync();
                return TypedResults.Ok();
            }
            catch (Exception)
            {
                return TypedResults.BadRequest();
            }
        }

        static async Task<IResult> CreatePost(int blogId, Post post, BloggingContext context)
        {
            try
            {
                var blog = context.Blogs.Include(b => b.Posts).First(b => b.Id == blogId);
                blog.Posts.Add(post);
                await context.SaveChangesAsync();
                return TypedResults.Ok();
            }
            catch (Exception)
            {
                return TypedResults.BadRequest();
            }
        }
    }

}

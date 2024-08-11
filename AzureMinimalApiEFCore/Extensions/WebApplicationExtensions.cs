using AzureMinimalApiEfCore.Infrastructure.Context;
using AzureMinimalApiEFCore.Domain;
using AzureMinimalApiEFCore.dtos;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Collections;
using System.Collections.ObjectModel;
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
                var blog = context.Blogs.Select(b => new BlogDto()
                {
                    Name = b.Name,
                    Posts = b.Posts.Select(p => new PostDto()
                    {
                        Id = p.Id,
                        Archived = p.Archived,
                        Content = p.Content,
                        PublishedOn = p.PublishedOn,
                        Title = p.Title
                    }).ToList()
                });
                if (!await blog.AnyAsync())
                {
                    return TypedResults.NotFound();
                }
                return TypedResults.Ok(await blog.ToListAsync());
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
                var blogs = context.Blogs.Select(b => new BlogDto()
                {
                    Name = b.Name,
                    Id = b.Id,
                    SiteUri = b.SiteUri
                });

                if (!await blogs.AnyAsync())
                {
                    return TypedResults.NotFound();
                }
                return TypedResults.Ok(await blogs.ToListAsync());
            }
            catch (Exception)
            {
                return TypedResults.BadRequest();
            }
        }

        static async Task<IResult> GetBlog(Guid blogId, BloggingContext context)
        {
            try
            {
                var blog = await context.Blogs.FirstOrDefaultAsync(b => b.Id == blogId);
                if (blog == null)
                {
                    return TypedResults.NotFound();
                }

                var blogDto = new BlogDto()
                {
                    SiteUri = blog.SiteUri,
                    Name = blog.Name,
                    Id = blog.Id

                };
                return TypedResults.Ok(blogDto);
            }
            catch (Exception)
            {
                return TypedResults.BadRequest();
            }
        }

        static async Task<IResult> GetPosts(Guid blogId, BloggingContext context)
        {
            try
            {
                var blog = context.Blogs.Where(b => b.Id==blogId).Select(b  => new BlogDto()
                {
                    Name = b.Name,
                    Posts = b.Posts.Select(p => new PostDto()
                    {
                        Id = p.Id,
                        Archived = p.Archived,
                        Content = p.Content,
                        PublishedOn = p.PublishedOn,
                        Title = p.Title
                    }).ToList()
                });
                if (!blog.Any())
                {
                    return TypedResults.NotFound();
                }
                return TypedResults.Ok(await blog.ToListAsync());
            }
            catch (Exception)
            {
                return TypedResults.BadRequest();
            }
        }
        static async Task<IResult> CreateBlog(BlogDto blogDto, BloggingContext context)
        {
            try
            {
                var blog = new Blog()
                {
                    Name = blogDto.Name,
                    SiteUri = blogDto.SiteUri
                };
                if(blog.Posts ==null)
                    blog.Posts = new Collection<Post>();
                blogDto.Posts.ForEach(p => blog.Posts.Add(new Post()
                {
                    Archived = p.Archived,
                    Content = p.Content,
                    PublishedOn = p.PublishedOn,
                    Title = p.Title
                }));
                context.Add(blog);
                await context.SaveChangesAsync();
                return TypedResults.Ok();
            }
            catch (Exception )
            {
                return TypedResults.BadRequest();
            }
        }

        static async Task<IResult> CreatePost(Guid blogId, PostDto postDto, BloggingContext context)
        {
            try
            {
                var post = new Post()
                {
                    PublishedOn = postDto.PublishedOn,
                    Archived = postDto.Archived,
                    Content = postDto.Content,
                    Title = postDto.Title
                };
                var blog = await context.Blogs.Include(b => b.Posts).FirstAsync(b => b.Id == blogId);
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

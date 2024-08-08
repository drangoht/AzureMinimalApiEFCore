using AzureMinimalApiEFCore.Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
namespace AzureMinimalApiEfCore.Infrastructure.Context
{


    public class BloggingContext : DbContext
    {
        public DbSet<Blog> Blogs { get; set; }
        public DbSet<Post> Posts { get; set; }

        public string DbPath { get; }

        public BloggingContext(DbContextOptions<BloggingContext> options)
            : base(options)
        {
        }
        public BloggingContext()
        {
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AzureMinimalApiEFCore.dtos
{
    public class BlogDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public virtual Uri SiteUri { get; set; }
        public List<PostDto> Posts { get; set; }
    }
}

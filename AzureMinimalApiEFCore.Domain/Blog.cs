using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AzureMinimalApiEFCore.Domain
{
    public class Blog
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public virtual Uri SiteUri { get; set; }
        [JsonIgnore]
        public ICollection<Post> Posts { get; }
    }
}

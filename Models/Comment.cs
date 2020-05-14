using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TFridBlog.Models
{
    public class Comment
    {
        public int Id { get; set;  }
        public int BlogPostId { get; set; }
        public string AuthorId { get; set; }
       
        public DateTime Create { get; set; }
        public DateTime? Update { get; set; }
        public string UpdateReason { get; set; }

        public string Body { get; set; }

        //Navigational properties
        public virtual BlogPost BlogPost { get; set; }
        public virtual ApplicationUser Author { get; set; }
    }
}
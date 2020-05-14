using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TFridBlog.Models;

namespace TFridBlog.Helpers
{
    public class TrendingPost
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public IQueryable<BlogPost> GetBlogPosts(int topNumber) 
        {
            IQueryable<BlogPost> result = null;

            result = db.BlogPosts.AsQueryable();
            return result = result.OrderByDescending(b => b.ViewCount).Take(topNumber);        


            
            //return result.OrderByDescending(p => p.Created);


        }
    }
}
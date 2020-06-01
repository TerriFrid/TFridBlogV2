using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PagedList;
using PagedList.Mvc;
using TFridBlog.Helpers;
using TFridBlog.Models;

namespace TFridBlog.Controllers
{
    [RequireHttps]
    [Authorize(Roles = "Admin")]
    public class BlogPostsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private SearchHelper searchHelper = new SearchHelper();

        // GET: BlogPosts
        public ActionResult Index(int? page, string searchStr)
        {
            ViewBag.Search = searchStr;
            var blogList = searchHelper.IndexSearch(searchStr);
            var pageSize = 5;
            var pageNumber = page ?? 1;
           // var listPosts = db.BlogPosts.AsQueryable();
            //return View(db.BlogPosts.OrderBy(b => b.IsPublished).Orderby(b=> b.Created).ToPagedList());
            //return View(listPosts.OrderBy(b => b.IsPublished).ThenBy(b=>b.Created).ToPagedList(pageNumber, pageSize));
            return View(blogList.OrderBy(b => b.IsPublished).ThenByDescending(b => b.Created).ToPagedList(pageNumber, pageSize));
        }

        
        
        // GET: BlogPosts/Details/5
        [AllowAnonymous]
          public ActionResult Details(string slug)
        { 
          if (String.IsNullOrWhiteSpace(slug)) 
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BlogPost blogPost = db.BlogPosts.FirstOrDefault(p => p.Slug == slug);
            if (blogPost == null) 
            {
                return HttpNotFound();
            }

            //I don't want to count admin or moderator tasks as views.  I would rather evaluate NOT In but I don't know the correct syntax
            if (User.IsInRole("Admin") || User.IsInRole("Moderator" ))
            {
                return View(blogPost);
            }
            else
            {
                if (blogPost.ViewCount == null) 
                {
                    blogPost.ViewCount = 1;
                }
                else 
                { 
                blogPost.ViewCount += 1;               
                }
                db.SaveChanges();
                return View(blogPost);
            }
                
        }

        // GET: BlogPosts/Create
       
        public ActionResult Create()
        {
            return View();
        }

        // POST: BlogPosts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]               
        public ActionResult Create([Bind(Include = "Title,Abstract,Body,MediaUrl,IsPublished")] BlogPost blogPost,HttpPostedFileBase image)
        {
            if (ModelState.IsValid)
            {
                var slug = StringUtilities.URLFriendly(blogPost.Title);
                if (String.IsNullOrWhiteSpace(slug))
                {
                    ModelState.AddModelError("Title", "Invalid title");
                    return View(blogPost);
                };

                if (db.BlogPosts.Any(p=> p.Slug == slug)) 
                {
                    ModelState.AddModelError("Title", "The title must be unique");
                    return View(blogPost);
                };

                if (ImageUploadValidator.IsWebFriendlyImage(image)) 
                {
                    var justFileName = System.IO.Path.GetFileNameWithoutExtension(image.FileName);                    
                    justFileName = StringUtilities.URLFriendly(justFileName);
                    justFileName = $"{justFileName}--{DateTime.Now.Ticks}";
                    justFileName = $"{justFileName}{System.IO.Path.GetExtension(image.FileName)}";
                    image.SaveAs(System.IO.Path.Combine(Server.MapPath("~/Uploads/"), justFileName));
                    blogPost.MediaUrl = "/Uploads/" + justFileName;
                }

                blogPost.Slug = slug;
                blogPost.Created = DateTime.Now;
                blogPost.ViewCount = 0;
                db.BlogPosts.Add(blogPost);
                db.SaveChanges();
                return RedirectToAction("Index");
              // return RedirectToAction("Index");
            }

            return View(blogPost);
        }

        // GET: BlogPosts/Edit/5
        public ActionResult Edit(string slug)
        {
            if (String.IsNullOrWhiteSpace(slug))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BlogPost blogPost = db.BlogPosts.FirstOrDefault(p => p.Slug == slug);
            if (blogPost == null)
            {
                return HttpNotFound();
            }
            return View(blogPost);
        }
       

        // POST: BlogPosts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Created,Updated,Title,Slug,Abstract,Body,MediaUrl,IsPublished")] BlogPost blogPost, HttpPostedFileBase image)
        {
            if (ModelState.IsValid)
            {
                
                var slug = StringUtilities.URLFriendly(blogPost.Title);
                if (blogPost.Slug != slug) { 
                    if (String.IsNullOrWhiteSpace(slug))
                    {
                        ModelState.AddModelError("Title", "Invalid title");
                        return View(blogPost);
                    };

                    if (db.BlogPosts.Any(p => p.Slug == slug))
                    {
                       //it can equal it's own previous slug
                        ModelState.AddModelError("Title", "The title must be unique");
                        return View(blogPost);
                    };

                };

                if (ImageUploadValidator.IsWebFriendlyImage(image))
                {
                    var justFileName = System.IO.Path.GetFileNameWithoutExtension(image.FileName);
                    justFileName = StringUtilities.URLFriendly(justFileName);
                    justFileName = $"{justFileName}--{DateTime.Now.Ticks}";
                    justFileName = $"{justFileName}{System.IO.Path.GetExtension(image.FileName)}";
                    image.SaveAs(System.IO.Path.Combine(Server.MapPath("~/Uploads/"), justFileName));
                    blogPost.MediaUrl = "/Uploads/" + justFileName;


                }

                blogPost.Slug = slug;            

                blogPost.Updated = DateTime.Now;
                db.Entry(blogPost).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(blogPost);
        }

        // GET: BlogPosts/Delete/5
        public ActionResult Delete(string slug)
        {
           if (String.IsNullOrWhiteSpace(slug))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BlogPost blogPost = db.BlogPosts.FirstOrDefault(p => p.Slug == slug);
            if (blogPost == null)
            {
                return HttpNotFound();
            }
            return View(blogPost);
        }

        // POST: BlogPosts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string slug)
        {
            BlogPost blogPost = db.BlogPosts.FirstOrDefault(p => p.Slug == slug);
            db.BlogPosts.Remove(blogPost);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}

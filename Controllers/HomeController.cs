using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using PagedList;
using PagedList.Mvc;
using TFridBlog.Helpers;
using TFridBlog.Models;

namespace TFridBlog.Controllers
{
    [RequireHttps]
    public class HomeController : Controller
    {

        private ApplicationDbContext db = new ApplicationDbContext();

        private SearchHelper searchHelper = new SearchHelper();
        public ActionResult Index(int? page, string searchStr)
        {
            ViewBag.Search = searchStr;
            var blogList = searchHelper.IndexSearch(searchStr);
            var pageSize = 3;
            var pageNumber = page ?? 1;
            
            
            return View(blogList.Where(b => b.IsPublished).ToPagedList(pageNumber, pageSize));
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            EmailModel model = new EmailModel();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Contact (EmailModel model)
        {
            if (ModelState.IsValid)
            {
                try 
                {
                    var from = $"{model.FromName}<{WebConfigurationManager.AppSettings["emailfrom"]}>";
                    var email = new MailMessage(from, WebConfigurationManager.AppSettings["emailto"])
                    {
                        Subject = model.Subject,
                        Body = model.Body,
                        IsBodyHtml = true
                    };

                    var svc = new PersonalEmail();
                    await svc.SendAsync(email);

                   return RedirectToAction("Index", "Home");
                    
                }
                catch(Exception ex) 
                {
                    Console.WriteLine(ex.Message);
                    await Task.FromResult(0);
                }
            }

            return View(model);
        }


    }
}
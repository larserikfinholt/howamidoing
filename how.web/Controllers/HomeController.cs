using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using how.web.Models;
using how.web.ViewModel;

namespace how.web.Controllers
{
    public class HomeController : Controller
    {
        private ModelContext db = new ModelContext();

        public ActionResult Index()
        {
            var vm = new HomeViewModel();
            vm.Goals = db.Goals.Where(x => x.UserName == User.Identity.Name).ToList();

            return View(vm);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your quintessential app description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your quintessential contact page.";

            return View();
        }
    }
}

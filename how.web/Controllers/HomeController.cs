using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using how.web.Business;
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
            var logic = new GoalProcessor();
            var goals =  db.Goals.Include("DoneIts").Where(x => x.UserName == User.Identity.Name && x.Enabled).ToList();
            foreach (var goal in goals)
            {
                vm.Goals.Add(logic.ProcessGoal(goal));
            }
            vm.OverallStatus = logic.FindOverallStatus(vm.Goals);

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

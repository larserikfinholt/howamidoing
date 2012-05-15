using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using how.web.Models;

namespace how.web.ViewModel
{
    public class HomeViewModel
    {
        public string UserName { get; set; }
        public List<Goal> Goals { get; set; }


    }
}
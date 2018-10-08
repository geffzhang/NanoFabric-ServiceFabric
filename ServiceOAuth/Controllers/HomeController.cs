using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LTMCompanyNameFree.YoyoCmsTemplate.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace ServiceOAuth.Controllers
{
    public class HomeController : YoyoCmsTemplateControllerBase
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}

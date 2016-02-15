using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BonusPortal.Models;

namespace BonusPortal.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Consultants()
        {
            ViewBag.Message = "Add, remove or edit consultants";
            return View();
        }

        public ActionResult BonusCalc()
        {
            ViewBag.Message = "Calculate your bonus";
            return View();
        }

    }
}
using Microsoft.AspNetCore.Mvc;
using PBL3Hos.Models;
using System.Diagnostics;

namespace PBL3Hos.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

    }
}

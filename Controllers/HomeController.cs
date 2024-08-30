
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using Newtonsoft.Json;
using PBL3Hos.Identity;
using PBL3Hos.Migrations;
using PBL3Hos.Models.DbModel;
using PBL3Hos.ViewModel;
using System.Globalization;
using System.Linq;
using System.Numerics;
using PBL3Hos.Filters;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using System;
namespace PBL3Hos.Controllers
{
    public class HomeController : Controller
    {

        private readonly SignInManager<AppUser> signInManager;
        private readonly UserManager<AppUser> userManager;
        private readonly AppDbContext appDbContext;


        public HomeController(SignInManager<AppUser> signInManager, UserManager<AppUser> userManager, AppDbContext dbContext)
        {
            this.signInManager = signInManager;
            this.userManager = userManager;
            this.appDbContext=dbContext;
        }
        public async Task<IActionResult> Index()
        {
            /* if (User.Identity.IsAuthenticated)
             {
                 var user = await userManager.GetUserAsync(User);

                     var patient = appDbContext.Patients.FirstOrDefault(x => x.AccountId == user.Id);
                     if (patient != null)
                     {
                         var appointmentCancel = appDbContext.AppointmentCancels.Where(x => x.PatientId == patient.id).ToList();
                         return View(appointmentCancel);
                     }
                 // Handle the case where the user is authenticated but not found in UserManager
             }

                 // Redirect to login or handle unauthenticated user
                 return View();*/
            
            var user = await userManager.GetUserAsync(User);
            if (user!=null)
            {
                var patient = appDbContext.Patients.Where(x => x.AccountId==user.Id).FirstOrDefault();
                if (patient != null && patient.id != null)
                {
                    var notifications = appDbContext.AppointmentCancels.Include(x=>x.Doctor).Where(a => a.PatientId == patient.id).ToList();


                    // Đếm số lượng thông báo chưa đọc
                    var unseenCount = notifications.Count(n => n.Availabale == "Unseen");

                    // Tạo model để truyền dữ liệu tới view

                    ViewBag.UnseenCount=unseenCount;
                    ViewBag.Notifications=notifications;
                }
            }
            var topDoctors = appDbContext.Doctors
                .Include(d=>d.AppUser)
                .OrderByDescending(x => x.DoctorRate)  // Sắp xếp giảm dần theo DoctorRate
                .Take(3)  // Lấy top 3 bác sĩ
                .ToList();
            ViewBag.DoctorRate=topDoctors;
            return View();

        }
        [HttpPost]
        public async Task<IActionResult> MarkAsSeen()
        {
            var user = await userManager.GetUserAsync(User);
            var patient = appDbContext.Patients.Where(x => x.AccountId==user.Id).FirstOrDefault();
            // Lấy danh sách thông báo chưa đọc
            var notifications = appDbContext.AppointmentCancels.Where(a => a.PatientId == patient.id && a.Availabale == "Unseen").ToList();

            // Đánh dấu tất cả thông báo là đã đọc
            foreach (var notification in notifications)
            {
                notification.Availabale = "Seen";
            }

            appDbContext.SaveChanges();
            return Ok();
        }

    }
}


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
using System.Numerics;

namespace PBL3Hos.Controllers
{

    public class AccountController : Controller
    {
        private readonly SignInManager<AppUser> signInManager;
        private readonly UserManager<AppUser> userManager;
        private readonly AppDbContext appDbContext;


        public AccountController(SignInManager<AppUser> signInManager, UserManager<AppUser> userManager, AppDbContext dbContext)
        {
            this.signInManager = signInManager;
            this.userManager = userManager;
            this.appDbContext=dbContext;
        }
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> LoginAsync(LoginVM lgm)
        {
            if (ModelState.IsValid)
            {
                var result = await signInManager.PasswordSignInAsync(lgm.Username!, lgm.Password!, lgm.RememberMe, false);

                if (result.Succeeded)
                {
                    var user = await userManager.FindByNameAsync(lgm.Username);
                    var isInrole = await userManager.IsInRoleAsync(user, "Admin");
                    var isInrole1 = await userManager.IsInRoleAsync(user, "Doctor");
                    if (isInrole)
                    {
                        return RedirectToAction("Index", "User", new { area = "Admin" });
                    }
                    if (isInrole1)
                    {
                        return RedirectToAction("Index", "Doctor", new { area = "Doctor" });
                    }
                    else return RedirectToAction("Index", "Home");

                }

                else
                {
                    ModelState.AddModelError("myError", "Invalid username or password.");
                }
            }

            return View();
        }
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM rmv)
        {


            AppUser user = new AppUser();
            if (ModelState.IsValid)
            {
                user.UserName=rmv.Username;
                user.Address=rmv.Address;
                user.Email=rmv.Email;
                user.City=rmv.City;
                user.PhoneNumber=rmv.Mobile;
                user.Birthday=rmv.DateofBirth;


                var result1 = await userManager.CreateAsync(user, rmv.Password);


                if (result1.Succeeded)
                {
                    var result = await userManager.AddToRoleAsync(user, "Customer");
                    if (result.Succeeded)
                    {
                        UserPatient p = new UserPatient();
                        p.NamePatient=rmv.Username;
                        p.AccountId=user.Id;
                        p.Phone=user.PhoneNumber;
                        appDbContext.Patients.Add(p);
                        appDbContext.SaveChanges();
                        return RedirectToAction("Index", "Home");
                    }
                }
            }
            return View();

        }
        public async Task<IActionResult> ShowInfo()
        {
            var user = await userManager.GetUserAsync(User);
            InfoUser user1 = new InfoUser();
            user1.UserName=user.UserName;
            user1.Email=user.Email;
            user1.City=user.City;
            user1.Mobile=user.PhoneNumber;
            user1.DateofBirth=user.Birthday?.ToString("dd/MM/yyyy");
            return View(user1);
        }

        public async Task<IActionResult> Appointment(string id)
        {
            AppointmentDB appoint = new AppointmentDB();
            UserDoctor doctor = await appDbContext.Doctors
                                  .Include(d => d.AppUser)
                                  .FirstOrDefaultAsync(x => x.AccountId == id);
            appoint.Doctor=doctor;
            appoint.DoctorId=doctor.id;
            var user = await userManager.GetUserAsync(User);
            UserPatient patient = appDbContext.Patients.Where(x => x.AccountId==user.Id).FirstOrDefault();
            appoint.Patient=patient;
            appoint.PatientId=patient.id;
            Dictionary<string, string> data = new Dictionary<string, string>();
            Dictionary<string, List<string>> data1 = new Dictionary<string, List<string>>();
            var doctorAppointment = await appDbContext.Appointments.Where(da => da.DoctorId==doctor.id).ToListAsync();
            var doctorAvailabilities = await appDbContext.DoctorAvailabilities.Where(da => da.DoctorId == doctor.id).ToListAsync();
            foreach (var availability in doctorAvailabilities)
            {
                data.Add(availability.AvailableDate, availability.Shift);
            }
            foreach (var appointment in doctorAppointment)
            {
                string dateKey = appointment.Date.ToString("MM-dd-yyyy");
                string timeValue = appointment.Date.ToString("HH:mm:ss");

                // Kiểm tra xem khóa đã tồn tại trong Dictionary chưa
                if (data1.ContainsKey(dateKey))
                {
                    // Nếu khóa đã tồn tại, thêm giá trị mới vào danh sách giá trị của khóa đó
                    data1[dateKey].Add(timeValue);
                }
                else
                {
                    // Nếu khóa chưa tồn tại, tạo một danh sách mới và thêm giá trị vào danh sách đó
                    List<string> timeList = new List<string>();
                    timeList.Add(timeValue);
                    data1.Add(dateKey, timeList);
                }
            }

            ViewBag.DoctorAvailability = data;
            ViewBag.DoctorAppointment=data1;
            List<string>Dayban=new List<string>();
            foreach(var item in data1)
            {
                    DateTime date = DateTime.ParseExact(item.Key, "MM-dd-yyyy", CultureInfo.InvariantCulture);
                string dayofWeek = date.DayOfWeek.ToString();
                var Schedule = appDbContext.DoctorAvailabilities.FirstOrDefault(x=>x.AvailableDate==dayofWeek);
                if (Schedule.Shift=="Day")
                {
                    if (item.Value.Count==4)
                    {
                        Dayban.Add(item.Key);
                    }
                }
                else if (Schedule.Shift=="Aftenoon")
                {
                    if (item.Value.Count==4)
                    {
                        Dayban.Add(item.Key);
                    }
                }
                else if (Schedule.Shift=="Night")
                {
                    if (item.Value.Count==3)
                    {
                        Dayban.Add(item.Key);
                    }
                }
                else if (Schedule.Shift=="AllDay")
                {
                    if (item.Value.Count==11)
                    {
                        Dayban.Add(item.Key);
                    }
                }
          

            }
            var Doctordayoff = appDbContext.DoctorDayOffs.Where(x => x.DoctorId==doctor.id).ToList();
            var dayoff = Doctordayoff.Select(x => x.DateOff).ToList();
         
            foreach(var item in dayoff)
            {
                Dayban.Add(item.ToString("MM-dd-yyyy"));
            }
            DateTime currentDate = DateTime.Now.Date;
            DateTime endDate = currentDate.AddDays(300);
            while (currentDate < endDate)
            {
                string dayOfWeek = currentDate.DayOfWeek.ToString();
                var schedule = appDbContext.DoctorAvailabilities
                                .FirstOrDefault(x => x.AvailableDate == dayOfWeek && x.DoctorId == doctor.id);
                if (schedule == null)
                {
                    Dayban.Add(currentDate.ToString("MM-dd-yyyy"));
                }
                currentDate = currentDate.AddDays(1);
            }

            // Chuyển đổi danh sách này sang một chuỗi JSON
            if (Dayban!=null)
            {
                ViewBag.BookedDatesJson = JsonConvert.SerializeObject(Dayban);
            }
            return View(appoint);
        }
        [HttpPost]
        public async Task<IActionResult> Appointment (string id,AppointmentDB appointment,string Starttime, string Starttime1, string Starttime2, string Starttime3)
            {

            // Kiểm tra xem người dùng đã có lịch hẹn với bác sĩ đó chưa
            var existingAppointment = appDbContext.Appointments.FirstOrDefault(a => a.PatientId == appointment.PatientId );
          
            UserDoctor doctor1 = await appDbContext.Doctors
                                .Include(d => d.AppUser)
                                .FirstOrDefaultAsync(x => x.AccountId == id);
            appointment.Doctor=doctor1;
            Dictionary<string, string> data = new Dictionary<string, string>();
            Dictionary<string, List<string>> data1 = new Dictionary<string, List<string>>();

            ViewBag.DoctorAvailability = data;
            ViewBag.DoctorAppointment=data1;

            if (existingAppointment != null)
            {
                // Nếu đã có lịch hẹn, trả về thông báo lỗi cho người dùng
                ViewBag.Error = "Bạn đã đặt lịch trên hệ thống trước đó";
                return View(appointment);
            }
            AppointmentDB p1 = new AppointmentDB();

            p1.PatientId=appointment.PatientId;
          
            p1.DoctorId=appointment.DoctorId;
            p1.Date=appointment.Date;
            TimeSpan startTime;
            if (TimeSpan.TryParseExact(Starttime, "hh\\:mm\\:ss", CultureInfo.InvariantCulture, out startTime))
            {
                p1.Date = p1.Date.Date.AddHours(startTime.Hours);
            }

            TimeSpan startTime1;
            if (TimeSpan.TryParseExact(Starttime1, "hh\\:mm\\:ss", CultureInfo.InvariantCulture, out startTime1))
            {
                p1.Date = p1.Date.Date.AddHours(startTime1.Hours);
            }
            TimeSpan startTime2;
            if (TimeSpan.TryParseExact(Starttime2, "hh\\:mm\\:ss", CultureInfo.InvariantCulture, out startTime2))
            {
                p1.Date = p1.Date.Date.AddHours(startTime2.Hours);
            }
            TimeSpan startTime3;
            if (TimeSpan.TryParseExact(Starttime3, "hh\\:mm\\:ss", CultureInfo.InvariantCulture, out startTime3))
            {
                p1.Date = p1.Date.Date.AddHours(startTime3.Hours);
            }

            appDbContext.Appointments.Add(p1);
            ViewBag.Error="";
          
         
                appDbContext.SaveChanges();
            return RedirectToAction("Index", "Home");

        }
        public async Task<IActionResult> ListDoctor()
        {
            var list = await userManager.GetUsersInRoleAsync("Doctor");

            return View(list);
        }
        public async Task<IActionResult> ListAppointment()
        {
            //Sort
           
            var user = await userManager.GetUserAsync(User);
            UserPatient patient = await appDbContext.Patients
                                 .Include(d => d.AppUser)
                                 .FirstOrDefaultAsync(x => x.AccountId == user.Id);
            var listAppointment =  appDbContext.Appointments
    .Include(a => a.Doctor)
    .ThenInclude(p => p.AppUser)
    .Where(x => x.PatientId == patient.id).FirstOrDefault();

            return View(listAppointment);
        }
        public async Task<IActionResult> Cancel(string id)
        {
            var user = await userManager.GetUserAsync(User);
            UserPatient patient = await appDbContext.Patients
                                 .Include(d => d.AppUser)
                                 .FirstOrDefaultAsync(x => x.AccountId == user.Id);
            var listAppointment = appDbContext.Appointments
    .Include(a => a.Doctor)
    .ThenInclude(p => p.AppUser)
    .Where(x => x.PatientId == patient.id).FirstOrDefault();
            return View(listAppointment);
        }
        [HttpPost]
        public async Task<ActionResult> Cancel(string id, Appointment appointment)
        {
            var user = await appDbContext.Appointments.FirstOrDefaultAsync(a => a.PatientId == id);

            if (user==null)
            {
                return View();
            }
            appDbContext.Appointments.Remove(user);
            await appDbContext.SaveChangesAsync();


            return RedirectToAction("Index", "Home");
        }
        public async Task<ActionResult> EditAppointment()
        {
            AppointmentDB appoint = new AppointmentDB();
            var user = await userManager.GetUserAsync(User);
            UserPatient patient = appDbContext.Patients.Where(x => x.AccountId==user.Id).FirstOrDefault();
            appoint.Patient=patient;
            appoint.PatientId=patient.id;
            var doctorid = appDbContext.Appointments.Where(x => x.PatientId==patient.id).FirstOrDefault();
            UserDoctor doctor1=appDbContext.Doctors.Where(x=>x.id==doctorid.DoctorId).FirstOrDefault();
            string id = doctor1.AccountId;
            UserDoctor doctor = await appDbContext.Doctors
                                  .Include(d => d.AppUser)
                                  .FirstOrDefaultAsync(x => x.AccountId == id);
            appoint.Doctor=doctor;
            appoint.DoctorId=doctor.id;
           
           
            Dictionary<string, string> data = new Dictionary<string, string>();
            Dictionary<string, List<string>> data1 = new Dictionary<string, List<string>>();
            var doctorAppointment = await appDbContext.Appointments.Where(da => da.DoctorId==doctor.id).ToListAsync();
            var doctorAvailabilities = await appDbContext.DoctorAvailabilities.Where(da => da.DoctorId == doctor.id).ToListAsync();
            foreach (var availability in doctorAvailabilities)
            {
                data.Add(availability.AvailableDate, availability.Shift);
            }
            foreach (var appointment in doctorAppointment)
            {
                string dateKey = appointment.Date.ToString("MM-dd-yyyy");
                string timeValue = appointment.Date.ToString("HH:mm:ss");

                // Kiểm tra xem khóa đã tồn tại trong Dictionary chưa
                if (data1.ContainsKey(dateKey))
                {
                    // Nếu khóa đã tồn tại, thêm giá trị mới vào danh sách giá trị của khóa đó
                    data1[dateKey].Add(timeValue);
                }
                else
                {
                    // Nếu khóa chưa tồn tại, tạo một danh sách mới và thêm giá trị vào danh sách đó
                    List<string> timeList = new List<string>();
                    timeList.Add(timeValue);
                    data1.Add(dateKey, timeList);
                }
            }

            ViewBag.DoctorAvailability = data;
            ViewBag.DoctorAppointment=data1;
            List<string> Dayban = new List<string>();
            foreach (var item in data1)
            {
                DateTime date = DateTime.ParseExact(item.Key, "MM-dd-yyyy", CultureInfo.InvariantCulture);
                string dayofWeek = date.DayOfWeek.ToString();
                var Schedule = appDbContext.DoctorAvailabilities.FirstOrDefault(x => x.AvailableDate==dayofWeek);
                if (Schedule.Shift=="Day")
                {
                    if (item.Value.Count==4)
                    {
                        Dayban.Add(item.Key);
                    }
                }
                else if (Schedule.Shift=="Aftenoon")
                {
                    if (item.Value.Count==4)
                    {
                        Dayban.Add(item.Key);
                    }
                }
                else if (Schedule.Shift=="Night")
                {
                    if (item.Value.Count==3)
                    {
                        Dayban.Add(item.Key);
                    }
                }
                else if (Schedule.Shift=="AllDay")
                {
                    if (item.Value.Count==11)
                    {
                        Dayban.Add(item.Key);
                    }
                }


            }
            var Doctordayoff = appDbContext.DoctorDayOffs.Where(x => x.DoctorId==doctor.id).ToList();
            var dayoff = Doctordayoff.Select(x => x.DateOff).ToList();
            foreach (var item in dayoff)
            {
                Dayban.Add(item.ToString("MM-dd-yyyy"));
            }

            // Chuyển đổi danh sách này sang một chuỗi JSON
            if (Dayban!=null)
            {
                ViewBag.BookedDatesJson = JsonConvert.SerializeObject(Dayban);
            }
            return View(appoint);
        }
        [HttpPost]
        public async Task<ActionResult> EditAppointment(AppointmentDB appointment, string Starttime)
        {
            var user = await userManager.GetUserAsync(User);
            UserPatient patient = appDbContext.Patients.Where(x => x.AccountId==user.Id).FirstOrDefault();
            AppointmentDB appoint = appDbContext.Appointments.Where(x => x.PatientId==patient.id).FirstOrDefault();
            appoint.Date=appointment.Date;
            TimeSpan startTime;

            TimeSpan.TryParseExact(Starttime, "h\\:mm\\:ss", CultureInfo.InvariantCulture, out startTime);
            appoint.Date =appoint.Date.Date.AddHours(startTime.Hours);
            appDbContext.SaveChanges();
            return RedirectToAction("Index", "Home");
        }
       

    }
}


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
using System.Net.WebSockets;
namespace PBL3Hos.Controllers
{

    public class AccountController : Controller
    {
        private readonly SignInManager<AppUser> signInManager;
        private readonly UserManager<AppUser> userManager;
        private readonly AppDbContext appDbContext;
        private readonly IWebHostEnvironment environment;


        public AccountController(SignInManager<AppUser> signInManager, UserManager<AppUser> userManager, AppDbContext dbContext, IWebHostEnvironment environment)
        {
            this.signInManager = signInManager;
            this.userManager = userManager;
            this.appDbContext=dbContext;
            this.environment=environment;
        }
        public async Task<IActionResult> Index()
        {

            return View();
        }

       

        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> LoginAsync(LoginVM lgm)
        {
            ViewBag.Eror="";
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
                    ViewBag.Eror="Username or Password is wrong";
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
                user.Fullname=rmv.Fullname;


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
                        p.Fullname=user.Fullname;
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
            var patient=appDbContext.Patients.Where(x=>x.AccountId==user.Id).FirstOrDefault();
            InfoUser user1 = new InfoUser();
            user1.UserName=user.UserName;
            user1.Email=user.Email;
            user1.City=user.City;
            user1.Mobile=user.PhoneNumber;
            user1.Fullname=user.Fullname;
            user1.ImgUrl = patient.ImgFile;
            user1.DateofBirth=user.Birthday?.ToString("dd/MM/yyyy");
            return View(user1);
        }


        [Authorize]
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
            Dictionary<string,  List<Tuple<string, string>>> data = new Dictionary<string, List<Tuple<string, string>>>();

            Dictionary<string, List<string>> data1 = new Dictionary<string, List<string>>();
            var doctorAppointment = await appDbContext.Appointments.Where(da => da.DoctorId==doctor.id).ToListAsync();
            var doctorAvailabilities = await appDbContext.DoctorAvailabilities.Where(da => da.DoctorId == doctor.id).ToListAsync();
            foreach (var availability in doctorAvailabilities)
            {
                string startTimeStr = availability.StartTime.ToString("hh\\:mm\\:ss");
                string endTimeStr = availability.EndTime.ToString("hh\\:mm\\:ss");

                if (!data.ContainsKey(availability.AvailableDate))
                {
                    // Nếu khóa chưa tồn tại, tạo một danh sách mới và thêm vào
                    data[availability.AvailableDate] = new List<Tuple<string, string>>();
                }
               
                    // Thêm giờ bắt đầu và kết thúc vào danh sách tương ứng với khóa
                    data[availability.AvailableDate].Add(Tuple.Create(startTimeStr, endTimeStr));
               
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
            Dictionary<string, List<int>> keyValuePairs = new Dictionary<string, List<int>>();
            //lưu các thứ làm việc của bác sĩ và khoảng thời gian từng ngày
            foreach (var item in doctorAvailabilities)
            {
                 if (keyValuePairs.ContainsKey(item.AvailableDate))
                {
                    TimeSpan duration = item.EndTime - item.StartTime;
               
                    // Nếu khóa đã tồn tại, thêm giá trị mới vào danh sách giá trị của khóa đó
                    keyValuePairs[item.AvailableDate].Add((int)(duration.TotalHours*2));
                }
                else
                {
                    TimeSpan duration = item.EndTime - item.StartTime;
                    // Nếu khóa chưa tồn tại, tạo một danh sách mới và thêm giá trị vào danh sách đó
                    List<int> timeList = new List<int>();
                    timeList.Add((int)(duration.TotalHours*2));
                    keyValuePairs.Add(item.AvailableDate, timeList);
                }
            }
            Dictionary<string, int> keySumPairs = new Dictionary<string, int>();

            // Lặp qua từng cặp key-value trong keyValuePairs
            foreach (var pair in keyValuePairs)
            {
                // Khởi tạo biến để tính tổng
                int sum = 0;

                // Lặp qua từng giá trị int trong danh sách giá trị của mỗi khóa
                foreach (int value in pair.Value)
                {
                    // Cộng giá trị vào tổng
                    sum += value;
                }

                // Lưu tổng vào Dictionary keySumPairs
                keySumPairs.Add(pair.Key, sum);
            }

            // đếm trong Appointment
              foreach (var item in data1) //data1 chứa ngày trong Appointment và List giờ tương ứng
              {
                  DateTime date = DateTime.ParseExact(item.Key, "MM-dd-yyyy", CultureInfo.InvariantCulture);
                  string dayofWeek = date.DayOfWeek.ToString();
                 foreach(var item1 in keySumPairs)
                  {
                      if (dayofWeek==item1.Key)
                      {
                          if (item1.Value==item.Value.Count)
                              Dayban.Add(item.Key);
                      }
                  }



              }
            // Chuyển đổi danh sách này sang một chuỗi JSON
            if (Dayban!=null)
            {
                ViewBag.BookedDatesJson = JsonConvert.SerializeObject(Dayban);
            }
            return View(appoint);
        }
        [HttpPost]
        public async Task<IActionResult> Appointment (string id,AppointmentDB appointment,string Starttime)
            {

            // Kiểm tra xem người dùng đã có lịch hẹn với bác sĩ đó chưa
            var existingAppointment = appDbContext.Appointments.FirstOrDefault(a => a.PatientId == appointment.PatientId );
            if (existingAppointment != null)
            {
                // Nếu đã có lịch hẹn, trả về thông báo lỗi cho người dùng
                return Json(new { success = false, message= "You have previously booked" });
            }

            if(appointment.Date == null || Starttime == null)
            {
                return Json(new { success = false, message = "Date and time cannot be blank" });

            }

            UserDoctor doctor1 = await appDbContext.Doctors
                                .Include(d => d.AppUser)
                                .FirstOrDefaultAsync(x => x.AccountId == id);
            appointment.Doctor=doctor1;
            Dictionary<string, string> data = new Dictionary<string, string>();
            Dictionary<string, List<string>> data1 = new Dictionary<string, List<string>>();

            ViewBag.DoctorAvailability = data;
            ViewBag.DoctorAppointment=data1;

           
            AppointmentDB p1 = new AppointmentDB();

            p1.PatientId=appointment.PatientId;
          
            p1.DoctorId=appointment.DoctorId;
            p1.Date=appointment.Date;
            p1.Symptom=appointment.Symptom;
            TimeSpan startTime;
            if (TimeSpan.TryParseExact(Starttime, "hh\\:mm\\:ss", CultureInfo.InvariantCulture, out startTime))
            {
                p1.Date = p1.Date.Date.AddHours(startTime.Hours).AddMinutes(startTime.Minutes);
            }


            appDbContext.Appointments.Add(p1);
            ViewBag.Error="";


            await appDbContext.SaveChangesAsync();
            return Json(new { success = true });

        }
        [Authorize]
        public async Task<IActionResult> ListDoctor(string Day = "", string Time = "",string Specialist="")
        {
            if (string.IsNullOrEmpty(Day) && string.IsNullOrEmpty(Time) && string.IsNullOrEmpty(Specialist))
            {
                return View(appDbContext.Doctors.ToList());
            }

            if (!string.IsNullOrEmpty(Day) && !string.IsNullOrEmpty(Time) && !string.IsNullOrEmpty(Specialist))
            {
                if (!DateTime.TryParse(Day, out DateTime selectedDay))
                {
                    // Xử lý lỗi ngày không hợp lệ
                    // Có thể trả về một trang lỗi hoặc thông báo
                    return View("Error");
                }

                DayOfWeek dayOfWeek = selectedDay.DayOfWeek;
                string dayOfWeekString = dayOfWeek.ToString();

                if (!TimeSpan.TryParse(Time, out TimeSpan selectedTime))
                {
                    // Xử lý lỗi thời gian không hợp lệ
                    // Thông báo lỗi cụ thể
                    ViewData["ErrorMessage"] = "Invalid time format.";
                    return View("Error");
                }

                var doctorAvailability = appDbContext.DoctorAvailabilities
                    .Where(x => x.AvailableDate == dayOfWeekString && x.StartTime <= selectedTime && x.EndTime > selectedTime && x.Doctor.Specialist==Specialist)
                    .Select(x => x.DoctorId)
                    .ToList();

                var doctorAppointments = appDbContext.Appointments
                    .Where(x => doctorAvailability.Contains(x.DoctorId) && x.Date.Date == selectedDay.Date && x.Date.Hour == selectedTime.Hours && x.Date.Minute == selectedTime.Minutes)
                    .Select(x => x.DoctorId)
                    .ToList();
                var selectedDateOnly = new DateOnly(selectedDay.Date.Year, selectedDay.Date.Month, selectedDay.Date.Day);
                var doctorDayOff = appDbContext.DoctorDayOffs.Where(x => doctorAvailability.Contains(x.DoctorId)&& x.DateOff==selectedDateOnly).Select(x => x.DoctorId).ToList();
                var availableDoctorIds = doctorAvailability.Except(doctorAppointments).Except(doctorDayOff).ToList();


                var availableDoctors = appDbContext.Doctors
                    .Where(d => availableDoctorIds.Contains(d.id))
                    .ToList();

                return View(availableDoctors);
            }
            else if (!string.IsNullOrEmpty(Day) && string.IsNullOrEmpty(Time) && string.IsNullOrEmpty(Specialist))
            {
                if (!DateTime.TryParse(Day, out DateTime selectedDay))
                {
                    // Xử lý lỗi ngày không hợp lệ
                    // Có thể trả về một trang lỗi hoặc thông báo
                    return View("Error");
                }

                DayOfWeek dayOfWeek = selectedDay.DayOfWeek;
                string dayOfWeekString = dayOfWeek.ToString();

                var doctorAvailability = appDbContext.DoctorAvailabilities
                    .Where(x => x.AvailableDate == dayOfWeekString )
                    .Select(x => x.DoctorId)
                    .ToList();

                var doctorAppointments = appDbContext.Appointments
                    .Where(x => doctorAvailability.Contains(x.DoctorId) && x.Date.Date == selectedDay.Date)
                    .Select(x => x.DoctorId)
                    .ToList();
                var selectedDateOnly = new DateOnly(selectedDay.Date.Year, selectedDay.Date.Month, selectedDay.Date.Day);
                var doctorDayOff = appDbContext.DoctorDayOffs.Where(x => doctorAvailability.Contains(x.DoctorId)&& x.DateOff==selectedDateOnly).Select(x => x.DoctorId).ToList();
                var availableDoctorIds = doctorAvailability.Except(doctorAppointments).Except(doctorDayOff).ToList();


                var availableDoctors = appDbContext.Doctors
                    .Where(d => availableDoctorIds.Contains(d.id))
                    .ToList();

                return View(availableDoctors);

            }
            else if (!string.IsNullOrEmpty(Day) && string.IsNullOrEmpty(Time) && !string.IsNullOrEmpty(Specialist))
            {
                if (!DateTime.TryParse(Day, out DateTime selectedDay))
                {
                    // Xử lý lỗi ngày không hợp lệ
                    // Có thể trả về một trang lỗi hoặc thông báo
                    return View("Error");
                }

                DayOfWeek dayOfWeek = selectedDay.DayOfWeek;
                string dayOfWeekString = dayOfWeek.ToString();

                var doctorAvailability = appDbContext.DoctorAvailabilities
                    .Where(x => x.AvailableDate == dayOfWeekString && x.Doctor.Specialist==Specialist)
                    .Select(x => x.DoctorId)
                    .ToList();

                var doctorAppointments = appDbContext.Appointments
                    .Where(x => doctorAvailability.Contains(x.DoctorId) && x.Date.Date == selectedDay.Date)
                    .Select(x => x.DoctorId)
                    .ToList();
                var selectedDateOnly = new DateOnly(selectedDay.Date.Year, selectedDay.Date.Month, selectedDay.Date.Day);
                var doctorDayOff = appDbContext.DoctorDayOffs.Where(x => doctorAvailability.Contains(x.DoctorId)&& x.DateOff==selectedDateOnly).Select(x => x.DoctorId).ToList();
                var availableDoctorIds = doctorAvailability.Except(doctorAppointments).Except(doctorDayOff).ToList();


                var availableDoctors = appDbContext.Doctors
                    .Where(d => availableDoctorIds.Contains(d.id))
                    .ToList();

                return View(availableDoctors);

            }
            else 
            {
            


                var doctorAvailability = appDbContext.Doctors
                    .Where(x=>x.Specialist==Specialist)
                    .ToList();

               
               

                return View(doctorAvailability);

            }
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


            return Json(new { success = true });
        }
        public async Task<ActionResult> EditAppointment()
        {
            AppointmentDB appoint = new AppointmentDB();
    
            var user = await userManager.GetUserAsync(User);
            UserPatient patient = appDbContext.Patients.Where(x => x.AccountId==user.Id).FirstOrDefault();
            appoint.Patient=patient;
            appoint.PatientId=patient.id;
            var doctorid = appDbContext.Appointments.Where(x => x.PatientId==patient.id).FirstOrDefault();
            
            UserDoctor doctor1 = appDbContext.Doctors.Where(x => x.id==doctorid.DoctorId).FirstOrDefault();
            string id = doctor1.AccountId;
            UserDoctor doctor = await appDbContext.Doctors
                                  .Include(d => d.AppUser)
                                  .FirstOrDefaultAsync(x => x.AccountId == id);
            appoint.Doctor=doctor;
            appoint.DoctorId=doctor.id;

            Dictionary<string, List<Tuple<string, string>>> data = new Dictionary<string, List<Tuple<string, string>>>();

            Dictionary<string, List<string>> data1 = new Dictionary<string, List<string>>();
            var doctorAppointment = await appDbContext.Appointments.Where(da => da.DoctorId==doctor.id).ToListAsync();
            var doctorAvailabilities = await appDbContext.DoctorAvailabilities.Where(da => da.DoctorId == doctor.id).ToListAsync();
            foreach (var availability in doctorAvailabilities)
            {
                string startTimeStr = availability.StartTime.ToString("hh\\:mm\\:ss");
                string endTimeStr = availability.EndTime.ToString("hh\\:mm\\:ss");

                if (!data.ContainsKey(availability.AvailableDate))
                {
                    // Nếu khóa chưa tồn tại, tạo một danh sách mới và thêm vào
                    data[availability.AvailableDate] = new List<Tuple<string, string>>();
                }

                // Thêm giờ bắt đầu và kết thúc vào danh sách tương ứng với khóa
                data[availability.AvailableDate].Add(Tuple.Create(startTimeStr, endTimeStr));

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


            }
            var Doctordayoff = appDbContext.DoctorDayOffs.Where(x => x.DoctorId==doctor.id).ToList();
            var dayoff = Doctordayoff.Select(x => x.DateOff).ToList();

            foreach (var item in dayoff)
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
            Dictionary<string, List<int>> keyValuePairs = new Dictionary<string, List<int>>();
            //lưu các thứ làm việc của bác sĩ và khoảng thời gian từng ngày
            foreach (var item in doctorAvailabilities)
            {
                if (keyValuePairs.ContainsKey(item.AvailableDate))
                {
                    TimeSpan duration = item.EndTime - item.StartTime;

                    // Nếu khóa đã tồn tại, thêm giá trị mới vào danh sách giá trị của khóa đó
                    keyValuePairs[item.AvailableDate].Add((int)(duration.TotalHours*2));
                }
                else
                {
                    TimeSpan duration = item.EndTime - item.StartTime;
                    // Nếu khóa chưa tồn tại, tạo một danh sách mới và thêm giá trị vào danh sách đó
                    List<int> timeList = new List<int>();
                    timeList.Add((int)(duration.TotalHours*2));
                    keyValuePairs.Add(item.AvailableDate, timeList);
                }
            }
            Dictionary<string, int> keySumPairs = new Dictionary<string, int>();

            // Lặp qua từng cặp key-value trong keyValuePairs
            foreach (var pair in keyValuePairs)
            {
                // Khởi tạo biến để tính tổng
                int sum = 0;

                // Lặp qua từng giá trị int trong danh sách giá trị của mỗi khóa
                foreach (int value in pair.Value)
                {
                    // Cộng giá trị vào tổng
                    sum += value;
                }

                // Lưu tổng vào Dictionary keySumPairs
                keySumPairs.Add(pair.Key, sum);
            }

            // đếm trong Appointment
            foreach (var item in data1) //data1 chứa ngày trong Appointment và List giờ tương ứng
            {
                DateTime date = DateTime.ParseExact(item.Key, "MM-dd-yyyy", CultureInfo.InvariantCulture);
                string dayofWeek = date.DayOfWeek.ToString();
                foreach (var item1 in keySumPairs)
                {
                    if (dayofWeek==item1.Key)
                    {
                        if (item1.Value==item.Value.Count)
                            Dayban.Add(item.Key);
                    }
                }



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
            return Json(new { success = true });
        }


        public async Task<IActionResult> AppointmentHistory(string Date="")
        {
            if (Date!="")
            {
                DateTime date = DateTime.ParseExact(Date, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                var user = await userManager.GetUserAsync(User);
                UserPatient patient = await appDbContext.Patients
                                   .Include(d => d.AppUser)
                                   .FirstOrDefaultAsync(x => x.AccountId == user.Id);
                var appointmentHistory = appDbContext.AppointmentHistory
                                  .Include(a => a.Doctor)
                                  .ThenInclude(p => p.AppUser)
                                  .Where(x => x.PatientId == patient.id &&
                                              x.Date.Day == date.Day &&
                                              x.Date.Month == date.Month &&
                                              x.Date.Year == date.Year)
                                  .ToList();
                ViewBag.Userid = patient.id;
                return View(appointmentHistory);
            }
            else
            {
                var user = await userManager.GetUserAsync(User);
                UserPatient patient = await appDbContext.Patients
                                   .Include(d => d.AppUser)
                                   .FirstOrDefaultAsync(x => x.AccountId == user.Id);
                var appointmentHistory = appDbContext.AppointmentHistory
                                  .Include(a => a.Doctor)
                                  .ThenInclude(p => p.AppUser)
                                  .Where(x => x.PatientId == patient.id)
                                  .ToList();
                ViewBag.Userid = patient.id;
                return View(appointmentHistory);
            }
           
            

        }

        public async Task<IActionResult> InfoDoctor(string id)
        {
          
            var doctor= appDbContext.Doctors.Include(d=>d.AppUser).Where(x=>x.AccountId==id).FirstOrDefault();
            var rated = appDbContext.Ratings.Include(d => d.Patient).Where(x => x.DoctorId==doctor.id).ToList();
            int count=rated.Count();
            if (count!=0)
            {
                var averagerate = rated.Average(d => d.Star);
                averagerate = Math.Round(averagerate, 1);

                ViewBag.Averagerate = averagerate;

          
            }
            ViewBag.Rating=rated;
            return View(doctor);


        }

        public async Task<IActionResult> ListDoctor1(string id)
        {

            var doctor = appDbContext.Doctors.ToList();

            return View(doctor);


        }

        [HttpPost]
        public async Task<IActionResult> Rating (double rate,string DoctorId,string PatientId, string Detail,string AppointmentHistoryId)
        {
            var Appointmenthistory=appDbContext.AppointmentHistory.Where(x=>x.Id.ToString()==AppointmentHistoryId).FirstOrDefault();
            var doctor=appDbContext.Doctors.Where(x=>x.id==DoctorId).FirstOrDefault();
            Appointmenthistory.Rate="Rated";
            appDbContext.SaveChanges();
            Rating Ratings = new Rating();
            Ratings.DoctorId = DoctorId;
            Ratings.PatientId = PatientId;
            Ratings.Detail = Detail;
            Ratings.Star = rate;
            if (doctor.DoctorRate!=0)
            {
                doctor.DoctorRate=(doctor.DoctorRate+rate)/2;
            }
            else
            {
                doctor.DoctorRate=rate;

            }
            appDbContext.Add(Ratings);
            appDbContext.SaveChanges();




            return RedirectToAction("AppointmentHistory", "Account");
        }
      
        public async Task<IActionResult> SaveImg(IFormFile ImgFile)
        {
            if (ImgFile != null)
            {
                string filename = DateTime.Now.ToString("yyyyMMddHHmmssfff") + Path.GetExtension(ImgFile.FileName);
                string fullpath = Path.Combine(environment.WebRootPath, "PatientsIMG", filename);

                using (var stream = new FileStream(fullpath, FileMode.Create))
                {
                    await ImgFile.CopyToAsync(stream);
                }

                var user = await userManager.GetUserAsync(User);
                var patient = appDbContext.Patients.FirstOrDefault(p => p.AccountId == user.Id);
                if (patient != null)
                {

                    string newImageUrl = Url.Content($"~/PatientsIMG/{filename}");
                    return Json(new { newImageUrl });
                }
            }

            return Json(new { newImageUrl = string.Empty });
        }

        [HttpPost]
        public async Task<IActionResult> SaveImg1(IFormFile ImgFile,DoctorAvailabilities a)
        {
            if (ImgFile != null)
            {
                string filename = DateTime.Now.ToString("yyyyMMddHHmmssfff") + Path.GetExtension(ImgFile.FileName);
                 string fullpath = Path.Combine(environment.WebRootPath, "PatientsIMG", filename);

                using (var stream = new FileStream(fullpath, FileMode.Create))
                {
                    await ImgFile.CopyToAsync(stream);
                }

                var user = await userManager.GetUserAsync(User);
                var patient = appDbContext.Patients.FirstOrDefault(p => p.AccountId == user.Id);
                if (patient != null)
                {
                    patient.ImgFile = filename;
                    await appDbContext.SaveChangesAsync();

                   
                }
            }

            return RedirectToAction("EditProfile","Account");
        }
        public async Task<IActionResult> EditProfile()
        {
            var user = await userManager.GetUserAsync(User);
            var patient = appDbContext.Patients.Where(x => x.AccountId==user.Id).FirstOrDefault();
            InfoUser user1 = new InfoUser();
            user1.UserName=user.UserName;
            user1.Email=user.Email;
            user1.City=user.City;
            user1.Mobile=user.PhoneNumber;
            user1.Fullname=user.Fullname;
            user1.ImgUrl = patient.ImgFile;
            user1.DateofBirth=user.Birthday?.ToString("yyyy-MM-dd");
            return View(user1);
        }
        [HttpPost]
        public async Task<IActionResult> EditProfile(InfoUser edit)
        {
            
            var user = await userManager.GetUserAsync(User);
            var useredit = appDbContext.Users.Where(x => x.Id==user.Id).FirstOrDefault();
            var patient = appDbContext.Patients.Where(x => x.AccountId==user.Id).FirstOrDefault();
            if (edit.Fullname!=null)
                useredit.Fullname=edit.Fullname;
            else
            {
                return Json(new { success = false, message = "Fullname cannot be blank" });

            }
            if (edit.Email!=null)
                useredit.Email=edit.Email;
            if (edit.UserName!=null)
            {
                useredit.UserName=edit.UserName;
            }
            else
            {
                return Json(new { success = false, message = "Username cannot be blank" });


            }

            if (edit.Mobile!=null)
                useredit.PhoneNumber=edit.Mobile;
            else
            {
                return Json(new { success = false, message = "Mobile cannot be blank" });

            }
            if (edit.Birthday!=null)
                useredit.Birthday=edit.Birthday;

            else
            {
                return Json(new { success = false, message = "Birthday cannot be blank" });

            }
            if (edit.Adress!=null)
                useredit.Address=edit.Adress;

            appDbContext.SaveChanges();

            if (edit.UserName!=null)
            {
                patient.NamePatient=edit.UserName;

            }

            if (edit.Mobile!=null)
                patient.Phone=edit.Mobile;

            if (edit.Fullname!=null)
                patient.Fullname=edit.Fullname;

            appDbContext.SaveChanges();
            return Json(new { success = true });
        }

    }
}

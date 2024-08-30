using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using PBL3Hos.Identity;
using PBL3Hos.Models.DbModel;
using PBL3Hos.ViewModel;
using System.Globalization;
using System.Net.WebSockets;

namespace PBL3Hos.Areas.Doctor.Controllers
{
    [Area("Doctor")]
    public class DoctorController : Controller
    {
        private readonly SignInManager<AppUser> signInManager;
        private readonly UserManager<AppUser> userManager;
        private readonly AppDbContext appDbContext;
        private readonly IWebHostEnvironment environment;



        public DoctorController(SignInManager<AppUser> signInManager, UserManager<AppUser> userManager, AppDbContext dbContext, IWebHostEnvironment environment)
        {
            this.signInManager = signInManager;
            this.userManager = userManager;
            this.appDbContext=dbContext;
            this.environment=environment;

        }
        public async Task<IActionResult> Index()
        {
            // list patient

            var user = await userManager.GetUserAsync(User);
            var userid = user.Id.ToString();
            var doctor = await appDbContext.Doctors.Where(x => x.AccountId==userid).FirstOrDefaultAsync();
            var patientlist = appDbContext.AppointmentHistory.Include(x => x.Patient)
                                                             .Include(x=>x.Doctor)
                                                             .Where(x => x.DoctorId==doctor.id).ToList();

            var AppointmentHistory = appDbContext.AppointmentHistory.Where(x => x.DoctorId==doctor.id).ToList();
            Dictionary<string, Tuple<string, string,string>> patientlists = new Dictionary<string, Tuple<string, string,string>>();
            List<string> patientid = new List<string>();
            foreach (var item in patientlist)
            {
                if (patientid.Contains(item.PatientId)!=true) {
                    patientlists[item.Patient.Fullname]=Tuple.Create(item.Patient.Phone, item.Doctor.Specialist,item.Patient.ImgFile);
                    patientid.Add(item.PatientId);
                    }

            }
            ViewBag.PatientList=patientlists;
            Dictionary<int, List<string>> data = new Dictionary<int, List<string>>();
            foreach (var item in AppointmentHistory)
            {
                int month = item.Date.Month;
                if (!data.ContainsKey(month))
                {
                    data[month]= new List<string> { item.Date.ToString() };
                }
                else
                {
                    data[month].Add(item.Date.ToString());
                }

            }
            var DoctorSchedule = appDbContext.DoctorAvailabilities.Where(x => x.DoctorId==doctor.id).ToList();
            Dictionary<string, double> data1 = new Dictionary<string, double>();
            foreach (var item in DoctorSchedule)
            {
                TimeSpan startTime = item.StartTime;
                TimeSpan endTime = item.EndTime;
                TimeSpan result = endTime.Subtract(startTime);
                double totalHours = result.TotalHours;
                if (!data1.ContainsKey(item.AvailableDate))
                {
                    data1[item.AvailableDate]=totalHours;
                }
                else
                {
                    data1[item.AvailableDate]+=totalHours;
                }

            }
            var DayOff = appDbContext.DoctorDayOffs.Where(x => x.DoctorId==doctor.id).ToList();

            Dictionary<int, Tuple<double, int>> data2 = new Dictionary<int, Tuple<double, int>>();
            var appointmentHistory = appDbContext.AppointmentHistory.Where(x => x.DoctorId==doctor.id).ToList();
            for (int i = 1; i<=12; i++)
            {

                data2[i]=Tuple.Create(data1.Values.Sum()*4, 0);
                foreach (var item1 in DayOff)
                {
                    int month = item1.DateOff.Month;
                    string DateofWeek = item1.DateOff.DayOfWeek.ToString();
                    if (month==i)
                    {

                        data2[i]=Tuple.Create(data2[i].Item1-data1[DateofWeek], 0);


                    }
                }

                var total = appointmentHistory.Count(x => x.Date.Month==i);
                if (total!=0)
                {
                    data2[i]=Tuple.Create(data2[i].Item1, total);
                }
                var date = DateTime.Now;
                var doctorappointment = appDbContext.Appointments.Where(x => x.DoctorId==doctor.id && x.Date.Date==date.Date).Count();
                ViewBag.DoctorAppointment = doctorappointment;  


            }
            return View(data2);
        }
        [HttpPost]
        public ActionResult Delete()
        {
            // Xử lý yêu cầu xóa ở đây
            // Ví dụ: xóa dữ liệu từ cơ sở dữ liệu

            // Trả về một phản hồi JSON cho client
            return Json(new { success = true });
        }
        public async Task<IActionResult> ShowSchedule()
        {
            var user = await userManager.GetUserAsync(User);
            var doctor= appDbContext.Doctors.Where(x=>x.AccountId==user.Id).FirstOrDefault();
            var schedule1 = appDbContext.DoctorAvailabilities.Where(x=>x.DoctorId==doctor.id).ToList();
            var schedule=schedule1.OrderBy(x=>x.StartTime).ToList();
            Dictionary<string, List<Tuple<string, string>>> DoctorSchedule=new Dictionary<string, List<Tuple<string, string>>>();
            foreach(var item in schedule)
            {
                if (!DoctorSchedule.ContainsKey(item.AvailableDate))
                {
                    DoctorSchedule[item.AvailableDate]=new List<Tuple<string, string>>();
                }
                DoctorSchedule[item.AvailableDate].Add(Tuple.Create(item.StartTime.ToString(), item.EndTime.ToString()));
            }
           ViewBag.DoctorSchedule = DoctorSchedule;
            return View();
        }
        public async Task<IActionResult> ScheduleOff()
        {
            AppointmentDB appoint = new AppointmentDB();
            var user = await userManager.GetUserAsync(User);
            UserDoctor doctor = await appDbContext.Doctors
                                  .Include(d => d.AppUser)
                                  .FirstOrDefaultAsync(x => x.AccountId == user.Id);
            appoint.Doctor=doctor;
            appoint.DoctorId=doctor.id;
            Dictionary<string, List<Tuple<string, string>>> data = new Dictionary<string, List<Tuple<string, string>>>();

            Dictionary<string, List<string>> data1 = new Dictionary<string, List<string>>();
            var doctorAvailabilities = await appDbContext.DoctorAvailabilities.Where(da => da.DoctorId == doctor.id).ToListAsync();
       
            ViewBag.DoctorAvailability = data;
            ViewBag.DoctorAppointment=data1;
            List<string> Dayban = new List<string>();
       
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
         

         
            // Chuyển đổi danh sách này sang một chuỗi JSON
            if (Dayban!=null)
            {
                ViewBag.BookedDatesJson = JsonConvert.SerializeObject(Dayban);
            }
            ViewBag.DoctorId=doctor.id;
            return View();
            
        }
        [HttpPost]
        public async Task<IActionResult> ScheduleOff(DoctorDayOff dayoff)
        {
            var user = await userManager.GetUserAsync(User);
            var doctor = appDbContext.Doctors.Where(x => x.AccountId==user.Id).FirstOrDefault();
            var appointment = appDbContext.Appointments
   .Include(a => a.Doctor)
   .Include(p =>p.Patient)
   .Where(x => x.DoctorId == doctor.id).ToList();
            foreach (var item in appointment)
            {
                AppointmentCancel p = new AppointmentCancel();
                p.Date=item.Date;
                p.FullnamePatient=item.Patient.Fullname;
                p.FullnameDoctor=doctor.Fullname;
                p.PatientId=item.PatientId;
                p.DoctorId=doctor.id;
                appDbContext.AppointmentCancels.Add(p);
                appDbContext.SaveChanges();
                appDbContext.Appointments.Remove(item);
                await appDbContext.SaveChangesAsync();

            }
          

            DoctorDayOff doctorDayOff = dayoff;
           appDbContext.DoctorDayOffs.Add(doctorDayOff);
            appDbContext.SaveChanges();
            return RedirectToAction("ScheduleOff", "Doctor", new { area = "Doctor" });
        }
        public async Task<IActionResult> ListAppointment(string SortColumn="Date",string IconClass="fa-sort-up")
        {
            //Sort
            ViewBag.SortColumn=SortColumn;
            ViewBag.IConClass=IconClass;
            var user = await userManager.GetUserAsync(User);
            UserDoctor doctor = await appDbContext.Doctors
                                 .Include(d => d.AppUser)
                                 .FirstOrDefaultAsync(x => x.AccountId == user.Id);
            var listAppointment = await appDbContext.Appointments
                        .Include(a => a.Patient)
                        .ThenInclude(p => p.AppUser)
                        .Where(x => x.DoctorId == doctor.id)
                        .ToListAsync();
            if (SortColumn=="Date")
            {
                if (IconClass=="fa-sort-up")
                {
                    listAppointment=listAppointment.OrderBy(row=>row.Date).ToList();
                }
                else
                {
                    listAppointment=listAppointment.OrderByDescending(row => row.Date).ToList();
                }
            }
            var schedule = appDbContext.DoctorAvailabilities.Where(x => x.DoctorId==doctor.id);
            if (schedule!=null)
            {
                ViewBag.Availability="True";
            }
            else ViewBag.Availability="False";
            return View(listAppointment);
        }
        public async Task<IActionResult> DoctorSchedule()
        {
          
         var doctorShedule= new DoctorAvailability();
            return View(doctorShedule);
        }
        [HttpPost]
        public async Task<IActionResult> DoctorSchedule(Dictionary<string, string> ShiftStartDay, Dictionary<string, string> ShiftEndDay, Dictionary<string, string> ShiftStartAfternoon, Dictionary<string, string> ShiftEndAfternoon, Dictionary<string, string> ShiftStartNight, Dictionary<string, string> ShiftEndNight)
        {
           

            foreach (var item in ShiftStartDay)
            {
                if ((!string.IsNullOrEmpty(ShiftStartDay[item.Key]) && string.IsNullOrEmpty(ShiftEndDay[item.Key])) ||
                    (string.IsNullOrEmpty(ShiftStartDay[item.Key]) && !string.IsNullOrEmpty(ShiftEndDay[item.Key])))
                {
                    return Json(new { success = false, message = $"Endtime @{item.Key} không được để trông khi StartTime được chọn." });
                   
                }
            }

            foreach (var item in ShiftStartAfternoon)
            {
                if ((!string.IsNullOrEmpty(ShiftStartAfternoon[item.Key]) && string.IsNullOrEmpty(ShiftEndAfternoon[item.Key])) ||
                    (string.IsNullOrEmpty(ShiftStartAfternoon[item.Key]) && !string.IsNullOrEmpty(ShiftEndAfternoon[item.Key])))
                {
                    return Json(new { success = false, message = $"Endtime @{item.Key} không được để trông khi StartTime được chọn." });

                }
            }

            foreach (var item in ShiftStartNight)
            {
                if ((!string.IsNullOrEmpty(ShiftStartNight[item.Key]) && string.IsNullOrEmpty(ShiftEndNight[item.Key])) ||
                    (string.IsNullOrEmpty(ShiftStartNight[item.Key]) && !string.IsNullOrEmpty(ShiftEndNight[item.Key])))
                {
                    return Json(new { success = false, message = $"Endtime @{item.Key} không được để trông khi StartTime được chọn." });

                }
            }
            if (ViewBag.ErrorDay!=null ||ViewBag.ErrorAfternoon!=null||ViewBag.ErrorNight!=null)
            {
                return View();
            }
            var user = await userManager.GetUserAsync(User);
            UserDoctor doctor = appDbContext.Doctors.FirstOrDefault(x => x.AccountId == user.Id);

            if (ShiftStartDay != null && ShiftEndDay != null)
            {
                SaveShiftData(ShiftStartDay, ShiftEndDay, doctor.id);
            }

            if (ShiftStartAfternoon != null && ShiftEndAfternoon != null)
            {
                SaveShiftData(ShiftStartAfternoon, ShiftEndAfternoon, doctor.id);
            }

            if (ShiftStartNight != null && ShiftEndNight != null)
            {
                SaveShiftData(ShiftStartNight, ShiftEndNight, doctor.id);
            }

            appDbContext.SaveChanges();
            return Json(new { success = true });

        }

        private void SaveShiftData(Dictionary<string, string> shiftStart, Dictionary<string, string> shiftEnd, string doctorId)
        {
            foreach (var item in shiftStart)
            {
                string day = item.Key;
                string startTime = item.Value;
                string endTime = shiftEnd.ContainsKey(day) ? shiftEnd[day] : null;

                if (!string.IsNullOrEmpty(startTime) && !string.IsNullOrEmpty(endTime))
                {
                    DoctorAvailability doctorAvailability = new DoctorAvailability
                    {
                        DoctorId = doctorId,
                        AvailableDate = day,
                        StartTime = TimeSpan.Parse(startTime),
                        EndTime = TimeSpan.Parse(endTime)
                    };

                    appDbContext.DoctorAvailabilities.Add(doctorAvailability);
                }
            }
        }
        public async Task<IActionResult> EditSchedule()
        {
            var user = await userManager.GetUserAsync(User);
            UserDoctor doctor = appDbContext.Doctors.FirstOrDefault(x => x.AccountId == user.Id);

            var doctorschedule = appDbContext.DoctorAvailabilities.Where(x => x.DoctorId == doctor.id).ToList();
            Dictionary<string, List<Tuple<string, string, string>>> data = new Dictionary<string, List<Tuple<string, string, string>>>();

            foreach (var item in doctorschedule)
            {
                string startTimeStr = item.StartTime.ToString(@"hh\:mm");
                string endTimeStr = item.EndTime.ToString(@"hh\:mm");

                if (!data.ContainsKey(item.AvailableDate))
                {
                    data[item.AvailableDate] = new List<Tuple<string, string, string>>();
                }
                if (item.StartTime.Hours >= 7 && item.StartTime.Hours <= 12)
                {
                    data[item.AvailableDate].Add(Tuple.Create("morning", startTimeStr, endTimeStr));
                }
                if (item.StartTime.Hours >= 13 && item.StartTime.Hours <= 18)
                {
                    data[item.AvailableDate].Add(Tuple.Create("afternoon", startTimeStr, endTimeStr));
                }
                if (item.StartTime.Hours >= 19 && item.StartTime.Hours <= 23)
                {
                    data[item.AvailableDate].Add(Tuple.Create("evening", startTimeStr, endTimeStr));
                }
            }
            ViewBag.Schedule = data;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> EditSchedule(Dictionary<string, string> ShiftStartDay, Dictionary<string, string> ShiftEndDay, Dictionary<string, string> ShiftStartAfternoon, Dictionary<string, string> ShiftEndAfternoon, Dictionary<string, string> ShiftStartNight, Dictionary<string, string> ShiftEndNight)
        {

            foreach (var item in ShiftStartDay)
            {
                if ((!string.IsNullOrEmpty(ShiftStartDay[item.Key]) && string.IsNullOrEmpty(ShiftEndDay[item.Key])) ||
                    (string.IsNullOrEmpty(ShiftStartDay[item.Key]) && !string.IsNullOrEmpty(ShiftEndDay[item.Key])))
                {
                    return Json(new { success = false, message = $"Endtime @{item.Key} không được để trông khi StartTime được chọn." });

                }
            }

            foreach (var item in ShiftStartAfternoon)
            {
                if ((!string.IsNullOrEmpty(ShiftStartAfternoon[item.Key]) && string.IsNullOrEmpty(ShiftEndAfternoon[item.Key])) ||
                    (string.IsNullOrEmpty(ShiftStartAfternoon[item.Key]) && !string.IsNullOrEmpty(ShiftEndAfternoon[item.Key])))
                {
                    return Json(new { success = false, message = $"Endtime @{item.Key} không được để trông khi StartTime được chọn." });

                }
            }

            foreach (var item in ShiftStartNight)
            {
                if ((!string.IsNullOrEmpty(ShiftStartNight[item.Key]) && string.IsNullOrEmpty(ShiftEndNight[item.Key])) ||
                    (string.IsNullOrEmpty(ShiftStartNight[item.Key]) && !string.IsNullOrEmpty(ShiftEndNight[item.Key])))
                {
                    return Json(new { success = false, message = $"Endtime @{item.Key} không được để trông khi StartTime được chọn." });

                }
            }
            var user = await userManager.GetUserAsync(User);
            var doctor = appDbContext.Doctors.Where(x => x.AccountId==user.Id).FirstOrDefault();
            if (doctor != null)
            {
                // Truy vấn tất cả các đối tượng Doctor có Id trùng với doctor.Id
                var doctorsToRemove = appDbContext.DoctorAvailabilities.Where(x => x.DoctorId == doctor.id);

                // Xóa tất cả các đối tượng đó
                appDbContext.DoctorAvailabilities.RemoveRange(doctorsToRemove);

                // Lưu thay đổi vào cơ sở dữ liệu
                await appDbContext.SaveChangesAsync();
            }

         
          
            if (ShiftStartDay != null && ShiftEndDay != null)
            {
                SaveShiftData(ShiftStartDay, ShiftEndDay, doctor.id);
            }

            if (ShiftStartAfternoon != null && ShiftEndAfternoon != null)
            {
                SaveShiftData(ShiftStartAfternoon, ShiftEndAfternoon, doctor.id);
            }

            if (ShiftStartNight != null && ShiftEndNight != null)
            {
                SaveShiftData(ShiftStartNight, ShiftEndNight, doctor.id);
            }

            appDbContext.SaveChanges();


            return RedirectToAction("ShowSchedule","Doctor", new { area = "Doctor" });
        }
        public async Task<IActionResult> Cancel(string id)
        {
            UserPatient patient = await appDbContext.Patients
                                 .Include(d => d.AppUser)
                                 .FirstOrDefaultAsync(x => x.id == id);
            var listAppointment = appDbContext.Appointments
    .Include(a => a.Doctor)
    .ThenInclude(p => p.AppUser)
    .Where(x => x.PatientId == patient.id).FirstOrDefault();
            return View(listAppointment);
        }
        [HttpPost]
        public async Task<ActionResult> Cancel(string id, AppointmentDB appointment)
        {
            var user = await appDbContext.Appointments.FirstOrDefaultAsync(a => a.PatientId == id);
            var patient= appDbContext.Patients.Where(a=>a.id== id).FirstOrDefault();
            var doctor=appDbContext.Doctors.Where(a=>a.id==appointment.DoctorId).FirstOrDefault();
            AppointmentCancel p = new AppointmentCancel();
            p.Date=appointment.Date;
            p.FullnamePatient=patient.Fullname;
            p.FullnameDoctor=doctor.Fullname;
            p.PatientId=appointment.PatientId;
            p.DoctorId=appointment.DoctorId;
            appDbContext.AppointmentCancels.Add(p);
            appDbContext.SaveChanges();

            if (user==null)
            {
                 return Json(new { success = false });
            }
            appDbContext.Appointments.Remove(user);
            await appDbContext.SaveChangesAsync();




            return Json(new { success = true });
        }
        public async Task<IActionResult> Statistics()
        {
            var user = await userManager.GetUserAsync(User);
            var userid = user.Id.ToString();
            var doctor = await appDbContext.Doctors.Where(x => x.AccountId==userid).FirstOrDefaultAsync();
            var AppointmentHistory = appDbContext.AppointmentHistory.Where(x => x.DoctorId==doctor.id).ToList();
            Dictionary<int, List<string>> data = new Dictionary<int, List<string>>();
            foreach (var item in AppointmentHistory)
            {
                int month = item.Date.Month;
                if (!data.ContainsKey(month))
                {
                    data[month]= new List<string> { item.Date.ToString() };
                }
                else
                {
                    data[month].Add(item.Date.ToString());
                }

            }
            var DoctorSchedule = appDbContext.DoctorAvailabilities.Where(x => x.DoctorId==doctor.id).ToList();
            Dictionary<string, double> data1 = new Dictionary<string, double>();
            foreach (var item in DoctorSchedule)
            {
                TimeSpan startTime = item.StartTime;
                TimeSpan endTime = item.EndTime;
                TimeSpan result = endTime.Subtract(startTime);
                double totalHours = result.TotalHours;
                if (!data1.ContainsKey(item.AvailableDate))
                {
                    data1[item.AvailableDate]=totalHours;
                }
                else
                {
                    data1[item.AvailableDate]+=totalHours;
                }

            }
            var DayOff = appDbContext.DoctorDayOffs.Where(x => x.DoctorId==doctor.id).ToList();

            Dictionary<int, Tuple<double, int>> data2 = new Dictionary<int, Tuple<double, int>>();
            var appointmentHistory = appDbContext.AppointmentHistory.Where(x => x.DoctorId==doctor.id).ToList();
            for (int i = 1; i<=12; i++)
            {

                data2[i]=Tuple.Create(data1.Values.Sum()*4, 0);
                foreach (var item1 in DayOff)
                {
                    int month = item1.DateOff.Month;
                    string DateofWeek = item1.DateOff.DayOfWeek.ToString();
                    if (month==i)
                    {

                        data2[i]=Tuple.Create(data2[i].Item1-data1[DateofWeek], 0);


                    }
                }

                var total = appointmentHistory.Count(x => x.Date.Month==i);
                if (total!=0)
                {
                    data2[i]=Tuple.Create(data2[i].Item1, total);
                }



            }
            return View(data2);
        }
        public async Task<IActionResult> ShowInfoDoctor()
        {
            var user = await userManager.GetUserAsync(User);
            var doctor = appDbContext.Doctors.Where(x => x.AccountId==user.Id).FirstOrDefault();
            InfoUser user1 = new InfoUser();
            user1.UserName=user.UserName;
            user1.Email=user.Email;
            user1.City=user.City;
            user1.Mobile=user.PhoneNumber;
            user1.Fullname=user.Fullname;
            user1.ImgUrl = doctor.ImgFile;
            user1.DateofBirth=user.Birthday?.ToString("yyyy-MM-dd");
            user1.Qualification=doctor.qualification;
            user1.Specialist=doctor.Specialist;
            user1.Adress=user.Address;
            return View(user1);
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
                var doctor = appDbContext.Doctors.FirstOrDefault(p => p.AccountId == user.Id);
                if (doctor != null)
                {

                    string newImageUrl = Url.Content($"~/PatientsIMG/{filename}");
                    return Json(new { newImageUrl });
                }
            }

            return Json(new { newImageUrl = string.Empty });
        }

        [HttpPost]
        public async Task<IActionResult> SaveImg1(IFormFile ImgFile)
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
                var doctor = appDbContext.Doctors.FirstOrDefault(p => p.AccountId == user.Id);
                if (doctor != null)
                {
                    doctor.ImgFile = filename;
                    await appDbContext.SaveChangesAsync();


                }
            }

            return RedirectToAction("ShowInfoDoctor", "Doctor", new { area = "Doctor" });
        }
    }


}

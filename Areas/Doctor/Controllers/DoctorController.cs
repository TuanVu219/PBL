using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PBL3Hos.Identity;
using PBL3Hos.Models.DbModel;
using PBL3Hos.ViewModel;

namespace PBL3Hos.Areas.Doctor.Controllers
{
    [Area("Doctor")]
    public class DoctorController : Controller
    {
        private readonly SignInManager<AppUser> signInManager;
        private readonly UserManager<AppUser> userManager;
        private readonly AppDbContext appDbContext;


        public DoctorController(SignInManager<AppUser> signInManager, UserManager<AppUser> userManager, AppDbContext dbContext)
        {
            this.signInManager = signInManager;
            this.userManager = userManager;
            this.appDbContext=dbContext;
        }
        public ActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> ShowSchedule()
        {
            var user = await userManager.GetUserAsync(User);
            var doctor= appDbContext.Doctors.Where(x=>x.AccountId==user.Id).FirstOrDefault();
            var schedule = appDbContext.DoctorAvailabilities.Where(x=>x.DoctorId==doctor.id).ToList();
           
            return View(schedule);
        }
        public async Task<IActionResult> ScheduleOff()
        {
            var user = await userManager.GetUserAsync(User);
            UserDoctor doctor = appDbContext.Doctors.Where(x => x.AccountId==user.Id).FirstOrDefault();
            ViewBag.DoctorId=doctor.id;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ScheduleOff(DoctorDayOff dayoff)
        {
           DoctorDayOff doctorDayOff = dayoff;
           appDbContext.DoctorDayOffs.Add(doctorDayOff);
            appDbContext.SaveChanges();
            return View("Index", "Doctor");
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
            public async Task<IActionResult> DoctorSchedule(Dictionary<string,List<string>>AvailableDay, Dictionary<string, string> Shift)
            {
                var user = await userManager.GetUserAsync(User);
                UserDoctor doctor= appDbContext.Doctors.Where(x=>x.AccountId==user.Id).FirstOrDefault();
            
                foreach(var item in Shift)
                {
                string day = item.Key;
             
                    DoctorAvailability doctorAvailability = new DoctorAvailability();

                    doctorAvailability.DoctorId = doctor.id;
                    doctorAvailability.AvailableDate = item.Key;
                    doctorAvailability.Shift = item.Value; // Sử dụng giá trị ca làm việc từ danh sách
                   

                        if (doctorAvailability.Shift == "Day")
                        {
                            doctorAvailability.StartTime = new TimeSpan(7, 0, 0);
                            doctorAvailability.EndTime = new TimeSpan(11, 0, 0);
                        }
                        else if (doctorAvailability.Shift =="Afternoon")
                        {
                            doctorAvailability.StartTime = new TimeSpan(13, 0, 0);
                            doctorAvailability.EndTime = new TimeSpan(17, 0, 0);
                        }
                        else if (doctorAvailability.Shift =="Night")
                         {
                            doctorAvailability.StartTime = new TimeSpan(19, 0, 0);
                            doctorAvailability.EndTime = new TimeSpan(22, 0, 0);


                        }
                        else if (doctorAvailability.Shift =="AllDay")
                        {
                            doctorAvailability.StartTime = new TimeSpan(7, 0, 0);
                            doctorAvailability.EndTime = new TimeSpan(22, 0, 0);


                        }

                appDbContext.DoctorAvailabilities.Add(doctorAvailability);
                    appDbContext.SaveChanges(); 
                
            }


                return View();
            }
        public async Task<IActionResult> EditSchedule()
        {
            var user = await userManager.GetUserAsync(User);
            UserDoctor doctor=appDbContext.Doctors.Where(x=>x.AccountId==user.Id).FirstOrDefault();

            var doctorschedule = appDbContext.DoctorAvailabilities.Where(x => x.DoctorId==doctor.id).ToList();
            Dictionary<string,string> map = new Dictionary<string,string>();
            foreach(var item in doctorschedule)
            {
                map.Add(item.AvailableDate,item.Shift);
            }
            ViewBag.Schedule=map;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> EditSchedule(Dictionary<string,string>Shift)
        {
           var user=await userManager.GetUserAsync(User);
           UserDoctor doctor=appDbContext.Doctors.Where(x=>x.AccountId==user.Id).FirstOrDefault();
            var schedule = appDbContext.DoctorAvailabilities.Where(x => x.DoctorId==doctor.id);
            appDbContext.DoctorAvailabilities.RemoveRange(schedule);
            foreach (var item in Shift)
            {
                DoctorAvailability scheduledoctor = new DoctorAvailability();
                scheduledoctor.DoctorId=doctor.id;
                scheduledoctor.AvailableDate=item.Key;
                scheduledoctor.Shift=item.Value;

                if (scheduledoctor.Shift=="Day")
                {
                    scheduledoctor.StartTime=new TimeSpan(7,0,0);
                    scheduledoctor.EndTime=new TimeSpan(11, 0, 0);

                }
                else if (scheduledoctor.Shift=="Afternoon")
                {
                    scheduledoctor.StartTime=new TimeSpan(13, 0, 0);
                    scheduledoctor.EndTime=new TimeSpan(17, 0, 0);

                }
                else if (scheduledoctor.Shift=="Night")
                {
                    scheduledoctor.StartTime=new TimeSpan(19, 0, 0);
                    scheduledoctor.EndTime=new TimeSpan(22, 0, 0);


                }
                else
                {
                    scheduledoctor.StartTime=new TimeSpan(7, 0, 0);
                    scheduledoctor.EndTime=new TimeSpan(22, 0, 0);
                }
                appDbContext.DoctorAvailabilities.Add(scheduledoctor);
                appDbContext.SaveChanges();

            }
            return View("Index","Home");
        }
    }
    
}

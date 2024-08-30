using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using PBL3Hos.Areas.Admin.Models;
using PBL3Hos.Filters;
using PBL3Hos.Identity;
using PBL3Hos.Models.DbModel;
using PBL3Hos.ViewModel;
using static System.Runtime.InteropServices.JavaScript.JSType;


namespace PBL3Hos.Areas.Admin.Controllers
{
    [Area("Admin")]
    [TypeFilter(typeof(MyAuthenFilter))]
    public class UserController : Controller
    {
        private readonly SignInManager<AppUser> signInManager;
        private readonly UserManager<AppUser> userManager;
        private readonly AppDbContext appDbContext;
        public UserController(SignInManager<AppUser> signInManager, UserManager<AppUser> userManager, AppDbContext dbContext)
        {
            this.signInManager = signInManager;
            this.userManager = userManager;
            this.appDbContext = dbContext;

        }
        public ActionResult Index()
        {
           
         
            Dictionary<int, int> data2 = new Dictionary<int, int>();
            var appointmentHistory = appDbContext.AppointmentHistory.Include(d => d.Doctor).ToList();
            for (int i = 1; i<=12; i++)
            {               
                var total = appointmentHistory.Count(x => x.Date.Month==i);
                if (total!=0)
                {
                    data2[i]=total;
                }
                else
                {
                    data2[i]=0;
                }
            }

            Dictionary<int, List<Tuple<string, int>>> data = new Dictionary<int, List<Tuple<string, int>>>();

            // Lặp qua lịch sử cuộc hẹn để cập nhật dữ liệu
            foreach (var item in appointmentHistory)
            {
                if (!data.ContainsKey(item.Date.Month))
                {
                    // Nếu khóa chưa tồn tại, tạo một danh sách mới và thêm vào
                    data[item.Date.Month] = new List<Tuple<string, int>>();
                }

                var specialistTuple = data[item.Date.Month].FirstOrDefault(t => t.Item1 == item.Doctor.Specialist);
                if (specialistTuple == null)
                {
                    // Nếu chuyên khoa chưa có trong danh sách, thêm một tuple mới với giá trị 1
                    data[item.Date.Month].Add(Tuple.Create(item.Doctor.Specialist, 1));
                }
                else
                {
                    // Nếu chuyên khoa đã tồn tại trong danh sách, tìm tuple và cập nhật giá trị đếm
                    specialistTuple = Tuple.Create(specialistTuple.Item1, specialistTuple.Item2 + 1);
                }
            }

            // Lấy danh sách các chuyên khoa từ cơ sở dữ liệu
            var specialists = appDbContext.Specialists.Select(s => s.SpecialistName).ToList();

            // Lặp qua mỗi chuyên khoa để đảm bảo mỗi chuyên khoa đều có một bản ghi trong data
            foreach (var specialist in specialists)
            {
                for (int i = 1; i <= 12; i++)
                {
                    if (!data.ContainsKey(i))
                    {
                        // Nếu không có dữ liệu cho tháng này, tạo một danh sách mới
                        data[i] = new List<Tuple<string, int>>();
                    }

                    if (data[i].All(t => t.Item1 != specialist))
                    {
                        // Nếu chuyên khoa không tồn tại trong danh sách, thêm một tuple mới với giá trị 0
                        data[i].Add(Tuple.Create(specialist, 0));
                    }
                }
            }

            string jsonData = JsonConvert.SerializeObject(data2);

            ViewBag.JsonData = jsonData;

            string jsonData2 = JsonConvert.SerializeObject(data);

            ViewBag.JsonData2 = jsonData2;

            var patient = appDbContext.Patients.Count();
            var doctor = appDbContext.Doctors.Count();
            var date = DateTime.Now;
            var listappointment = appDbContext.Appointments.Where(x => x.Date.Date==date.Date).Count();
           /* int appointment = 0;*/
          /*  foreach (var item in listappointment)
            {
                item.Date.ToString("MM/dd/yyyy");
                if()
            }
*/
            ViewBag.Patient=patient;
            ViewBag.Doctor=doctor;
            ViewBag.Appointment=listappointment;

            return View();
        }
        public ActionResult ListUser(string search = "")
        {
            ListUser1 listUser1 = new ListUser1();
            var user = userManager.Users.Where(x => x.UserName.Contains(search)).ToList();
            listUser1.users=user;

            return View(listUser1);
        }
        public ActionResult Delete(string id)
        {

            List<AppUser> appuser = userManager.Users.ToList();
            AppUser user1 = appuser.Where(x => x.Id==id).FirstOrDefault();
            return View(user1);
        }
        [HttpPost]
        public async Task<ActionResult> Delete(string id, AppUser p)
        {
            var user = await userManager.FindByIdAsync(id);

            var result = await userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                return RedirectToAction("ListUser", "User", new { area = "Admin" }); // Nếu xóa thành công, chuyển hướng tới danh sách người dùng
            }
            else
            {
                // Xử lý lỗi nếu việc xóa không thành công
                ModelState.AddModelError(string.Empty, "Error deleting user.");
                return View(); // Hoặc trả về view xác nhận xóa không thành công
            }
        }
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterVMDT rmv)
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
                    var result = await userManager.AddToRoleAsync(user, "Doctor");

                    if (result.Succeeded)
                    {
                        UserDoctor doctor = new UserDoctor
                        {
                            NameDoctor=rmv.Username,
                            Phone=rmv.Mobile,
                            AccountId=user.Id,
                            Fullname=user.Fullname,
                            Career=rmv.Career,
                            Specialist=rmv.Specialist,
                            qualification=rmv.qualification,
                            yearofexperience=rmv.yearofexperience,
                            studyprocess=rmv.studyprocess


                        };
                        appDbContext.Doctors.Add(doctor);
                        await appDbContext.SaveChangesAsync();

                        return RedirectToAction("ListDoctor", "User", new { area = "Admin" });
                    }
                }
            }
            return View();



        }
        public async Task<IActionResult> ListPatient()
        {
            var patient = appDbContext.Patients.Include(x=>x.AppUser).ToList();
            return View(patient);
        }

        public async Task<IActionResult> ListDoctor()
        {
            var doctor = appDbContext.Doctors.Include(x => x.AppUser).ToList();
            return View(doctor);
        }

        public async Task<IActionResult> Edit (string id)
        {
          
            var doctor =appDbContext.Doctors.Include(a=>a.AppUser).Where(x=>x.AccountId==id).FirstOrDefault();
            ViewBag.Birthday=doctor.AppUser.Birthday?.ToString("yyyy-MM-dd");
            return View(doctor);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(AppUser appuser, UserDoctor doctor)
        {

           
                var Doctor = appDbContext.Doctors.Where(x => x.AccountId==appuser.Id).FirstOrDefault();
                var Appuser = appDbContext.Users.Where(x => x.Id==appuser.Id).FirstOrDefault();
            var AppointmentCancel = appDbContext.AppointmentCancels.Include(a => a.Doctor).Where(x => x.Doctor.AccountId==appuser.Id).ToList();
            foreach(var item in AppointmentCancel)
            {
                item.FullnameDoctor=appuser.Fullname;
            }


                Appuser.UserName=appuser.UserName;
                Appuser.Fullname=appuser.Fullname;
                Appuser.Address=appuser.Address;
                Appuser.PhoneNumber=appuser.PhoneNumber;
                Appuser.Birthday=appuser.Birthday;
                Appuser.Email=appuser.Email;
                Appuser.City=appuser.City;

                Doctor.Fullname=appuser.Fullname;
                Doctor.Career=doctor.Career;
                Doctor.NameDoctor=appuser.UserName;
                Doctor.Specialist=doctor.Specialist;
                Doctor.qualification=doctor.qualification;
                Doctor.yearofexperience=doctor.yearofexperience;
                Doctor.studyprocess=doctor.studyprocess;

                appDbContext.SaveChanges();

                return RedirectToAction("ListDoctor", "User", new { area = "Admin" });
          
        }
    }

}

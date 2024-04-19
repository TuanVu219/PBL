using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PBL3Hos.Identity;
using PBL3Hos.Models.DbModel;
using PBL3Hos.ViewModel;

namespace PBL3Hos.Areas.Admin.Controllers
{
    [Area("Admin")]
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
                    var result = await userManager.AddToRoleAsync(user, "Doctor");

                    if (result.Succeeded)
                    {
                        UserDoctor doctor = new UserDoctor
                        {
                            NameDoctor=rmv.Username,
                            Phone=rmv.Mobile,
                            AccountId=user.Id


                        };
                        appDbContext.Doctors.Add(doctor);
                        await appDbContext.SaveChangesAsync();

                        return RedirectToAction("Index", "User", new { area = "Admin" });
                    }
                }
            }
            return View();

        }

    }

}

using AllupMVC.DAL;
using AllupMVC.Models;
using AllupMVC.Utilities.Enums;
using AllupMVC.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AllupMVC.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> usermanager;
        private readonly SignInManager<AppUser> signmanager;
        private readonly RoleManager<IdentityRole> userrole;


        public AccountController(UserManager<AppUser> usermanager, SignInManager<AppUser> signmanager, RoleManager<IdentityRole> userrole)
        {
            this.usermanager = usermanager;
            this.signmanager = signmanager;
            this.userrole = userrole;
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM vm)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            AppUser user = new()
            {
                Name = vm.Name,
                Email = vm.Email,
                Surname = vm.Surname,
                UserName = vm.UserName,

            };
            var result = await usermanager.CreateAsync(user, vm.Password);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View();
            }
            await usermanager.AddToRoleAsync(user, UserRoles.User.ToString());
            await signmanager.SignInAsync(user, false);
            return RedirectToAction(nameof(HomeController.Index), "Home");

        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]

        public async Task<IActionResult> Login(LoginVM uservm,string returnUrl)
        {
            if(!ModelState.IsValid)
            {
                return View();
            }

            var user = await usermanager.Users.FirstOrDefaultAsync(u => u.UserName == uservm.UserNameorEmail || u.Email == uservm.UserNameorEmail);
            if(user is null)
            {
                ModelState.AddModelError(string.Empty, "Username or E-Mail Address is not correct");
                return View();
            }
            var result = await signmanager.PasswordSignInAsync(user, uservm.Password, uservm.IsPersistent, true);
           if(result.IsLockedOut)
            {
                ModelState.AddModelError(string.Empty, "Your Account is blocked!Try Again!");
                return View();
            }
           if(!result.Succeeded)
            {
                ModelState.AddModelError(string.Empty, "Username or E-Mail Address is not correct");
                return View();
            }

           if(returnUrl == null)
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
           return Redirect(returnUrl);
        }

        public async Task<IActionResult> Logout()
        {
            await signmanager.SignOutAsync();
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }
        public async Task<IActionResult> CreateRoles()
        {
            foreach (var role in Enum.GetValues(typeof(UserRoles)))
            {
                if (!await userrole.RoleExistsAsync(role.ToString()))
                {
                    await userrole.CreateAsync(new IdentityRole { Name = role.ToString() });
                }

            }
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }
    } 
}
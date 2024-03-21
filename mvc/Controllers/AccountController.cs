using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using mvc.Models;

namespace mvc.Controllers
{
    // [Route("[controller]")]
    public class AccountController : Controller
    {
        private readonly ILogger<AccountController> _logger;
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;

        public AccountController( SignInManager<User> signInManager,UserManager<User> userManager)
        {
            
            _signInManager = signInManager;
            _userManager = userManager;
        }

    
        [Authorize]
        public IActionResult Index()
        {   
            ViewData["Id"] = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            ViewData["userName"] = User.Identity.Name;
            return View();
        }
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new User { UserName = model.UserName, Email = model.Email };
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                
                    return RedirectToAction("Login");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.Users.SingleOrDefaultAsync(x => x.Email == model.Email);

                if (user == null) return View(model);
                else{
                    // Sign in the user after creating an account
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Index");
                }    
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }


        // public IActionResult SomeAction()
        // {
        //     if (User.Identity.IsAuthenticated)
        //     {
        //         // User is authenticated, perform some action
        //     }
        //     else
        //     {
        //         // User is not authenticated, handle accordingly
        //     }
        // }
    }
}
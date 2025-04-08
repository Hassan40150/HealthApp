using System;
using HealthApp.Helpers;
using HealthApp.ViewModels;
using Microsoft.AspNetCore.Mvc;


namespace HealthApp.Controllers
{
    public class RegisterController : Controller
    {

        private readonly ApplicationDbContext _context;

        public RegisterController(ApplicationDbContext context)
        {
            _context = context;
        }



        public IActionResult Index()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new Users
                {
                    Name = model.Name,
                    // Password = model.Password,
                    Password = PasswordHelper.HashPassword(model.Password),

                    Email = model.Email
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index", "Dashboard");
            }

            return View(model);
        }





    }
}

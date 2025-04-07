using System;
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
                    Password = model.Password, // Consider hashing this in production
                    Email = model.Email
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index", "Home"); // or wherever you want to go after registering
            }

            return View(model);
        }





    }
}

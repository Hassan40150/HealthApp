using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;
using HealthApp.Data; // adjust if your DbContext is elsewhere

namespace HealthApp.Attributes
{
    public class OnboardingRequiredAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var user = context.HttpContext.User;
            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim))
            {
                context.Result = new RedirectToActionResult("Index", "Landing", null);
                return;
            }

            var db = context.HttpContext.RequestServices.GetService(typeof(ApplicationDbContext)) as ApplicationDbContext;
            int userId = int.Parse(userIdClaim);

            var profile = db.UserProfiles.FirstOrDefault(p => p.UserID == userId);

            if (profile == null)
            {
                context.Result = new RedirectToActionResult("Index", "Landing", null);
                return;
            }

            // 🛑 FIX HERE: Avoid redirecting when already on an onboarding view
            var controller = context.RouteData.Values["controller"]?.ToString();
            var action = context.RouteData.Values["action"]?.ToString();

            if (profile.OnboardingComplete && controller == "Onboarding")
            {
                context.Result = new RedirectToActionResult("Index", "Dashboard", null);
                return;
            }

            if (!profile.OnboardingComplete && controller != "Onboarding")
            {
                context.Result = new RedirectToActionResult("OnboardingWelcome", "Onboarding", null);
                return;
            }

            base.OnActionExecuting(context);
        }

    }

}

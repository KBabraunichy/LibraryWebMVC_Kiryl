using LibraryWebMVC.Interfaces;
using LibraryWebMVC.Models;
using Microsoft.AspNetCore.Mvc;

namespace LibraryWebMVC.Controllers
{
    public class LoginController : Controller
    {

        private readonly IApiRequestService _apiRequest;

        public LoginController(IApiRequestService apiRequest)
        {
            _apiRequest = apiRequest;
        }
        public IActionResult LogIn()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> LogInAsync(UserLogin user)
        {

            var response = await _apiRequest.PostRequestResponseMessage("api/Login", user);

            if (response.IsSuccessStatusCode)
            {
                var resToken = response.Content.ReadAsStringAsync().Result;

                Response.Cookies.Append("X-Access-Token", resToken, new CookieOptions()
                {
                    HttpOnly = true,
                    SameSite = SameSiteMode.Strict,
                    Secure = true,
                    Expires = DateTime.Now.AddMinutes(15)
                });
                return Redirect("/Home/Catalog");
            }
            
            ViewBag.ErrorMessage = "Incorrect username or password";
            return View();
        }

        public IActionResult LogOut()
        {
            Response.Cookies.Delete("X-Access-Token");

            return Redirect("/Home/Index");
        }
    }
}

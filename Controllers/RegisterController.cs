using LibraryWebMVC.Interfaces;
using LibraryWebMVC.Models;
using Microsoft.AspNetCore.Mvc;

namespace LibraryWebMVC.Controllers
{
    public class RegisterController : Controller
    {

        private readonly IApiRequestService _apiRequest;

        public RegisterController(IApiRequestService apiRequest)
        {
            _apiRequest = apiRequest;
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RegisterAsync(User user)
        {
            var response = await _apiRequest.PostRequestResponseMessage(UserApi, user);

            if (response.IsSuccessStatusCode)
            {
                return Redirect("/Home/Catalog");
            }
            string errorMessage = await response.Content.ReadAsStringAsync();
            ViewBag.ErrorMessage = errorMessage.Trim('"');
           
            
            return View();
        }
    }
}

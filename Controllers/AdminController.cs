using Microsoft.AspNetCore.Mvc;
using LibraryWebMVC.Interfaces;
using LibraryWebMVC.Models;
using Newtonsoft.Json;
using LibraryWebMVC.Utils;
using LibraryWebMVC.ViewModels;

namespace LibraryWebMVC.Controllers
{
    public class AdminController : Controller
    {
        private readonly IApiRequestService _apiRequest;

        private static string Api { get; set; } = "";
        private static int Id { get; set; } = 0;

        public AdminController(IApiRequestService apiRequest)
        {
            _apiRequest = apiRequest;
        }

        public async Task<IActionResult> AdminToolsAsync()
        {

            bool isAdmin = await _apiRequest.IsAdmin();
            if (isAdmin)
            {
                var booksJSON = await _apiRequest.Get("Books");
                var authorsJSON = await _apiRequest.Get("Authors");


                if (booksJSON is null || authorsJSON is null)
                {
                    ViewBag.ErrorMessage = "There was an error when receiving data";
                    return View();
                }
                else
                {
                    var booksList = JsonConvert.DeserializeObject<List<Book>>(booksJSON);
                    var authorsList = JsonConvert.DeserializeObject<List<Author>>(authorsJSON);

                    LibViewModel models = new LibViewModel
                    {
                        Books = booksList,
                        Authors = authorsList
                    };

                    return View(models);
                }
       
            }

            return Redirect("~/Admin/AccessDenied");

        }

        public IActionResult AccessDeniedAsync()
        {
            return View();
        }


        public async Task<IActionResult> EditAsync(string api, int id, string errorMessage = null)
        {

            bool isAdmin = await _apiRequest.IsAdmin();
            if (isAdmin)
            {
                if (id == 0)
                {
                    return Redirect("~/Admin/AdminTools");
                }
                var objectJSON = await _apiRequest.Get(api, id);

                if (objectJSON is null)
                {

                    ViewBag.ErrorMessage = "There was an error when receiving data";
                    return Redirect("~/Admin/AdminTools");
                }
                else
                {
                    CollectionItems bookCollectionItem;
                    if(api.Equals("Authors"))
                    {
                        var author = JsonConvert.DeserializeObject<Author>(objectJSON);

                        bookCollectionItem = new CollectionItems { Author = author, Book = null };
                        ViewBag.IsAuthor = true;
                        
                    }
                    else
                    {
                        
                        var book = JsonConvert.DeserializeObject<Book>(objectJSON);
                        
                        bookCollectionItem = new CollectionItems { Author = null, Book = book };
                        ViewBag.IsAuthor = false;
                    }

                    Api = api;
                    Id = id;

                    if(errorMessage is not null) 
                        ViewBag.ErrorMessage = errorMessage;

                    return View(bookCollectionItem);
                }

            }
            return Redirect("~/Admin/AccessDenied");
           
        }

        [HttpPost]
        public async Task<IActionResult> EditBookAsync(Book book)
        {
            bool isAdmin = await _apiRequest.IsAdmin();
            if (isAdmin)
            {
                var objectJSON = await _apiRequest.Get(Api, Id);

                if (objectJSON is null)
                {
                    ViewBag.ErrorMessage = "There was an error when receiving data";
                    return View();
                }

                var stringResponse = await _apiRequest.Put(Api, Id, book);
                if (!string.IsNullOrEmpty(stringResponse))
                {
                    return RedirectToAction("Edit", new { api = Api, id = Id, errorMessage = stringResponse });
                }

                return Redirect("~/Admin/AdminTools");
            }

            return Redirect("~/Admin/AccessDenied");
        }

        [HttpPost]
        public async Task<IActionResult> EditAuthorAsync(Author author)
        {
            bool isAdmin = await _apiRequest.IsAdmin();
            if (isAdmin)
            {
                var objectJSON = await _apiRequest.Get(Api, Id);

                if (objectJSON is null)
                {
                    ViewBag.ErrorMessage = "There was an error when receiving data";
                    return View();
                }

                var stringResponse = await _apiRequest.Put(Api, Id, author);
                if (!string.IsNullOrEmpty(stringResponse))
                {
                    return RedirectToAction("Edit", new { api = Api, id = Id, errorMessage = stringResponse });
                }

                return Redirect("~/Admin/AdminTools");

            }

            return Redirect("~/Admin/AccessDenied");
        }

        public async Task<IActionResult> DeleteAsync(string api, int id)
        {

            bool isAdmin = await _apiRequest.IsAdmin();
            if (isAdmin)
            {
                if (id == 0)
                {
                    return Redirect("~/Admin/AdminTools");
                }
                var objectJSON = await _apiRequest.Get(api, id);

                if (objectJSON is null)
                {
                    return Redirect("~/Admin/AdminTools");
                }
                var stringMessage = _apiRequest.Delete(api, id);

                if (stringMessage is not null)
                    ViewBag.ErrorMessage = stringMessage;

                return Redirect("~/Admin/AdminTools");
            }

            return Redirect("~/Admin/AccessDenied");

        }


        public async Task<IActionResult> CreateAsync(string api, string errorMessage = null)
        {

            bool isAdmin = await _apiRequest.IsAdmin();
            if (isAdmin)
            {
                Api = api;

                if (api.Equals("Authors"))
                {
                    ViewBag.IsAuthor = true;
                }
                else
                {
                    ViewBag.IsAuthor = false;
                }

                if (errorMessage is not null)
                    ViewBag.ErrorMessage = errorMessage;

                return View();

            }

            return Redirect("~/Admin/AccessDenied");

        }

        [HttpPost]
        public async Task<IActionResult> CreateBookAsync(Book book)
        {
            bool isAdmin = await _apiRequest.IsAdmin();
            if (isAdmin)
            {
                var stringResponse = await _apiRequest.Post(Api, book);
                if (!string.IsNullOrEmpty(stringResponse))
                {
                    return RedirectToAction("Create", new { api = Api, errorMessage = stringResponse });
                }

                return Redirect("~/Admin/AdminTools");

            }

            return Redirect("~/Admin/AccessDenied");
        }

        [HttpPost]
        public async Task<IActionResult> CreateAuthorAsync(Author author)
        {
            bool isAdmin = await _apiRequest.IsAdmin();
            if (isAdmin)
            {
                var stringResponse = await _apiRequest.Post(Api, author);
                if (!string.IsNullOrEmpty(stringResponse))
                {
                    return RedirectToAction("Create", new { api = Api, errorMessage = stringResponse });
                }

                return Redirect("~/Admin/AdminTools");

            }

            return Redirect("~/Admin/AccessDenied");
        }

    }
}

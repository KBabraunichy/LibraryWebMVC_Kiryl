using LibraryWebMVC.Interfaces;
using LibraryWebMVC.Utils;
using LibraryWebMVC.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace LibraryWebMVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly IApiRequestService _apiRequest;
        private readonly IApiAuthService _apiAuthService;

        private static List<CollectionItems> LibraryCollection;

        public HomeController(IApiRequestService apiRequest, IApiAuthService apiAuthService)
        {
            _apiRequest = apiRequest;
            _apiAuthService = apiAuthService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Catalog(string firstName, string bookName, int page = 1)
        {
            var authorizedRole = await _apiAuthService.GetAuthorizedRole();
            if (authorizedRole is not null)
            {
                int pageSize = 7;

                List<CollectionItems> libraryCollection = await _apiRequest.GetList();

                if (!string.IsNullOrEmpty(firstName))
                {
                    libraryCollection = libraryCollection.Where(p => p.Author.FirstName.ToLower().Contains(firstName.ToLower())).ToList();
                }
                if (!string.IsNullOrEmpty(bookName))
                {
                    libraryCollection = libraryCollection.Where(p => p.Book.Name.ToLower().Contains(bookName.ToLower())).ToList();
                }

                LibraryCollection = libraryCollection;


                var count = libraryCollection.Count();
                var pagedLibraryCollection = libraryCollection.Skip((page - 1) * pageSize).Take(pageSize).ToList();

                PageViewModel pageViewModel = new PageViewModel(count, page, pageSize);
                FilterViewModel filterViewModel = new FilterViewModel(firstName, bookName);

                CollectionItemsViewModel viewModel = new CollectionItemsViewModel
                {
                    CollectionItems = pagedLibraryCollection,
                    PageViewModel = pageViewModel,
                    FilterViewModel = filterViewModel
                };

                ViewBag.IsNotNull = true;
                return View(viewModel);
                
            }
            ViewBag.IsNotNull = false;
            return View();

        }


        public async Task SaveDocumentAsync()
        {

            string libraryData = string.Empty;

            foreach (CollectionItems obj in LibraryCollection)
            {
                libraryData += obj.ToString();
                libraryData += '\n';
            }

            Response.Clear();
            Response.ContentType = "text/csv";
            Response.Headers["Content-Disposition"] = $"attachment;filename={FileName}.csv";

            
            Task task = Task.Run(() => {
                Response.WriteAsync(libraryData);
            });

            task.Wait();

            await Response.CompleteAsync();

        }
    }
}
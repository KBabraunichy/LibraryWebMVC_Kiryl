using LibraryWebMVC.Interfaces;
using LibraryWebMVC.Models;
using LibraryWebMVC.Utils;
using Microsoft.Extensions.Http;
using Newtonsoft.Json;
using Polly;
using Polly.Retry;
using RestEase;
using System.Net;
using System.Net.Http.Headers;
using System.Text;

namespace LibraryWebMVC.Services
{
    public class ApiRequestService : IApiRequestService
    {

        private readonly IConfiguration _config;
        private readonly IRestEaseService _catalogService;
        private readonly IHttpContextAccessor _contextAccessor;

        public IHttpClientFactory _httpClientFactory;

        public ApiRequestService(IConfiguration configuration, IHttpContextAccessor contextAccessor, IHttpClientFactory httpClientFactory)
        {
            _config = configuration;
            _contextAccessor = contextAccessor;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<HttpResponseMessage?> PostRequestResponseMessage(string api, object body)
        {

            using (var client = _httpClientFactory.CreateClient())
            {
                var data = new StringContent(System.Text.Json.JsonSerializer.Serialize(body), Encoding.UTF8, "application/json");

                var response = await client.PostAsync(api, data);

                return response;
            }

        }

        //restease part

        public async Task<List<CollectionItems>> GetList()
        {
            if (_contextAccessor.HttpContext.Request.Cookies.TryGetValue("X-Access-Token", out string token))
            {
                _catalogService.Authorization = new AuthenticationHeaderValue("Bearer", token.Trim('"'));

                var authorsJSON = await Get("Authors");
                var booksJSON = await Get("Books");

                if (!string.IsNullOrEmpty(authorsJSON) && !string.IsNullOrEmpty(booksJSON))
                {
                    var authorsList = JsonConvert.DeserializeObject<List<Author>>(authorsJSON);
                    var booksList = JsonConvert.DeserializeObject<List<Book>>(booksJSON);

                    return new List<CollectionItems>(
                        from authors in authorsList
                        join books in booksList on authors.Id equals books.AuthorId
                        select new CollectionItems
                        {
                            Author = new Author
                            {
                                Id = authors.Id,
                                FirstName = authors.FirstName,
                                LastName = authors.LastName,
                                SurName = authors.SurName,
                                BirthDate = authors.BirthDate
                            },
                            Book = new Book
                            {
                                Name = books.Name,
                                Year = books.Year,
                                AuthorId = books.AuthorId
                            }
                        }
                    );
                }
            }

            return null;

        }

        public async Task<string> Get(string api, int id = 0) 
        {

            string objectJSON;
            if (id == 0)
            {
                objectJSON = await _catalogService.GetObjectListAsync(api);
            }
            else
            {
                objectJSON = await _catalogService.GetObjectAsync(api, id);
            }

            if (!string.IsNullOrEmpty(objectJSON))
            {
                return objectJSON;
            }
            
            return null;
        }

        public async Task<string> Put(string api, int id, object obj)
        {
            try
            {
                string objectJSON = await _catalogService.PutObjectAsync(api, id, obj);

                return null;
            }
            catch(ApiException e)
            {
                Console.WriteLine(e.Content);
                return e.Content;
            }

        }

        public async Task<string> Delete(string api, int id)
        {
            try
            {
                string objectJSON = await _catalogService.DeleteObjectAsync(api, id);
                Thread.Sleep(1000);
                return null;
            }
            catch (ApiException e)
            {
                Console.WriteLine(e.Content);
                return e.Content;
            }

        }

        public async Task<string> Post(string api, object obj)
        {
            try
            {
                string objectJSON = await _catalogService.PostObjectAsync(api, obj);

                return null;
            }
            catch (ApiException e)
            {
                Console.WriteLine(e.Content);
                return e.Content;
            }

        }

    }
}

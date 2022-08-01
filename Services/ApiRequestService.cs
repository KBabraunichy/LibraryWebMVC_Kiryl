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
        private const int MaxRetries = 3;

        private IHttpContextAccessor _contextAccessor;
        private readonly IConfiguration _config;
        private readonly ICatalogService _catalogService;
        private readonly string _baseUrl;
        private static string Token;
        private readonly AsyncRetryPolicy _retryPolicy;
        private readonly AsyncRetryPolicy<HttpResponseMessage> _retryRestEasePolicy;

        public ApiRequestService(IHttpContextAccessor contextAccessor, IConfiguration configuration)
        {
            _contextAccessor = contextAccessor;
            _config = configuration;
            _baseUrl = _config["WebAPI:BaseUrl"];

            _retryPolicy = Policy
                .Handle<HttpRequestException>()
                .WaitAndRetryAsync(MaxRetries, times => TimeSpan.FromMilliseconds(times * 100));

            _retryRestEasePolicy = Policy
                .HandleResult<HttpResponseMessage>(r => r.StatusCode == HttpStatusCode.BadRequest)
                .WaitAndRetryAsync(MaxRetries, times => TimeSpan.FromMilliseconds(times * 100));

            _catalogService = RestClient.For<ICatalogService>(_baseUrl, new PolicyHttpMessageHandler(_retryRestEasePolicy));

            
        }

        public async Task<HttpResponseMessage?> PostRequestResponseMessage(string api, object body)
        {
            return await _retryPolicy.ExecuteAsync(async () =>
            {
                using (var client = new HttpClient())
                {

                    client.BaseAddress = new Uri(_baseUrl);
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    var data = new StringContent(System.Text.Json.JsonSerializer.Serialize(body), Encoding.UTF8, "application/json");

                    var response = await client.PostAsync(api, data);

                    return response;
                }
            });

        }

        public async Task<bool> IsAuthorized()
        {
            if (_contextAccessor.HttpContext.Request.Cookies.TryGetValue("X-Access-Token", out string token))
            {
                Token = token;

                return await _retryPolicy.ExecuteAsync(async () =>
                {
                    using (var client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(_baseUrl);
                        client.DefaultRequestHeaders.Clear();
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Trim('"'));

                        var response = await client.PostAsync("api/Authors", null);

                        int statusCode = (int)response.StatusCode;

                        if (statusCode == 403 || statusCode == 415)
                        {
                            return true;
                        }
                        return false;
                    }
                });
            }

            return false;
        }

        public async Task<bool> IsAdmin()
        {
            var isAuthorized = await IsAuthorized();
            if (isAuthorized)
            {
                return await _retryPolicy.ExecuteAsync(async () =>
                {
                    using (var client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(_baseUrl);
                        client.DefaultRequestHeaders.Clear();
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Token.Trim('"'));

                        var response = await client.PostAsync("api/Authors", null);

                        int statusCode = (int)response.StatusCode;

                        if (statusCode == 415)
                        {
                            return true;
                        }
                        return false;
                    }
                });
            }

            return false;
        }

        //restease part

        public async Task<List<CollectionItems>> GetList()
        {

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
            

            return null;

        }

        public async Task<string> Get(string api, int id = 0) 
        {

            _catalogService.Authorization = new AuthenticationHeaderValue("Bearer", Token.Trim('"'));

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

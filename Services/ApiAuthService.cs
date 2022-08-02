

using LibraryWebMVC.Interfaces;
using System.Net.Http.Headers;

namespace LibraryWebMVC.Services
{
    public class ApiAuthService : IApiAuthService
    {
        private IHttpContextAccessor _contextAccessor;
        private readonly string _baseUrl;
        private readonly IHttpClientFactory _httpClientFactory;

        public ApiAuthService(IHttpContextAccessor contextAccessor, IHttpClientFactory httpClientFactory)
        {
            _contextAccessor = contextAccessor;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<string> GetAuthorizedRole()
        {
            if (_contextAccessor.HttpContext.Request.Cookies.TryGetValue("X-Access-Token", out string token))
            {

                using (var client = _httpClientFactory.CreateClient())
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Trim('"'));

                    var response = await client.GetAsync(AuthorsApi);

                    if (response.IsSuccessStatusCode)
                    {
                        return await response.Content.ReadAsStringAsync();
                    }
                }
                
            }

            return null;
        }
    }
}

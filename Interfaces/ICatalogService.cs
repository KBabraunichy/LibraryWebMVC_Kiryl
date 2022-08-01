using RestEase;
using System.Net.Http.Headers;

namespace LibraryWebMVC.Interfaces
{
    public interface ICatalogService
    {
        [Header("Authorization")]
        AuthenticationHeaderValue Authorization { get; set; }

        [AllowAnyStatusCode]
        [Get("api/{path}")]
        Task<string> GetObjectListAsync([Path] string path);

        [AllowAnyStatusCode]
        [Get("api/{path}/{id}")]
        Task<string> GetObjectAsync([Path] string path, [Path] int id);

        [Post("api/{path}")]
        Task<string> PostObjectAsync([Path] string path, [Body] object obj);

        [Put("api/{path}/{id}")]
        Task<string> PutObjectAsync([Path] string path, [Path] int id, [Body] object obj);

        [Delete("api/{path}/{id}")]
        Task<string> DeleteObjectAsync([Path] string path, [Path] int id);
    }
}

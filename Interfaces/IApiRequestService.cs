using LibraryWebMVC.Utils;
using RestEase;

namespace LibraryWebMVC.Interfaces
{
    public interface IApiRequestService
    {
        public Task<HttpResponseMessage?> PostRequestResponseMessage(string api, object body);
        public Task<bool> IsAuthorized();
        public Task<bool> IsAdmin();

        //for restease 

        public Task<List<CollectionItems>> GetList();

        public Task<string> Get(string api, int id = 0);

        public Task<string> Put(string api, int id, object obj);

        public Task<string> Delete(string api, int id);

        public Task<string> Post(string api, object obj);

    }
}

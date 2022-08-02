namespace LibraryWebMVC.Interfaces
{
    public interface IApiAuthService
    {
        public Task<string> GetAuthorizedRole();
    }
}

using LibraryWebMVC.Models;

namespace LibraryWebMVC.ViewModels
{
    public class LibViewModel
    {
        public IEnumerable<Author> Authors { get; set; }
        public IEnumerable<Book> Books { get; set; }
    }
}
